using System.Linq;
using UnityEngine;

public class PointyPlace : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public int WayCost = int.MaxValue;
    public PointyPlace[] Neighbors;
    
    public bool IsOpen => Neighbors.Any(n => n == null);

    public void Eat(int newCost)
    {
        
    }
}