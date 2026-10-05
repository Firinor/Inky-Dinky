using System.Collections;
using FirAnimations;
using UnityEngine;
#if IS_YANDEX
using YG;
#endif

public class MetaBOOTSTRAP : MonoBehaviour
{
    public FirAnimation closeСurtain;  
    
    [SerializeField] 
    private Settings settings;
    [SerializeField] 
    private SityManager sityManager;
    
    private SaveData player;
    
    IEnumerator Start()
    {
        closeСurtain.Initialize();
        
        yield return null;
        
        closeСurtain.Play();//OpenScene
        
        settings.Initialize();
        
        LoadPlayerData();

        sityManager.Initialize(player);
        
#if IS_YANDEX
        YG2.GameReadyAPI();
#endif
    }
    
    private void LoadPlayerData()
    {
        player = SaveData.GetPlayer();
        player.FirstLoad();
    }
}