using UnityEngine;
using System.Linq;

[System.Serializable]
public class UIScreen
{
    public enum UIScreensType
    {
        App, Player, Home, Inventory, Store, GameSystem, Settings, Messages, ReadMessages, SwapClass, CreatePlayer, SelectPlayer, Collectables, Files, Storage, CreateCharacter, Leaderboard, 
        LeaderboardCreationScreen, CreateLeaderboardScreen, LeaderboardsMainDisplay
    }
    public UIScreensType screenFriendlyName;
    public UIScreenView mainScreen;
}

public class UIManagerBase : MonoBehaviour
{
    public UIScreen[] uIScreen;
    public UIScreen.UIScreensType currentlyOpenUI;
    public UIScreen.UIScreensType previouslyOpenUI;

    [SerializeField] UIScreen.UIScreensType defaultScreen;
    [SerializeField] bool openDefault;
    private void Awake()
    {

    }

    private void Start()
    {
        if (openDefault)
        {
            currentlyOpenUI = defaultScreen;
            OpenUI(defaultScreen);
        }
    }

    public void OpenPreviouslyOpenUI()
    {
        OpenUI(previouslyOpenUI);
    }

    public void CloseCurrentView()
    {
        CloseUI(currentlyOpenUI);
    }

    public void OpenUI(UIScreen.UIScreensType screenToOpen, bool hideAllOtherScreens = true, bool hideCurrentlyOpen = true, bool instantAction = false, ILootLockerScreenData screenData = null)
    {
        UIScreen screen = uIScreen.FirstOrDefault(x => x.screenFriendlyName == screenToOpen);
        if (screen != null)
        {
            if (hideAllOtherScreens)
            {
                for (int i = 0; i < uIScreen.Length; i++)
                {
                    if (uIScreen[i] != screen)
                        uIScreen[i]?.mainScreen?.Close();
                }
            }
            if (hideCurrentlyOpen)
                CloseUI(currentlyOpenUI);

            screen?.mainScreen?.Open(instantAction, screenData);
            previouslyOpenUI = currentlyOpenUI;
            currentlyOpenUI = screenToOpen;
        }
    }

    public void CloseUI(UIScreen.UIScreensType screenToOpen, bool instantAction = false)
    {
        UIScreen screen = uIScreen.FirstOrDefault(x => x.screenFriendlyName == screenToOpen);
        screen?.mainScreen?.Close(instantAction);
    }

    public void CloseAllScreens()
    {
        for (int i = 0; i < uIScreen.Length; i++)
            uIScreen[i]?.mainScreen?.Close();
    }

}
