using System;
using UnityEngine;

public class MainImageManager : MonoBehaviour
{
    public PointyPlace PointyPrefab;
    public Transform PointyPool;
    public PointyPlace[,] Places;
    public Transform LowerLeftCorner;
    public Transform UperRightCorner;
    public Vector2 MainImageSize;

    public Sprite[] Levels;
    
    public static Sprite MainSprite;
    private const int maxPointyCount = 10000;

    public int PixelCount;

    public void Initialize(SaveData player)
    {
        LoadLevel(player.Level);
        CreateMainImage();
    }
    
    private void LoadLevel(int level)
    {
        MainSprite = Levels[level];
    }
    
    [ContextMenu(nameof(CreateMainImage))]
    private void CreateMainImage()
    {
        if (MainSprite == null || PointyPrefab == null) 
            return;
        
        PointyPool.ClearAll(instant: true);
        PixelCount = 0;
        
        Texture2D tex = MainSprite.texture;
        Rect rect = MainSprite.textureRect;
        
        Color[] pixels = tex.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height
        );

        int w = (int)rect.width;
        int h = (int)rect.height;
        
        float deltaX = UperRightCorner.position.x - LowerLeftCorner.position.x;
        deltaX = deltaX / MainImageSize.x;
        
        Vector2 pivot = MainSprite.pivot;

        int created = 0;

        Places = new PointyPlace[w,h];
        
        PointyPlace deadEnd = Instantiate(PointyPrefab, PointyPool);
        deadEnd.gameObject.SetActive(false);
        deadEnd.name = "DeadEnd";
        deadEnd.IsDeadEnd = true;
        
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                PixelCount++;
                Color c = pixels[y * w + x];
                
                float px = (x - pivot.x) * deltaX + deltaX/2;
                float py = (y - pivot.y) * deltaX + deltaX/2;

                PointyPlace pointy = Instantiate(PointyPrefab, PointyPool);
                pointy.name = pointy.name + $"X{x}Y{y}";
                pointy.transform.localPosition = new Vector3(px, py, 0f);
                pointy.transform.localScale = Vector3.one * deltaX;
                pointy.Renderer.color = c;
                pointy.Neighbors = new PointyPlace[4];
                if (y == 0)
                {
                    
                    pointy.WayCost = 0;
                }

                created++;
                if (created >= maxPointyCount) 
                    throw new Exception();
                
                if(x == 0)
                    pointy.Neighbors[(int)ESide.Left] = deadEnd;
                else if (x == w - 1)
                {
                    pointy.Neighbors[(int)ESide.Left] = Places[x-1,y];
                    pointy.Neighbors[(int)ESide.Right] = deadEnd;
                    Places[x-1,y].Neighbors[(int)ESide.Right] = pointy;
                }
                else
                {
                    pointy.Neighbors[(int)ESide.Left] = Places[x-1,y];
                    Places[x-1,y].Neighbors[(int)ESide.Right] = pointy;
                }

                if (y == 0)
                    { }
                else if (y == h - 1)
                {
                    pointy.Neighbors[(int)ESide.Down] = Places[x,y-1];
                    pointy.Neighbors[(int)ESide.Up] = deadEnd;
                    Places[x,y-1].Neighbors[(int)ESide.Up] = pointy;
                }
                else
                {
                    pointy.Neighbors[(int)ESide.Down] = Places[x,y-1];
                    Places[x,y-1].Neighbors[(int)ESide.Up] = pointy;
                }
                
                Places[x,y] = pointy;
            }
        }
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var place = Places[x, y];
                if(Mathf.Approximately(place.Renderer.color.a, 1))
                    continue;
                
                PixelCount--;
                place.Eat();
                place.InGame = false;
                place.CheckWayCost();
            }
        }
    }
}

public enum ESide
{
    Down = 0,
    Left = 1,
    Right = 2,
    Up = 3,
}
