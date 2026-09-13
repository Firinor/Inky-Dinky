using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorJarsManager : MonoBehaviour
{
    public int MinJarCount;
    public Vector2Int JarNumberMinMax;
    public RectTransform[] Collumns;
    public ColorBar BarPrefab;
    
    public void Initialize()
    {
        //GetImage();
        GenerateColorSequence();
    }

    private void GenerateColorSequence()
    {
        foreach (var collumn in Collumns)
        {
            collumn.ClearAll(instant: true);
        }

        int deltaJar = JarNumberMinMax.y - JarNumberMinMax.x;
        
        Sprite mainSprite = MainImageManager.MainSprite;
        
        Texture2D tex = mainSprite.texture;
        Rect rect = mainSprite.textureRect;
        
        Color[] pixels = tex.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height
        );

        List<ColorBar> colorBars = new List<ColorBar>();
        
        Dictionary<Color, int> counts = pixels
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
        
        foreach (var i in counts)
        {
            //Debug.Log(i.Key + " " + i.Value);
            if (i.Value > JarNumberMinMax.y)
            {
                int j = 0;
                while (j < i.Value)
                {
                    int colorValue = JarNumberMinMax.x + Random.Range(0, deltaJar);
                    j += colorValue;
                    if (j > i.Value)
                        colorValue -= j - i.Value;
                    ColorBar newColorBar = Instantiate(BarPrefab, Collumns[0]);
                    newColorBar.Image.color = i.Key;
                    newColorBar.Text.text = colorValue.ToString();
                    newColorBar.Count = colorValue;
                    colorBars.Add(newColorBar);
                }
            }
            else
            {
                ColorBar newColorBar = Instantiate(BarPrefab, Collumns[0]);
                newColorBar.Image.color = i.Key;
                newColorBar.Text.text = i.Value.ToString();
                newColorBar.Count = i.Value;
                colorBars.Add(newColorBar);
            }
        }
        
        colorBars.Shuffle();

        int columnIndex = 0;
        foreach (var colorBar in colorBars)
        {
            colorBar.transform.SetParent(Collumns[columnIndex]);
            columnIndex++;
            columnIndex %= Collumns.Length;
        }
    }
}