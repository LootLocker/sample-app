using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker;
using UnityEngine.UI;

namespace LootLockerDemoApp
{
    public class GameSystemScreen : UIScreenView
    {
        [SerializeField] Button leaderboardBtn;

        public override void Awake()
        {
            base.Awake();
            leaderboardBtn.onClick.AddListener(OpenLeaderboard);
        }

        void ListMessages()
        {

        }
        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);

        }

        public void UpdateScreenData(ILootLockerScreenData stageData)
        {

        }

        public void OpenLeaderboard()
        {
            LoadingManager.ShowLoadingScreen();
            uiManager.OpenUI(UIScreen.UIScreensType.Leaderboard);
        }
    }
}