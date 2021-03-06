using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LootLocker.Example
{
    public class MissionTest : MonoBehaviour
    {
        public string missionId;
        public string signature;
        public string finishTime;
        public string finishScore;
        public List<LootLockerCheckpointTimes> checkpointScores;
        public string labelText;
        Vector2 scrollPosition, scrollPosition2;

        private void OnGUI()
        {

            GUIStyle centeredTextStyle = new GUIStyle();
            centeredTextStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000)))
                UnityEngine.SceneManagement.SceneManager.LoadScene("NavigationScene");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Mission ID");

            missionId = GUILayout.TextField(missionId, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
            missionId = Regex.Replace(missionId, @"[^0-9 ]", "");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Signature");

            signature = GUILayout.TextField(signature, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Finish Time");

            finishTime = GUILayout.TextField(finishTime, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Finish Score");

            finishScore = GUILayout.TextField(finishScore, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
            finishScore = Regex.Replace(finishScore, @"[^0-9 ]", "");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New Checkpoint", GUILayout.ExpandWidth(true)))
                checkpointScores.Add(new LootLockerCheckpointTimes { index = 0, score = 0, time = 0 });

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);

            List<LootLockerCheckpointTimes> checkpointsToDelete = new List<LootLockerCheckpointTimes>();

            for (int i = 0; i < checkpointScores.Count; i++)
            {

                GUILayout.Label("Checkpoint #" + i.ToString());

                GUILayout.BeginHorizontal();

                GUILayout.Label("Index");

                string idx = GUILayout.TextField(checkpointScores[i].index.ToString(), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
                idx = Regex.Replace(idx, @"[^0-9 ]", "");
                checkpointScores[i].index = int.Parse(idx);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label("Score");

                string scr = GUILayout.TextField(checkpointScores[i].score.ToString(), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
                scr = Regex.Replace(scr, @"[^0-9 ]", "");
                checkpointScores[i].score = int.Parse(scr);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label("Time");

                string tm = GUILayout.TextField(checkpointScores[i].time.ToString(), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1000));
                tm = Regex.Replace(tm, @"[^0-9 ]", "");
                checkpointScores[i].time = int.Parse(tm);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Delete Checkpoint #" + i.ToString(), GUILayout.ExpandWidth(true)))
                    checkpointsToDelete.Add(checkpointScores[i]);

                GUILayout.EndHorizontal();

            }

            for (int i = 0; i < checkpointsToDelete.Count; i++)
                checkpointScores.Remove(checkpointsToDelete[i]);

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Get All Missions", GUILayout.ExpandWidth(true)))
            {
                GettingAllEvents();
            }

            if (GUILayout.Button("Get A Single Mission", GUILayout.ExpandWidth(true)))
            {
                GettingASingleEvent();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Start Mission", GUILayout.ExpandWidth(true)))
            {
                StartingEvent();
            }

            if (GUILayout.Button("Finish Mission", GUILayout.ExpandWidth(true)))
            {
                FinishingEvent();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label(labelText);

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        public void GettingAllEvents()
        {
            LootLockerSDKManager.GettingAllMissions((response) =>
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

        public void GettingASingleEvent()
        {
            LootLockerSDKManager.GettingASingleMission(int.Parse(missionId), (response) =>
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

        public void StartingEvent()
        {
            LootLockerSDKManager.StartingAMission(int.Parse(missionId), (response) =>
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

        public void FinishingEvent()
        {
            LootLockerFinishingPayload finishingPayload = new LootLockerFinishingPayload()
            {
                finish_score = finishScore,
                finish_time = finishTime,
                checkpoint_times = checkpointScores.ToArray()
            };
            LootLockerSDKManager.FinishingAMission(int.Parse(missionId), signature, finishTime, finishingPayload, (response) =>
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