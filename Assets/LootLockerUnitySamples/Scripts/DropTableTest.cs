using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTableTest : MonoBehaviour
{
    public int dropTableInstanceId;
    public int[] picks;

    [ContextMenu("Compute Drop")]
    public void ComputeAndLockDropTable()
    {
        LootLockerSDKManager.ComputeAndLockDropTable(dropTableInstanceId, (response)=>
        {
            if(response.success)
            {
                Debug.LogError(response.text);
            }
            else
            {
                Debug.LogError("Failed");
            }
        },true);
    }

    [ContextMenu("Pick Drop")]
    public void PickDropsFromDropTable()
    {
        LootLockerSDKManager.PickDropsFromDropTable(picks, dropTableInstanceId, (response) =>
        {
            if (response.success)
            {
                Debug.LogError(response.text);
            }
            else
            {
                Debug.LogError("Failed");
            }
        });
    }
}
