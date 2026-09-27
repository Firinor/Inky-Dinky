using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class BOOTSTRAP : MonoBehaviour
{
    public MainImageManager MainImageManager;
    public ColorJarsManager ColorJarsManager;
    public WorkerManager WorkerManager;

    private SaveData player;
    
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
#if IS_YANDEX
        yield return YG2.onGetSDKData;
#endif
        
        LoadPlayerData();
        //settings.Initialize(bootstrap: true);
        
        MainImageManager.Initialize(player);
        ColorJarsManager.Initialize();
        WorkerManager.Initialize(player);
    }
    
    private void LoadPlayerData()
    {
        player = SaveData.GetPlayer();
        player.FirstLoad();
    }
}