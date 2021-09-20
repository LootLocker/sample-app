using LootLocker.Requests;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker;

namespace LootLockerDemoApp
{
    public class CreateLeaderboard : UIScreenView
    {
        public InputField leaderboardName;
        public Dropdown direction;
        public Dropdown type;
        public Toggle overWriteScore;
        public Button create;
        [SerializeField] LeaderboardCreationScreen leaderboardCreation;

        public override void Awake()
        {
            base.Awake();
            create.onClick.AddListener(CreateBoard);
        }

        public override void BackButtonPressed()
        {
            base.BackButtonPressed();
            uiManager?.OpenUI(UIScreen.UIScreensType.LeaderboardCreationScreen);
        }

        void CreateBoard()
        {
            LoadingManager.ShowLoadingScreen();
            LootLockerCreateLeaderboardRequest lootLockerCreateLeaderboardRequest = new LootLockerCreateLeaderboardRequest();
            lootLockerCreateLeaderboardRequest.direction_method = direction.options[direction.value].text.ToLower();
            lootLockerCreateLeaderboardRequest.enable_game_api_writes = true;
            lootLockerCreateLeaderboardRequest.overwrite_score_on_submit = overWriteScore.isOn;
            lootLockerCreateLeaderboardRequest.type = type.options[type.value].text.ToLower();
            lootLockerCreateLeaderboardRequest.name = leaderboardName.text;

            DemoAppAdminRequests.CreateLeaderboard(LootLockerConfig.current.gameID, lootLockerCreateLeaderboardRequest, (response) =>
            {
                LoadingManager.HideLoadingScreen();

                if (response.success)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("1", "You successfully Created a leaderboard");
                    PopupSystem.ShowApprovalFailPopUp("Created", data, "keySucces", false, onComplete: () =>
                    {
                        BackButtonPressed();
                    });
                }
                else
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("1", response.Error);
                    PopupSystem.ShowApprovalFailPopUp("Failed", data, "keySucces", false, onComplete: () =>
                    {
                        //BackButtonPressed();
                    });
                }
            });
        }

        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            leaderboardName.text = "";
            direction.value = 0;
            type.value = 0;
            overWriteScore.isOn = true;
            gameObject.SetActive(true);
        }
    }
}