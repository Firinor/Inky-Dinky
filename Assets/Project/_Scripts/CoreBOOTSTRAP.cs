using UnityEngine;

public class CoreBOOTSTRAP : MonoBehaviour
{
    public MainImageManager MainImageManager;
    public ColorJarsManager ColorJarsManager;
    public WorkerManager WorkerManager;

    private SaveData player;
    
    void Start()
    {
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