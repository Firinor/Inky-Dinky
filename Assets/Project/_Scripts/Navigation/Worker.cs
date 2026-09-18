using System;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public Action OnEndWay;
    public List<Vector3> Way;
    public float speed = 1f;
    public SpriteRenderer Sprite;
    
    private int wayIndex;

    public void ToStart()
    {
        wayIndex = 0;
    }
    
    public void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            Way[wayIndex],
            speed * Time.deltaTime
        );
        
        if(Vector3.Distance(transform.position, Way[wayIndex]) < 0.1f)
        {
            wayIndex++;
            if (wayIndex >= Way.Count)
            {
                gameObject.SetActive(false);
                OnEndWay?.Invoke();
            }
        }
    }
}