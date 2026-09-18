using System.Linq;
using UnityEngine;

public class PointyPlace : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public int WayCost = int.MaxValue;
    public PointyPlace[] Neighbors;
    public bool InGame = true;

    public bool IsDeadEnd = false;
    public bool InCheck = false;
    
    public bool IsOpen => Neighbors.Any(n => n == null || !n.InGame);

    public void Eat()
    {
        Renderer.enabled = false;
        InGame = false;
        CheckWayCost();
    }

    private void CheckWayCost()
    {
        int cost = int.MaxValue;
        foreach (PointyPlace place in Neighbors)
        {
            if(place is null)
                continue;
            if(place.IsDeadEnd)
                continue;
            if(place.InCheck)
                continue;
            if(place.IsDeadEnd)
                continue;
            if(place.WayCost >= cost)
                continue;
            cost = place.WayCost;
            InCheck = true;
            place.CheckWayCost();
        }
        if(cost < int.MaxValue
           && cost < WayCost)
            WayCost = cost + 1;
    }
}