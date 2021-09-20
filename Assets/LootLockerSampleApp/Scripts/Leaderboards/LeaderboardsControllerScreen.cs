using LootLocker;
using LootLocker.Requests;
using LootLockerDemoApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class LeaderboardsControllerScreen : UIScreenView
{
    [SerializeField] UIManagerBase localUiManager;

    protected override void InternalEasyPrefabSetup()
    {
        base.InternalEasyPrefabSetup();
    }

    public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
    {
        base.Open(instantAction, screenData);
        UpdateScreenData(null);
    }

    public void UpdateScreenData(ILootLockerScreenData stageData)
    {
        localUiManager.OpenUI(UIScreen.UIScreensType.LeaderboardCreationScreen);
        bottomOpener.gameObject.SetActive(false);
    }
}
