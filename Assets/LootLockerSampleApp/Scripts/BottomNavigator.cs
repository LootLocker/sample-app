using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LootLockerDemoApp
{
    public class BottomNavigator : UIScreenView
    {
        public override void Awake()
        {
            base.Awake();
            GetComponent<ScreenCloser>()?.Close();
        }

        public void ViewInventory()
        {
            LoadingManager.ShowLoadingScreen();
            uiManager.OpenUI(UIScreen.UIScreensType.Inventory);
        }

        public void ViewStore()
        {
            LoadingManager.ShowLoadingScreen();
            uiManager.OpenUI(UIScreen.UIScreensType.Store);
        }

        public void ViewCollectables()
        {
            LoadingManager.ShowLoadingScreen();
            uiManager.OpenUI(UIScreen.UIScreensType.Collectables);
        }

        public void ViewHome()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Home);
        }

        public void ViewGameSystem()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.GameSystem);
        }

        public void ViewSettings()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Settings);
        }

        public void ViewMessages()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Messages);
        }

        public void ViewStorage()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Storage);
        }
    }
}