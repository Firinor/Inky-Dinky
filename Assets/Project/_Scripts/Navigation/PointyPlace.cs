using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PointyPlace : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public int WayCost = int.MaxValue;
    public PointyPlace[] Neighbors;
    public bool InGame = true;
    //public TextMeshProUGUI textTemp;

    public bool IsDeadEnd = false;
    
    public PointyPlace BestNeighbor => Neighbors
        .Where(place => place != null
            && !place.IsDeadEnd
            && !place.InGame)
        .OrderBy(n => n.WayCost)
        .FirstOrDefault();

    public int BestNeighborWayCost
    {
        get
        {
            PointyPlace bestNeighbor = BestNeighbor;
            if (bestNeighbor == null)
                return int.MaxValue;
            return bestNeighbor.WayCost;
        }
    }

    public bool IsOpen => Neighbors.Any(n => n == null || (!n.InGame && n.WayCost < int.MaxValue));

    public void Eat()
    {
        Renderer.enabled = false;
    }

    public void CheckWayCost()
    {
        if(InGame)
            return;

        PointyPlace bestNeighbor = BestNeighbor;
        
        if(bestNeighbor == null)
            return;
        if (bestNeighbor.WayCost > WayCost)
        {
            Debug.LogError($"bestNeighbor.WayCost({bestNeighbor.WayCost}) > WayCost ({WayCost})");
            throw new Exception();
        }
        
        WayCost = Math.Min(WayCost, bestNeighbor.WayCost + 1);
        //textTemp.text = WayCost.ToString();
        foreach (PointyPlace pointyPlace in Neighbors)
        {
            if(pointyPlace == null)
                continue;
            if(pointyPlace.WayCost == 0)
                continue;
            if(pointyPlace.WayCost <= WayCost+1)
                continue;
            
            pointyPlace.CheckWayCost();
        }
    }
}