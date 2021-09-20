using LootLocker;
using LootLocker.Requests;
using LootLockerDemoApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class LeaderboardCreationScreen : UIScreenView
{
    public Transform parent;
    public GameObject prefab;
    [SerializeField] Button back;
    int count = 10;
    [SerializeField] Button next;
    [SerializeField] Button prev;
    [SerializeField] Button createLeaderboard;
    [SerializeField] CreateLeaderboard createLeaderboardScreen;
    [SerializeField] LeaderboardScreen leaderboardScreen;
    [SerializeField] UIManagerBase parentUIManager;
    ListPopulator listPopulator;

    public override void Awake()
    {
        base.Awake();
        next.onClick.AddListener(Next);
        prev.onClick.AddListener(Prev);
        createLeaderboard.onClick.AddListener(CreateLeaderboard);
        listPopulator = GetComponent<ListPopulator>();
    }

    protected override void InternalEasyPrefabSetup()
    {
        base.InternalEasyPrefabSetup();
        Populate();
    }

    public override void BackButtonPressed()
    {
        base.BackButtonPressed();
        parentUIManager?.OpenUI(UIScreen.UIScreensType.GameSystem);
        bottomOpener.gameObject.SetActive(true);
    }

    void ClearParent()
    {
        foreach (Transform tr in parent)
            Destroy(tr.gameObject);
    }

    public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
    {
        base.Open(instantAction, screenData);
        Populate();
    }

    public void Populate()
    {
        LootLockerListLeaderboardRequest getValues = new LootLockerListLeaderboardRequest();
        getValues.gameId = LootLockerConfig.current.gameID;
        getValues.count = count;
        List(getValues);
    }

    public void Next()
    {
        LoadingManager.ShowLoadingScreen();
        LootLockerListLeaderboardRequest getValues = new LootLockerListLeaderboardRequest();
        getValues.gameId = LootLockerConfig.current.gameID;
        getValues.count = count;
        getValues.after = LootLockerListLeaderboardRequest.nextCursor.ToString();
        List(getValues);
    }

    public void Prev()
    {
        LoadingManager.ShowLoadingScreen();
        LootLockerListLeaderboardRequest getValues = new LootLockerListLeaderboardRequest();
        getValues.gameId = LootLockerConfig.current.gameID;
        getValues.count = count;
        getValues.after = LootLockerListLeaderboardRequest.prevCursor.ToString();
        List(getValues);
    }

    public void List(LootLockerListLeaderboardRequest getValues)
    {
        DemoSDKManager.ListLeaderboards(getValues, (response) =>
        {
            LoadingManager.HideLoadingScreen();
            if (response.success)
            {
                ClearParent();
                if (response.items.Length > 0)
                {
                    listPopulator?.Populate(response.items);
                    LoadingManager.HideLoadingScreen();
                    bottomOpener.gameObject.SetActive(false);
                    prev.interactable = response.pagination.allowPrev;
                    next.interactable = response.pagination.allowNext;
                }
                else
                {
                    next.interactable = false;
                }
            }
            else
            {
                Debug.LogError($"Failed with error: {response.Error}");
            }
        });
    }

    public void CreateLeaderboard()
    {
        uiManager?.OpenUI(UIScreen.UIScreensType.CreateLeaderboardScreen, hideAllOtherScreens: false);
    }


}
