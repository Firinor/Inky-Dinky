using System;
using UnityEngine;

public class MainImageManager : MonoBehaviour
{
    public GameObject PointyPrefab;
    public Transform PointyPool;
    public RectTransform MainImagePosition;
    public Vector2 MainImageSize;
    public float PointyScale;
    
    public static Sprite MainSprite;
    private const int maxPointyCount = 10000;

    public void Initialize()
    {
        LoadLevel();
        CreateMainImage();
    }
    
    private void LoadLevel()
    {
        MainSprite = Resources.Load<Sprite>("Levels/1");
    }
    
    [ContextMenu(nameof(CreateMainImage))]
    private void CreateMainImage()
    {
        if (MainSprite == null || PointyPrefab == null) 
            return;
        
        PointyPool.ClearAll(instant: true);
        
        Texture2D tex = MainSprite.texture;
        Rect rect = MainSprite.textureRect;
        
        Color[] pixels = tex.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height
        );

        int w = (int)rect.width;
        int h = (int)rect.height;
        
        Vector3[] corners = new Vector3[4];
        MainImagePosition.GetWorldCorners(corners);
        Vector2 startPosition = corners[0];
        Vector2 endPosition = corners[2];
        float deltaX = endPosition.x - startPosition.x;
        deltaX = deltaX / MainImageSize.x;
        float deltaY = endPosition.y - startPosition.y;
        deltaY = deltaY / MainImageSize.y;
        
        Vector2 pivot = MainSprite.pivot;

        int created = 0;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color c = pixels[y * w + x];
                if (c.a < 0.5f) continue;
                
                float px = (x - pivot.x) * deltaX + deltaX/2;
                float py = (y - pivot.y) * deltaY + deltaY/2;

                GameObject pointy = Instantiate(PointyPrefab, PointyPool);
                pointy.transform.localPosition = new Vector3(px, py, 0f);
                pointy.transform.localScale = Vector3.one * PointyScale;
                
                var sr = pointy.GetComponent<SpriteRenderer>();
                if (sr != null) 
                    sr.color = c;

                created++;
                if (created >= maxPointyCount) 
                    throw new Exception();
            }
        }
    }
}