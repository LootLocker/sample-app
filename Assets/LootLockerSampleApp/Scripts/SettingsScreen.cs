using LootLocker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LootLockerDemoApp
{
    public class ClassData: ILootLockerScreenData
    {
        public string classType;
    }
    public class SettingsScreen : UIScreenView
    {
        public ScreenCloser bottomNavigator;
        public ButtonExtention buttonExtentionEvent;
        public Button editDefaultCharacter;
        public Button changeCharacter;

        public override void Awake()
        {
            base.Awake();
            editDefaultCharacter.onClick.AddListener(EditdefultCharacter);
            changeCharacter.onClick.AddListener(ChangeCharacter);
        }

        public void RefreshGameData()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Home);
            ButtonExtention.buttonClicked?.Invoke(buttonExtentionEvent);
        }

        public void ChangePlayer()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.Player);
            ButtonExtention.buttonClicked?.Invoke(buttonExtentionEvent);
            bottomNavigator?.Close();

        }

        public void Logout()
        {
            uiManager.OpenUI(UIScreen.UIScreensType.App);
            ButtonExtention.buttonClicked?.Invoke(buttonExtentionEvent);
            bottomNavigator?.Close();
        }

        public void EditdefultCharacter()
        {
            dataObject.swappingCharacter = true;
            uiManager.OpenUI(UIScreen.UIScreensType.CreateCharacter);
        }

        public void ChangeCharacter()
        {
            dataObject.swappingCharacter = true;
            uiManager.OpenUI(UIScreen.UIScreensType.CreateCharacter);
        }
    }
}