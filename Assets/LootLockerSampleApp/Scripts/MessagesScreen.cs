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
    public class MessagesScreen : UIScreenView
    {
        [Header("Messages")]
        public Transform messagesParent;
        public GameObject messagesObject, readMessageObject, messagePrefab;
        [Header("Easy Prefab Setup")]
        public GameObject readMessages;

        protected override void InternalEasyPrefabSetup()
        {
            base.InternalEasyPrefabSetup();
            backButton?.gameObject.SetActive(false);
            ListMessages();
        }

        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            ViewMessages();
        }

        public override void BackButtonPressed()
        {
            base.BackButtonPressed();
            uiManager?.OpenUI(UIScreen.UIScreensType.Home);
        }

        void ListMessages()
        {
            LoadingManager.ShowLoadingScreen();
            LootLockerSDKManager.GetMessages((response) =>
            {
                LoadingManager.HideLoadingScreen();
                if (response.success)
                {
                    Debug.Log("Successful got all messages: " + response.text);
                    for (int i = 0; i < messagesParent.childCount; i++)
                        Destroy(messagesParent.GetChild(i).gameObject);
                    foreach (LootLockerGMMessage message in response.messages)
                    {
                        GameObject messageObject = Instantiate(messagePrefab, messagesParent);
                        messageObject.GetComponent<MessageElement>().InitMessage(message);
                        messageObject.GetComponent<Button>().onClick.AddListener(() => SelectMessage(message));
                    }
                }
                else
                {
                    Debug.LogError("failed to get all messages: " + response.Error);
                }
            });
        }
        public void ViewMessages()
        {
            ListMessages();
        }
        public void SelectMessage(LootLockerGMMessage selectedMessage)
        {
            if (selectedMessage == null) return;

            string json = JsonConvert.SerializeObject(selectedMessage);
            DemoMessageResponse response = JsonConvert.DeserializeObject<DemoMessageResponse>(json);
            if (!readMessages)
            {
                uiManager.OpenUI(UIScreen.UIScreensType.ReadMessages, screenData: response);
            }
            else
                readMessages?.GetComponent<ReadMessageScreen>()?.StartEasyPrefab(response);
        }

        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            ViewMessages();
        }
    }
}