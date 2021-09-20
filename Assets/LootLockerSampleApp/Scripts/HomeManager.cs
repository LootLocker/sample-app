using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using LootLocker;
using Newtonsoft.Json;

namespace LootLockerDemoApp
{
    public class HomeManager : UIScreenView
    {
     
        protected override void InternalEasyPrefabSetup()
        {
            CreateNewSession();
        }

        public void CreateNewSession()
        { 

            LoadingManager.ShowLoadingScreen();
            string defaultUser =string.IsNullOrEmpty(LootLockerConfig.current.deviceID) ? LootLockerConfig.current.deviceID : "NewUserDefault";

            //Starting a new session using the new id that has been created
            LootLockerSDKManager.StartSession(defaultUser, (response) =>
            {
                if (response.success)
                {
                    dataObject.SaveSession(response);
                    DemoAppSession demoAppSession = JsonConvert.DeserializeObject<DemoAppSession>(response.text);
                    UpdateScreenData(demoAppSession);
                    Debug.Log("Created Session for new player with id: " + defaultUser);
                }
                else
                {
                    Debug.LogError("Session failure: " + response.text);
                }
            });
        }


        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            UpdateScreenData(screenData);
        }

        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            GetComponentInChildren<PlayerProfile>()?.UpdateScreen(dataObject?.session);
            GetComponentInChildren<Progression>()?.UpdateScreen(dataObject?.session);
            bottomOpener?.Open();
        }

    }
}