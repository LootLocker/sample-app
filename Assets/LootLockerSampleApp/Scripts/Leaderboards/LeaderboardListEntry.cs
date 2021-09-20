using LootLocker.Requests;
using LootLockerDemoApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardListEntry : ListPrefab
{
    [SerializeField] Text idTxt;
    [SerializeField] Text nameTxt;
    [SerializeField] Text typeTxt;
    [SerializeField] Button button;

    public override void Init(IListData listData)
    {
        base.Init(listData);
        LootLockerListLeaderboardsItem lootLockerMember = listData as LootLockerListLeaderboardsItem;
        idTxt.text = lootLockerMember.id.ToString();
        nameTxt.text = lootLockerMember.name.ToString();
        typeTxt.text = lootLockerMember.type.ToString();
        button.onClick.AddListener(() =>
        {
            GetComponentInParent<UIManagerBase>()?.OpenUI(UIScreen.UIScreensType.LeaderboardsMainDisplay, screenData: lootLockerMember);
        });
    }
}
