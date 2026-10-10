using System.Collections.Generic;
using UnityEngine;

public class WorkerPlace : MonoBehaviour
{
    public float Cooldown = 1f;
    public ColorBar bar;
    public Dictionary<PointyPlace, float> NearestPlaces;

    public float cooldown;
    public int isWarningNumber = int.MaxValue;
    
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