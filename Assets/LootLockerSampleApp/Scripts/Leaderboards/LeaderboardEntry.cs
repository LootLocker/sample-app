using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : ListPrefab
{
    [SerializeField] Text rankTxt;
    [SerializeField] Text nameTxt;
    [SerializeField] Text scoreTxt;

    public override void Init(IListData listData)
    {
        base.Init(listData);
        LootLockerLeaderboardMember lootLockerMember = listData as LootLockerLeaderboardMember;
        if (lootLockerMember != null)
        {
            rankTxt.text = lootLockerMember.rank.ToString();
            nameTxt.text = lootLockerMember.player != null && !string.IsNullOrEmpty(lootLockerMember.player.name) ? lootLockerMember.player.name : lootLockerMember.member_id.ToString();
            scoreTxt.text = lootLockerMember.score.ToString();
        }
    }
}
