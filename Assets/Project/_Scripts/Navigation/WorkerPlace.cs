using System.Collections.Generic;
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
    private int unlockCount;
    public bool IsLocked = false;
    public Image LockImgage;
    public GameObject LockObject;

    public void Initialize()
    {
        if(LockImgage == null)
            return;
        
        LockImgage.fillAmount = 0;
    }
    
    public void UnlockADS()
    {
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
        LockImgage.fillAmount = unlockCount / (float)UnlockCount;
        if(LockImgage.fillAmount < 1)
            return;

        DoDestroy();
    }

    public void DoDestroy()
    {
        IsLocked = false;
        Destroy(LockObject);
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