using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlyLootManager : MonoBehaviour
{
    public static FlyLootManager instance;

    [SerializeField] 
    private Transform parent;
    [SerializeField] 
    private FlyLoot prefab;
    [SerializeField] 
    private float spawnRadius;
    [SerializeField] 
    private float spawnTotalTime = 1f;

    [Header("Test")]
    [SerializeField] 
    private Sprite testSprite;
    [SerializeField] 
    private Transform testStartpoint;
    [SerializeField] 
    private Transform testEndpoint;
    [SerializeField] 
    private int testCount;

    private List<FlyLoot> lootPool = new();
    
    private void Awake()
    {
        instance = this;
    }

    public void AnimateGoods(SpriteRenderer goods, Transform startPoint, Transform endPoint, Action OnEndAction = null, int count = 1)
    {
        StartCoroutine(AnimateGoodsCoroutine(goods, startPoint, endPoint, OnEndAction, count));
    }

    private IEnumerator AnimateGoodsCoroutine(SpriteRenderer goods, Transform startPoint, Transform endPoint, Action OnEndAction = null, int count = 1)
    {
       // bool isOffset = count > 1;
        
        float timer = 0;
        float yieldDelay = spawnTotalTime/count;

        for (int i = 0; i < count; i++)
        {
            timer -= yieldDelay;
            while (timer < 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;

            FlyLoot newGoods = lootPool.FirstOrDefault(loot => !loot.gameObject.activeSelf);

            if (newGoods is null)
            {
                newGoods = Instantiate(prefab, startPoint.position , Quaternion.identity, parent);
                lootPool.Add(newGoods);
            }
            else
            {
                newGoods.ResetLoot();
                newGoods.transform.position = startPoint.position;
                newGoods.gameObject.SetActive(true);
            }
            newGoods.OnFlyEndAction = OnEndAction;
            Color newColor = WorkerManager.GetSimilarColorRGB(goods.color);
            newGoods.SetDestination(goods.sprite, newColor, endPoint, randomOffset);
            newGoods.OnPointerEnterAction += FlyToEnd;
        }
    }

    public void FlyToEnd()
    {
        foreach (FlyLoot loot in lootPool)
        {
            loot.FlyToEnd();
            loot.OnPointerEnterAction -= FlyToEnd;
        }
    }
}