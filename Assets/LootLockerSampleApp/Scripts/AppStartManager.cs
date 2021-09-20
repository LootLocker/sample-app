﻿using LootLocker;
using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LootLockerDemoApp
{
    public class AppStartManager : UIScreenView, ILootLockerStageOwner
    {
        public InputField usernameField, passwordField, sixDigitCodeField;
        public Transform spinner;
        public float spinnerRotateSpeed = 300;
        public GameObject carouselScreen, activatingScreen, successScreen,
            normalLoginContent, failedLoginContent, twoAF, twoAFFailedLoginContent, twoAFNormalLoginContent;
        private string mfa_key;

        public override void Awake()
        {
            base.Awake();
#if !UNITY_EDITOR
        usernameField.text = PlayerPrefs.GetString("AdminUserName","");
        passwordField.text = PlayerPrefs.GetString("AdminPassword","");
#endif
        }
        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            carouselScreen.SetActive(true);
            failedLoginContent.SetActive(false);
            normalLoginContent.SetActive(true);
            successScreen.SetActive(false);
        }


        public void Login()
        {

            if (string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(passwordField.text))
            {
                Debug.Log("Please fill both username and password fields.");
                return;
            }

            PlayerPrefs.SetString("AdminUserName", usernameField.text);
            PlayerPrefs.SetString("AdminPassword", passwordField.text);

            carouselScreen.SetActive(false);
            activatingScreen.SetActive(true);

            ///Connect to the Intial API request passing in username and password, Note:This is an admin call and is not necessary for the normal gameplay
            DemoSDKManager.InitialAuthRequest(usernameField.text, passwordField.text, (response) =>
            {
                if (response.success)
                {
                    if (!String.IsNullOrEmpty(response.mfa_key))
                    {
                        twoAF.SetActive(true);
                        activatingScreen.SetActive(false);
                        mfa_key = response.mfa_key;
                    }
                    else
                    {
                        //If authentication is successful, lets try to create a demo game or connect to the current demo game
                        DemoAppAdminRequests.token = response.auth_token;
                        LoginAndCreateGame(response);

                    }
                }
                else
                {
                    activatingScreen.SetActive(false);
                    carouselScreen.SetActive(true);
                    normalLoginContent.SetActive(false);
                    failedLoginContent.SetActive(true);
                    twoAF.SetActive(false);
                }
            });
        }


        public void GoToPlayerScreen()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Player);
        }

        private void LoginAndCreateGame(LootLockerAuthResponse response)
        {
            int organisationID = response.user.organisations.FirstOrDefault().id;
            AppManager.activeOrganisationID = organisationID;
            ///lets try to find the demo game from the list of games coming from the server
            LootLockerGameAndDevelopment demoGame = response.user.organisations.FirstOrDefault()?.games?.FirstOrDefault(x => x.is_demo == true);

            //if we found one, lets set the config up
            if (demoGame != null)
            {
                Debug.Log("There's already a Demo Game. Moving on.");
                AppManager.activeGameID = demoGame.id;
                LootLockerConfig.current.gameID = !LootLockerConfig.current.developmentMode ? demoGame.id : demoGame.development.id;
             //   Debug.LogError($"Game id {demoGame.id} with name {demoGame.name}");
                ///Lets pull more information about the game
                DemoSDKManager.GetDetailedInformationAboutAGame(LootLockerConfig.current.gameID.ToString(), (createGameResponse) =>
                {
                    if (createGameResponse.success)
                    {
                        //update the config of the game
                        LootLockerConfig.current.token = (createGameResponse.game.game_key);
                        LootLockerConfig.current.apiKey = (createGameResponse.game.game_key);
                        successScreen.SetActive(true);
                        activatingScreen.SetActive(false);
                    }
                    else
                    {
                        activatingScreen.SetActive(false);
                        carouselScreen.SetActive(true);
                        normalLoginContent.SetActive(false);
                        failedLoginContent.SetActive(true);
                    }
                });
            }
            else
            {
                //There's no "Demo Game". Create it.
                Debug.LogWarning("No game called Demo Game. Creating one...");
                DemoSDKManager.CreatingAGame("Demo Game", "0", true, organisationID, true, (createGameResponse) =>
                {
                    //we were succesful in creating a demo game
                    if (createGameResponse.success)
                    {
                        Debug.Log("Successful created a demo game: " + createGameResponse.text);
                        AppManager.activeGameID = createGameResponse.game.id;
                        LootLockerConfig.current.gameID = !LootLockerConfig.current.developmentMode ? createGameResponse.game.id : createGameResponse.game.development.id;

                        //   Debug.LogError($"Game id {createGameResponse.game.id} with name {createGameResponse.game.name}");
                        DemoSDKManager.GetDetailedInformationAboutAGame(LootLockerConfig.current.gameID.ToString(), (createGameResponses) =>
                        {
                            if (createGameResponses.success)
                            {
                                LootLockerConfig.current.token = (createGameResponses.game.game_key);
                                LootLockerConfig.current.apiKey = (createGameResponse.game.game_key);
                                successScreen.SetActive(true);
                                activatingScreen.SetActive(false);
                            }
                            else
                            {
                                activatingScreen.SetActive(false);
                                carouselScreen.SetActive(true);
                                normalLoginContent.SetActive(false);
                                failedLoginContent.SetActive(true);
                            }
                        });
                    }
                    else
                    {

                        activatingScreen.SetActive(false);
                        carouselScreen.SetActive(true);
                        normalLoginContent.SetActive(false);
                        failedLoginContent.SetActive(true);
                    }
                });
            }
        }
        /// <summary>
        /// if we are using 2FA
        /// </summary>
        public void Verify2FACode()
        {
            carouselScreen.SetActive(false);
            activatingScreen.SetActive(true);
            twoAF.SetActive(false);

            DemoSDKManager.TwoFactorAuthVerification(mfa_key, sixDigitCodeField.text, (response) =>
            {
                if (response.success)
                {
                    DemoAppAdminRequests.token = response.auth_token;
                    LoginAndCreateGame(response);
                    activatingScreen.SetActive(false);
                }
                else
                {
                    activatingScreen.SetActive(false);
                    twoAF.SetActive(true);
                    twoAFFailedLoginContent.SetActive(true);
                    twoAFNormalLoginContent.SetActive(false);
                }
            });
        }

        public void Reset2FALayout()
        {
            twoAFNormalLoginContent.SetActive(true);
            twoAFFailedLoginContent.SetActive(false);
        }

        public void Discord()
        {
            Application.OpenURL("https://discord.com/invite/XG9KSP4");
        }

        private void Update()
        {

            spinner.Rotate(0, 0, -spinnerRotateSpeed * Time.deltaTime);

        }

    }
}