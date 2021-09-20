using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LootLocker.Requests;
using Newtonsoft.Json;
using System;
using LootLocker;

namespace LootLockerDemoApp
{
    public class CreatePlayerRequest : ILootLockerScreenData
    {
        public string playerName;
    }

    public class PlayerManager : UIScreenView
    {

        [Header("Screens")]
        public GameObject playersScreen;
        public GameObject createPlayerScreen;

        [Header("----------------------------------------------")]
        public GameObject playerElementPrefab;
        public Transform playersContent;
        public InputField newPlayerName;
        public Dictionary<LocalPlayer, GameObject> playerElements = new Dictionary<LocalPlayer, GameObject>();


        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            UpdateScreenData(screenData);
        }

        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            ListPlayers();
        }

        protected override void InternalEasyPrefabSetup()
        {
            base.InternalEasyPrefabSetup();
            ListPlayers();
        }

        public void ListPlayers()
        {

            if (!PlayerPrefs.HasKey(dataObject?.playerStorageKeyNameToUse))
                PlayerPrefs.SetString(dataObject?.playerStorageKeyNameToUse, JsonConvert.SerializeObject(new List<LocalPlayer>()));

            List<LocalPlayer> localPlayers = JsonConvert.DeserializeObject<List<LocalPlayer>>(PlayerPrefs.GetString(dataObject?.playerStorageKeyNameToUse));

            FillPlayers(localPlayers);

        }

        void FillPlayers(List<LocalPlayer> players)
        {

            for (int i = 0; i < playersContent.childCount; i++)
                Destroy(playersContent.GetChild(i).gameObject);

            playerElements = new Dictionary<LocalPlayer, GameObject>();

            foreach (LocalPlayer user in players)
            {

                GameObject playerElementObject = Instantiate(playerElementPrefab, playersContent);
                playerElementObject.GetComponentInChildren<Text>().text = user.playerName;
                playerElementObject.GetComponent<Button>().onClick.AddListener(() => SelectPlayer(user));
                playerElements.Add(user, playerElementObject);
            }
        }

        public void ClickCreateNewPlayer()
        {
            playersScreen.SetActive(false);
            createPlayerScreen.SetActive(true);
        }

        public void Back()
        {
            playersScreen.SetActive(true);
            createPlayerScreen.SetActive(false);
        }

        public void ClickNextOnName()
        {
            if (string.IsNullOrEmpty(newPlayerName.text))
                return; //TODO: Show a message saying player name can't be empty
            createPlayerScreen.SetActive(false);
            playersScreen.SetActive(true);
            StartNewSession();
        }

        public void StartNewSession()
        {
            Guid guid = Guid.NewGuid();
            LoadingManager.ShowLoadingScreen();
            localPlayer = new LocalPlayer { playerName = newPlayerName.text, uniqueID = guid.ToString(), characterClass = null };
            StartSession(localPlayer, (response) =>
            {
                Debug.Log("Created Session for new player with id: " + guid.ToString());
                dataObject.SavePlayer(localPlayer.playerName, localPlayer.uniqueID);
                LootLockerSDKManager.SetPlayerName(localPlayer.playerName, null);
                //we want to reset the current character
                dataObject?.SaveCharacter(new LootLockerCharacter { name = "None", type = "None" });
                uiManager.OpenUI(UIScreen.UIScreensType.CreateCharacter,screenData: localPlayer);
                LoadingManager.HideLoadingScreen();
            },
            () =>
            {
                LoadingManager.HideLoadingScreen();
            });
        }

        public void StartSession(LocalPlayer player, Action<LootLockerSessionResponse> onStartSessionCompleted, Action onSessionStartingFailed = null)
        {
            LoadingManager.ShowLoadingScreen();
            //Starting a new session using the new id that has been created
            LootLockerSDKManager.StartSession(player.uniqueID, (response) =>
            {
                if (response.success)
                {
                    dataObject.SaveSession(response);
                    onStartSessionCompleted?.Invoke(response);
                    LoadingManager.HideLoadingScreen();
                }
                else
                {
                    onSessionStartingFailed?.Invoke();
                    Debug.LogError("Session failure: " + response.text);
                }

            });
        }

        public void SelectPlayer(LocalPlayer selectedPlayer)
        {
            if (isEasyPrefab)
            {
                Debug.LogError("You clicked on player " + selectedPlayer.playerName + " thats all we know :) ");
                return;
            }
            playersScreen.SetActive(false);
            createPlayerScreen.SetActive(false);
            LoadingManager.ShowLoadingScreen();
            StartSession(selectedPlayer, (response) =>
            {
                playersScreen.SetActive(true);
                Debug.Log("Logged in successfully.");
            
                dataObject.SaveCharacter(selectedPlayer.playerName, selectedPlayer.characterClass);
                LootLockerConfig.current.deviceID = selectedPlayer.uniqueID;
                DemoAppSession demoAppSession = JsonConvert.DeserializeObject<DemoAppSession>(response.text);
                uiManager.OpenUI(UIScreen.UIScreensType.Home, screenData: demoAppSession);
                LootLockerSDKManager.GetPlayerName((response) =>
                {
                    if (response.success)
                    {
                        if (string.IsNullOrEmpty(response.name))
                        {
                            LootLockerSDKManager.SetPlayerName(selectedPlayer.playerName, (res)=>
                            {
                                LoadingManager.HideLoadingScreen();
                            });
                        }
                        else
                        {
                            LoadingManager.HideLoadingScreen();
                        }
                    }
                    else
                    {
                        LoadingManager.HideLoadingScreen();
                    }
                });
            },
            () =>
             {
                 playersScreen.SetActive(true);
                 Debug.LogError("Log in failure.");
                 LoadingManager.HideLoadingScreen();
             });
        }
    }

}