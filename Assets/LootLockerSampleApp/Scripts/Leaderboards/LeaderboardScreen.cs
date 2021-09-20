using LootLocker;
using LootLocker.Requests;
using LootLockerDemoApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;

public class LootLockerMemberLocal : LootLockerLeaderboardMember, IListData
{
    public int index { get; set; }
    public ListPopulator listParent { get; set; }
}

public class LeaderboardScreen : UIScreenView
{
    public Transform parent;
    public GameObject prefab;
    int leaderboardId;
    [SerializeField]
    int count;
    [SerializeField] GameObject bottom;
    [SerializeField] Button back;
    [SerializeField] Button next;
    [SerializeField] Button prev;
    [SerializeField] InputField score;
    [SerializeField] Button addScore;
    ListPopulator listPopulator;


    public override void Awake()
    {
        base.Awake();
        next.onClick.AddListener(Next);
        prev.onClick.AddListener(Prev);
        next.interactable = false;
        prev.interactable = false;
        addScore.onClick.AddListener(AddScoreToLeaderboard);
        listPopulator = GetComponent<ListPopulator>();
    }

    public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
    {
        base.Open(instantAction, screenData);
        score.text = Random.Range(20, 13233).ToString();
        LootLockerListLeaderboardsItem data = screenData as LootLockerListLeaderboardsItem;
        if (data != null)
            leaderboardId = data.id;
        Populate();
    }

    protected override void InternalEasyPrefabSetup()
    {
        base.InternalEasyPrefabSetup();
        Populate();
    }

    public override void BackButtonPressed()
    {
        base.BackButtonPressed();
        listPopulator.ClearParent();
        uiManager?.OpenUI(UIScreen.UIScreensType.LeaderboardCreationScreen);
    }

    public void Populate()
    {
        LootLockerSDKManager.GetScoreList(leaderboardId, count, (response) =>
        {
            if (response.success)
            {
                LootLockerMemberLocal[] members = GetLocalTypeMembers(response.items);
                listPopulator.Populate(members);
                prev.interactable = response.pagination.allowPrev;
                next.interactable = response.pagination.allowNext;
                LoadingManager.HideLoadingScreen();
                bottom.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"Failed with error: {response.Error}");
            }
        });
    }

    public LootLockerMemberLocal[] GetLocalTypeMembers(LootLockerLeaderboardMember[] mem)
    {
        return JsonConvert.DeserializeObject<LootLockerMemberLocal[]>(JsonConvert.SerializeObject(mem));
    }

    public void Next()
    {
        LoadingManager.ShowLoadingScreen();
        LootLockerSDKManager.GetNextScoreList(leaderboardId, count, (response) =>
        {
            LoadingManager.HideLoadingScreen();
            if (response.success)
            {
                if (response.items.Length > 0)
                {
                    LootLockerMemberLocal[] members = GetLocalTypeMembers(response.items);
                    listPopulator.Populate(members);
                    LoadingManager.HideLoadingScreen();
                    bottom.gameObject.SetActive(false);
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

    public void Prev()
    {
        LoadingManager.ShowLoadingScreen();
        LootLockerSDKManager.GetPrevScoreList(leaderboardId, count, (response) =>
        {
            LoadingManager.HideLoadingScreen();
            if (response.success)
            {
                if (response.items.Length > 0)
                {
                    LootLockerMemberLocal[] members = GetLocalTypeMembers(response.items);
                    listPopulator.Populate(members);
                    LoadingManager.HideLoadingScreen();
                    bottom.gameObject.SetActive(false);
                    prev.interactable = response.pagination.allowPrev;
                    next.interactable = response.pagination.allowNext;

                }
                else
                {
                    prev.interactable = false;
                }
            }
            else
            {
                Debug.LogError($"Failed with error: {response.Error}");
            }
        });
    }

    public void AddScoreToLeaderboard()
    {
        LoadingManager.ShowLoadingScreen();
        LootLockerSDKManager.SubmitScore(dataObject.session.player_id.ToString(), int.Parse(score.text), leaderboardId, (response) =>
        {
            LoadingManager.HideLoadingScreen();
            if (response.success)
            {
                Populate();
                Debug.Log("Added Score to leaderboard");
            }
        });
    }
}
