using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker;

namespace LootLockerDemoApp
{
    public class StorageScreen : UIScreenView
    {
        public GameObject keyValueElement;
        public GameObject content;
        public Button addKey;
        public InputPopup inputPopup;
        public Button refresh;


        public override void Awake()
        {
            base.Awake();
            addKey.onClick.AddListener(OpenKeysWindow);
            refresh.onClick.AddListener(Refresh);
        }

        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            UpdateScreenData(screenData);
        }

        protected override void InternalEasyPrefabSetup()
        {
            base.InternalEasyPrefabSetup();
            backButton?.gameObject.SetActive(false);
            SetUpEasyPrefab();
            Refresh();
        }

        public void Refresh()
        {
            LoadingManager.ShowLoadingScreen();
            foreach (Transform tr in content.transform)
            {
                Destroy(tr.gameObject);
            }
            LootLockerSDKManager.GetEntirePersistentStorage((response) =>
            {
                LoadingManager.HideLoadingScreen();
                UpdateScreen(response.payload);
            });
        }

        public void UpdateScreen(LootLockerPayload[] payload)
        {
            foreach (Transform tr in content.transform)
            {
                Destroy(tr.gameObject);
            }
            for (int i = 0; i < payload.Length; i++)
            {
                GameObject go = Instantiate(keyValueElement, content.transform);
                KeyValueElements keyValueElements = go?.GetComponent<KeyValueElements>();
                if (keyValueElements != null)
                {
                    keyValueElements.Init(payload[i]);
                }
            }
        }

        public void OpenKeysWindow()
        {
            inputPopup.Init(new string[] { "Save" });
        }

        public void OpenKeyWindow(string key, string value, string[] btns)
        {
            inputPopup.Init(key, value, btns);
        }

        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            LootLockerGetPersistentStoragResponse response = stageData as LootLockerGetPersistentStoragResponse;
            if (response != null)
            {
                UpdateScreen(response.payload);
                LoadingManager.HideLoadingScreen();
            }
            else
                Refresh();
        }

    }
}
