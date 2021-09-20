using LootLocker;
using LootLockerDemoApp;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UIScreen;

public interface ILootLockerScreenData
{

}

public class OnScreenDataAvailable : UnityEvent<UIScreensType, ILootLockerScreenData> { };
public class UIScreenView : MonoBehaviour
{
    public static OnScreenDataAvailable onScreenDataAvailable = new OnScreenDataAvailable();
    public Button backButton;
    [HideInInspector]
    public UIManagerBase uiManager;
    [SerializeField] protected UIScreensType screenType;
    ScreenOpener screenOpener;
    ScreenCloser screenCloser;
    protected PlayerDataObject dataObject;
    [SerializeField]
    protected ScreenOpener bottomOpener;
    public static LocalPlayer localPlayer;
    [Header("Easy Prefab Setup")]
    public bool isEasyPrefab;

    public virtual void Awake()
    {
        uiManager = GetComponentInParent<UIManagerBase>();
        backButton?.onClick?.AddListener(BackButtonPressed);
        screenOpener = GetComponent<ScreenOpener>();
        screenCloser = GetComponent<ScreenCloser>();
        StartEasyPrefab();
        dataObject = Resources.Load<PlayerDataObject>("PlayerData");
        dataObject.swappingCharacter = false;
    }

    public void SetUpEasyPrefab()
    {
        if (TexturesSaver.Instance == null)
        {
            GameObject saver = Resources.Load("EasyPrefabsResources/TextureSaver") as GameObject;
            Instantiate(saver);
        }

        if (LoadingManager.Instance == null)
        {
            GameObject loading = Resources.Load("EasyPrefabsResources/LoadingPrefab") as GameObject;
            Instantiate(loading);
        }

        if (PopupSystem.Instance == null)
        {
            GameObject popup = Resources.Load("EasyPrefabsResources/PopupPrefab") as GameObject;
            Instantiate(popup);
        }
    }

    public void StartEasyPrefab()
    {
        if (isEasyPrefab)
        {
            SetUpEasyPrefab();
            InternalEasyPrefabSetup();
        }
    }

    protected virtual void InternalEasyPrefabSetup()
    {
       
    }

    public virtual void OnEnable()
    {
        onScreenDataAvailable.AddListener(ScreenDataAvailable);
    }

    public virtual void OnDisable()
    {
        onScreenDataAvailable.RemoveListener(ScreenDataAvailable);
    }

    public virtual void ScreenDataAvailable(UIScreensType type, ILootLockerScreenData data)
    {
        if (type != screenType) return;
    }

    // Start is called before the first frame update
    public virtual void Open(bool instantAction = false, ILootLockerScreenData screenData = null)
    {
        screenOpener.Open();
    }

    // Start is called before the first frame update
    public virtual void Close(bool instantAction = false)
    {
        screenCloser.Close();
    }

    public virtual void BackButtonPressed()
    {
        CloseCurrentView();
    }

    public void CloseCurrentView()
    {
        uiManager?.CloseCurrentView();
    }

    public void BackButtonPressed(GameObject go)
    {
        BackButtonPressed();
    }


}
