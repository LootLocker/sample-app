using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LootLockerDemoApp
{
    public class Progression : MonoBehaviour
    {
        public Text firstLevel;
        public Text secondLevel;
        public Text xpCalculation;
        public Image xpProgress;
        public Image[] rewards;

        public void UpdateScreen(LootLockerSessionResponse sessionResponse)
        {
            firstLevel.text = sessionResponse.level.ToString();
            secondLevel.text = (sessionResponse.level + 1).ToString();
            float fillAmount = 0;
            if (sessionResponse.level_thresholds != null)
            {
                float numerator = sessionResponse.xp - sessionResponse.level_thresholds.current;
                float denominator = sessionResponse.level_thresholds.next - sessionResponse.level_thresholds.current;
                fillAmount = numerator / denominator;
                xpCalculation.text = sessionResponse.xp + " / " + sessionResponse.level_thresholds.next + " XP";
            }
            xpProgress.fillAmount = fillAmount;
    
        }
    }
}