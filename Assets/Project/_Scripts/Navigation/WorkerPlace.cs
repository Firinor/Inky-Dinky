using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    public void ToUnlock(int count)
    {
        if(LockImgage == null)
            return;
        
        unlockCount += count;
        LockImgage.fillAmount = unlockCount / (float)UnlockCount;
        if(LockImgage.fillAmount < 1)
            return;

        UnlockADS();
    }

    public void UnlockADS()
    {
        IsLocked = false;
        Destroy(LockObject);
    }

    public void HandleUpdate()
    {
        cooldown -= Time.deltaTime;
        if (cooldown < 0)
        {
            cooldown += Cooldown;
            enabled = false;
        }
    }

    public void AddCooldown(float value)
    {
        cooldown += value;
    }
}