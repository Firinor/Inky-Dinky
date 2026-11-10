using System.Collections.Generic;
using FirAnimations;
using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class WorkerPlace : MonoBehaviour
{
    public float Cooldown = 1f;
    public ColorBar bar;
    public Dictionary<PointyPlace, float> NearestPlaces;

    public float cooldown;
    public int isWarningNumber = int.MaxValue;
    public int UnlockCount;
    public int UnlockCounterForAnimations;
    [SerializeField]
    private int unlockCount;
    public bool IsLocked = false;
    public Image LockImgage;
    public GameObject LockObject;

    public FirZoomAnimation endZoom;
    
    public void Initialize()
    {
        if(LockImgage == null)
            return;
        
        LockImgage.fillAmount = 0;
    }
    
    public void UnlockADS()
    {
        if(endZoom is null || endZoom.enabled)
            return;
        
#if IS_YANDEX
        YG2.RewardedAdvShow("OpenWorkerPlace", DoDestroy);
#else
         DoDestroy()
#endif
    }
    
    public void ToUnlock(int count)
    {
        if(LockImgage == null)
            return;
        
        unlockCount += count;
        LockImgage.fillAmount = unlockCount / (UnlockCount*3f);//3f - is a FlyLootManager.instance.AnimateGoods(targer.Renderer, 3);
        if(LockImgage.fillAmount < 1)
            return;

        DoDestroy();
    }

    public void DoDestroy()
    {
        IsLocked = false;
        endZoom.OnComplete = () =>
        {
            if(endZoom is not null)
                endZoom.OnComplete = null;
            Destroy(LockObject);
        };
        endZoom.Play();
        SoundManager.Instance.PlayBarDespawn();
    }

    public void HandleUpdate(float multiplicator = 1)
    {
        cooldown -= Time.deltaTime * multiplicator;
        if (cooldown < 0)
        {
            cooldown += Cooldown;
            enabled = false;
        }
    }
}