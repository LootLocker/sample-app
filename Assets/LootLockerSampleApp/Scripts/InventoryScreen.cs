﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using LootLocker;
using System.Linq;
using Newtonsoft.Json;

namespace LootLockerDemoApp
{
    public class InventoryScreen : UIScreenView
    {
        [Header("Inventory")]
        public Transform inventoryParent;
        public Text normalText, headerText;
        public ToggleGroup inventoryToggles;
        InventoryAssetPair currentAsset;
        public Button selectBtn;
        public Text selectText;
        public GameObject contextsPrefab;
        List<ContextController> contextControllers = new List<ContextController>();
        public Text className;

        public override void Awake()
        {
            base.Awake();
            currentAsset = new InventoryAssetPair();
            selectBtn.onClick.AddListener(ButtonClicked);
        }

        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            ViewInventory();
        }

        protected override void InternalEasyPrefabSetup()
        {
            base.InternalEasyPrefabSetup();
            ViewInventory();
        }

        public void ViewInventory()
        {
            this.normalText.text = "";
            this.normalText.text += "Asset Name" + " : " + "" + "\n";
            this.normalText.text += "Asset Context" + " : " + "" + "\n";

            LootLockerSDKManager.GetCurrentLoadOutToDefaultCharacter((response) =>
            {
                if (response.success)
                {
                    foreach (Transform tr in inventoryParent)
                        Destroy(tr.gameObject);

                    contextControllers.Clear();
                    LootLockerSDKManager.GetEquipableContextToDefaultCharacter((contextResponse) =>
                   {
                       if (contextResponse.success)
                       {
                           string[] contexts = contextResponse.contexts.Select(x => x.name).ToArray();
                           LootLockerCommonAsset[] assets = response.GetAssets();
                           for (int i = 0; i < contexts.Length; i++)
                           {
                               GameObject contextObject = Instantiate(contextsPrefab, inventoryParent);
                               ContextController contextController = contextObject.GetComponent<ContextController>();
                               contextController.Init(contexts[i], assets.FirstOrDefault(x => x.context == contexts[i])?.id.ToString());
                               contextControllers.Add(contextController);
                           }

                           this.headerText.text = "Inventory";
                           selectText.text = "Equip";
                           this.normalText.text = "";
                           this.normalText.text += "Asset Name" + " : " + "" + "\n";
                           this.normalText.text += "Asset Context" + " : " + "" + "\n";
                           LootLockerSDKManager.GetInventory((res) =>
                           {
                               LoadingManager.HideLoadingScreen();
                               if (res.success)
                               {
                                   InventoryAssetResponse.InventoryResponse mainResponse = JsonConvert.DeserializeObject<InventoryAssetResponse.InventoryResponse>(res.text);
                                   Debug.Log("Successful got inventory: " + res.text);
                                   for (int j = 0; j < contextControllers.Count; j++)
                                   {
                                       InventoryAssetResponse.Inventory[] inventories = mainResponse.inventory.Where(x => x.asset?.context == contextControllers[j].context)?.ToArray();

                                       Dictionary<int, int> numberOfInventories = new Dictionary<int, int>();//It will contain amount of each inventory we have
                                   for (int i = 0; i < inventories.Length; i++)
                                       {
                                           if (numberOfInventories.ContainsKey(inventories[i].asset.id))//Increase amount of inventory if we already have it in our dictionary
                                       {
                                               numberOfInventories[inventories[i].asset.id]++;
                                           }
                                           else
                                           {
                                               numberOfInventories.Add(inventories[i].asset.id, 1);
                                           }
                                       }

                                       inventories = inventories.GroupBy(x => x.asset.id).Select(x => x.First()).ToArray();//Grouping inventories, so we won't have duplicates
                                   contextControllers[j].Populate(inventories, numberOfInventories);
                                   }
                               }
                               else
                               {
                                   Debug.LogError("failed to get all inventory items: " + res.Error);
                               }
                           });
                       }
                       else
                       {
                           Debug.LogError("failed to get all inventory items: " + contextResponse.Error);
                       }
                   });
                }
            });

        }

        public void SelectItem(InventoryAssetPair inventoryAssetPair)
        {
            currentAsset = inventoryAssetPair;
            this.normalText.text = "";
            this.normalText.text += "Asset Name" + " : " + currentAsset.asset.name + "\n";
            this.normalText.text += "Asset Context" + " : " + currentAsset.asset.context + "\n";
        }

        public void ButtonClicked()
        {
            if (currentAsset.asset != null)
            {
                if (currentAsset.asset.files != null && currentAsset.asset.files.Length > 0)
                {
                    uiManager.OpenUI(UIScreen.UIScreensType.Files, screenData: currentAsset.asset);
                    return;
                }
            }

            string header = "Equip";
            string btnText = "Equip";
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Asset Name", currentAsset.asset.name);
            data.Add("Asset Context", currentAsset.asset.context);

            PopupSystem.ShowPopup(header, data, btnText, () =>
            {
                LoadingManager.ShowLoadingScreen();
                LootLockerSDKManager.EquipIdAssetToDefaultCharacter(currentAsset.inventory.instance_id.ToString(), (response) =>
                {
                    LoadingManager.HideLoadingScreen();
                    if (response.success)
                    {
                        header = "Success";
                        data.Clear();
                    //Preparing data to display or error messages we have
                    data.Add("1", "You successfully equipped: " + currentAsset.inventory.asset.name);
                        LoadingManager.ShowLoadingScreen();
                        uiManager.OpenUI(UIScreen.UIScreensType.Inventory);
                        PopupSystem.ShowApprovalFailPopUp(header, data, currentAsset.asset.url, false);
                    }
                    else
                    {
                        Debug.LogError(response.Error);
                        header = "Failed";
                        data.Clear();

                    //Preparing data to display or error messages we have
                    string correctedResponse = response.Error.First() == '{' ? response.Error : response.Error.Substring(response.Error.IndexOf('{'));
                        ResponseError equipErrorResponse = new ResponseError();
                        equipErrorResponse = JsonConvert.DeserializeObject<ResponseError>(correctedResponse);

                        data.Add("1", equipErrorResponse.messages[0]);
                        PopupSystem.ShowApprovalFailPopUp(header, data, currentAsset.asset.url, true);
                    }

                });
            }, currentAsset.asset.url);
        }


        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            ViewInventory();
        }

    }
}