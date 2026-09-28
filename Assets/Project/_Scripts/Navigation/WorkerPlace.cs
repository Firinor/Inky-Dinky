using UnityEngine;

public class WorkerPlace : MonoBehaviour
{
    public float Cooldown = 1f;
    public ColorBar bar;

    public float cooldown;
    public bool isWarning;
    
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