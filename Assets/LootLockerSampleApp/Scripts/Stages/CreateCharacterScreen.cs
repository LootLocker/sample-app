using LootLocker;
using LootLocker.Requests;
using LootLockerDemoApp;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace LootLockerDemoApp
{
    public class CreateCharacterScreen : UIScreenView
    {
        public Transform parent;
        public Button button;
        public Button editButton;
        Action failResponse;
        public InputPopupCharacter inputPopupCharacter;
        public CharacterPrefabClass[] slots;
        CharacterPrefabClass activeSlot;


        public void UpdateActiveSlot(CharacterPrefabClass activeSlot)
        {
            this.activeSlot = activeSlot;
            if (activeSlot != null)
            {
                editButton.interactable = button.interactable = true;
                editButton?.onClick.RemoveAllListeners();
                editButton?.onClick.AddListener(EditCharacter);
            }
        }

        public void UnselectPrevious()
        {
            editButton.interactable = button.interactable = false;
        }

        public override void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
        {
            base.Open(instantAction, screenData);
            UpdateScreenData(screenData);
        }

        protected override void InternalEasyPrefabSetup()
        {
            base.InternalEasyPrefabSetup();
            ListAllCharacterClasses();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                button.interactable = false;
                ListAllCharacterClasses();
            });
        }

        public void EditCharacter()
        {
            activeSlot?.EditClass("Done");
        }

        public void ListAllCharacterClasses()
        {
            //Loadouts are the default items that are associated with each character class. This is why we can use this to list all character classes and then display to user
            LootLockerSDKManager.ListCharacterTypes((res) =>
            {
                if (res.success)
                {
                    SetUpSlots(res.character_types);
                }
                else
                {
                    failResponse?.Invoke();
                }
            });
        }


        public void SetUpSlots(LootLockerCharacter_Types[] lootLockerCharacter_Types)
        {
            LoadingManager.ShowLoadingScreen();
            LootLockerSDKManager.GetCharacterLoadout((response) =>
            {
                if (response.success)
                {

                    if (response.loadouts.Length > 0)
                    {
                        if (response.loadouts.Length < slots.Length)
                        {
                            List<LootLockerCharacter> lootLockerCharacter = new List<LootLockerCharacter>(new LootLockerCharacter[slots.Length - response.loadouts.Length]);
                            for (int i = 0; i < response.loadouts.Length; i++)
                            {
                                lootLockerCharacter.Add(response.loadouts[i].character);
                            }
                            lootLockerCharacter.Reverse();
                            for (int i = 0; i < lootLockerCharacter.Count; i++)
                            {
                                slots[i].Init(lootLockerCharacter[i], lootLockerCharacter_Types, inputPopupCharacter, false);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < slots.Length; i++)
                            {
                                slots[i].Init(response.loadouts[i].character, lootLockerCharacter_Types, inputPopupCharacter, false);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            slots[i].Init(null, lootLockerCharacter_Types, inputPopupCharacter, false);
                        }
                    }
                }
                //string currentClassName = playerDataObject?.lootLockerCharacter?.name;
                //if (!string.IsNullOrEmpty(currentClassName) && playerDataObject.swappingCharacter)
                //{
                //    playerDataObject.swappingCharacter = false;
                //    CharacterPrefabClass characterPrefabClass = slots.FirstOrDefault(x => x.characterName.text == currentClassName);
                //    if (characterPrefabClass != null)
                //        characterPrefabClass?.EditClass("Edit");
                //}
                LoadingManager.HideLoadingScreen();
            });
        }


        public void UpdateScreenData(ILootLockerScreenData stageData)
        {
            ListAllCharacterClasses();
            button.onClick.RemoveAllListeners();
            if (!dataObject.swappingCharacter)
            {
                button.onClick.AddListener(() =>
                {
                    button.interactable = false;
                    //   OnNextClicked(UIScreen.UIScreensType.Home);
                    LoadingManager.ShowLoadingScreen();
                    LootLockerSDKManager.UpdateCharacter(activeSlot.character.id.ToString(), activeSlot.characterName.text, true, (response) =>
                    {
                        LoadingManager.HideLoadingScreen();
                        if (response.success)
                        {
                            LoadingManager.HideLoadingScreen();
                            OnNextClicked(UIScreen.UIScreensType.Home);
                        }
                        else
                            button.interactable = true;
                    });
                });
                failResponse = () => { uiManager.OpenUI(UIScreen.UIScreensType.Home); button.interactable = true; };
            }
            else
            {
                button.onClick.AddListener(() =>
                {
                    button.interactable = false;
                    LoadingManager.ShowLoadingScreen();                 
                    LootLockerSDKManager.UpdateCharacter(activeSlot.character.id.ToString(), activeSlot.characterName.text, true, (response) =>
                    {
                        LoadingManager.HideLoadingScreen();
                        if (response.success)
                        {
                            LoadingManager.HideLoadingScreen();
                            OnNextClicked(UIScreen.UIScreensType.Settings);
                        }
                        else 
                            button.interactable = true;
                    });
         
                });
                failResponse = () => { uiManager.OpenUI(UIScreen.UIScreensType.Settings); };
            }
        }

        public void OnNextClicked(UIScreen.UIScreensType stageID)
        {
            uiManager.OpenUI(stageID);
        }

        public void UpdateDefaultCharacterClass(Action onCompletedUpdate)
        {

        }

    }
}