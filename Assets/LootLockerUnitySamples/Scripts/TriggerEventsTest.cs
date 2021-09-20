using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LootLocker.Example
{
    public class TriggerEventsTest : MonoBehaviour
    {
        public string eventName;


        [ContextMenu("Trigger Event")]
        public void TriggerEvent()
        {

            LootLockerSDKManager.TriggeringAnEvent(eventName, (response) =>
             {
                 if (response.success)
                 {
                     LootLockerSDKManager.DebugMessage("Successful");
                 }
                 else
                 {
                     LootLockerSDKManager.DebugMessage("failed: " + response.Error, true);
                 }
             });
        }

        [ContextMenu("Listing Triggered Events")] 
        public void ListTriggeredEvents()
        {

            LootLockerSDKManager.ListingTriggeredTriggerEvents((response) =>
            {
                if (response.success)
                {
                    LootLockerSDKManager.DebugMessage("Successful");
                }
                else
                {
                    LootLockerSDKManager.DebugMessage("failed: " + response.Error, true);
                }
            });
        }
    }
}