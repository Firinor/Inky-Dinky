using System.Collections;
using FirAnimations;
using UnityEngine;

public class CoreBOOTSTRAP : MonoBehaviour
{
    public MainImageManager MainImageManager;
    public ColorJarsManager ColorJarsManager;
    public WorkerManager WorkerManager;

    [SerializeField] 
    private Settings settings;
    
    public FirAnimation closeСurtain;
    
    private SaveData player;
    
    IEnumerator Start()
    {
        closeСurtain.Initialize();
    
        yield return null;
    
        closeСurtain.Play();//OpenScene
            
        LoadPlayerData();
        settings.Initialize(bootstrap: true);
        
        MainImageManager.Initialize(player);
        ColorJarsManager.Initialize();
        WorkerManager.Initialize(player);
    }
    
    private void LoadPlayerData()
    {
        player = SaveData.GetPlayer();
        player.FirstLoad();
    }

    [ContextMenu(nameof(ClearAllSaves))]
    public void ClearAllSaves()
    {
        player.ResetProgress();
    }
}