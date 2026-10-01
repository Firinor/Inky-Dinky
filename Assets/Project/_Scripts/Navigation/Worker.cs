using System;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public Action OnEndWay;
    public Action OnFirstPoint;
    public List<Vector3> Way;
    public float speed = 1f;
    public SpriteRenderer Sprite;
    
    private int wayIndex;

    public void ToStart()
    {
        wayIndex = 0;
        float angle = Vector2.Angle(transform.position, Way[wayIndex]);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            Way[wayIndex],
            speed * Time.deltaTime
        );
        
        if(Vector3.Distance(transform.position, Way[wayIndex]) < 0.01f)
        {
            wayIndex++;
            OnFirstPoint?.Invoke();
            if (wayIndex >= Way.Count)
            {
                gameObject.SetActive(false);
                OnEndWay?.Invoke();
            }
            else
            {
                float angle = Vector2.Angle(transform.position, Way[wayIndex]);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}