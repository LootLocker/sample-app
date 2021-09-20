using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using LootLocker;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace LootLocker.Example
{
    public class LeaderboardTest : MonoBehaviour
    {
        public string labelText;
        Vector2 scrollPosition;
        [SerializeField] string leaderboardId = "0";
        [SerializeField] string memberid = "0";

        private void OnGUI()
        {

            GUIStyle centeredTextStyle = new GUIStyle();
            centeredTextStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();

            //GUILayout.BeginHorizontal();

            //if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000)))
            //    UnityEngine.SceneManagement.SceneManager.LoadScene("NavigationScene");

            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Leaderboard ID");

            leaderboardId = GUILayout.TextField(leaderboardId, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
            leaderboardId = Regex.Replace(leaderboardId, @"[^0-9 ]", "");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Member ID");

            memberid = GUILayout.TextField(memberid, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
            memberid = Regex.Replace(memberid, @"[^0-9 ]", "");

            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();

            //GUILayout.Label("Asset Count To Download:");

            //assetCountToDownload = GUILayout.TextField(assetCountToDownload, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
            //assetCountToDownload = Regex.Replace(assetCountToDownload, @"[^0-9 ]", "");

            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Get Member Rank", GUILayout.ExpandWidth(true)))
            {
                GetMemberRank();
            }

            if (GUILayout.Button("Get By List Of Members", GUILayout.ExpandWidth(true)))
            {
                GetByListOfMembers();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Get All Member Ranks", GUILayout.ExpandWidth(true)))
            {
                GetAllMemberRanks();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Get Score List", GUILayout.ExpandWidth(true)))
            {
                GetScoreList();
            }

            if (GUILayout.Button("Submit Score", GUILayout.ExpandWidth(true)))
            {
                SubmitScore();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label(labelText);

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }




        [ContextMenu("GetMemberRank")]
        public void GetMemberRank()
        {
            LootLockerSDKManager.GetMemberRank(leaderboardId.ToString(), int.Parse(memberid), (response) =>
             {
                 if (response.success)
                 {
                     labelText = "Success\n" + response.text;
                 }
                 else
                 {
                     labelText = "Failed\n" + response.text;
                 }

             });
        }

        [SerializeField] string[] membersList;

        [ContextMenu("GetByListOfMembers")]
        public void GetByListOfMembers()
        {
            LootLockerSDKManager.GetByListOfMembers(membersList, int.Parse(leaderboardId), (response) =>
               {
                   //GetAssetsOriginal
                   if (response.success)
                   {
                       labelText = "Success\n" + response.text;
                   }
                   else
                   {
                       labelText = "Failed\n" + response.text;
                   }

               });
        }
        [SerializeField] int count;
        [SerializeField] int after;

        [ContextMenu("GetAllMemberRanks")]
        public void GetAllMemberRanks()
        {
            int member_id = int.Parse(memberid);

            LootLockerSDKManager.GetAllMemberRanks(member_id, 20, (response) =>
            {
                if (response.success)
                {
                    labelText = "Success\n" + response.text;
                }
                else
                {
                    Debug.LogError($"Failed with error: {response.Error}");
                }
            });
        }

        [ContextMenu("GetScoreList")]
        public void GetScoreList()
        {
            int leaderboardId = 3;

            LootLockerSDKManager.GetScoreList(leaderboardId, 20, (response) =>
             {
                 if (response.success)
                 {
                     LootLockerLeaderboardMember[] members = response.items;
                     for (int i = 0; i < members.Length; i++)
                     {
                         Debug.LogError($"Rank{members[i].rank};Name:{members[i].player.name};User Id: {members[i].player.id};Member Id:{members[i].member_id}; ");
                     }
                 }
                 else
                 {
                     Debug.LogError($"Failed with error: {response.Error}");
                 }
             });
        }
        [SerializeField] int score;

        [ContextMenu("Submit Score")]
        public void SubmitScore()
        {
            LootLockerSDKManager.SubmitScore(memberid.ToString(), score, int.Parse(leaderboardId), (response) =>
             {
                 if (response.success)
                 {
                     labelText = "Success\n" + response.text;
                 }
                 else
                 {
                     labelText = "Failed\n" + response.text;
                 }
             });
        }

    }
}