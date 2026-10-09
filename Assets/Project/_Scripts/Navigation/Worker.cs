using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public Action OnEndWay;
    public Action OnFirstPoint;
    public List<Vector3> Way;
    public float speed = 1f;
    public float eatTime = .6f;
    public SpriteRenderer Sprite;
    public float flipTime = .2f;
    public float distansToPoint = 1f;

    private bool isEndCoroutine;
    private float flipTimer;
    private int wayIndex;

    public void ToStart()
    {
        wayIndex = 0;
        flipTimer = flipTime;
        LookToTarget();
    }

    private void LookToTarget()
    {
        float angle = Vector2.SignedAngle(Vector2.up, Way[wayIndex]-transform.position);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Update()
    {
        flipTimer -= Time.deltaTime;
        if (flipTimer < 0)
        {
            flipTimer += flipTime;
            Sprite.flipX = !Sprite.flipX;
        }
        
        if(isEndCoroutine)
            return;
        
        transform.position = Vector3.MoveTowards(
            transform.position,
            Way[wayIndex],
            speed * Time.deltaTime
        );
        
        if(Vector3.Distance(transform.position, Way[wayIndex]) < distansToPoint)
        {
            wayIndex++;
            OnFirstPoint?.Invoke();
            if (wayIndex >= Way.Count)
                StartCoroutine(EatCoroutine());
            else
                LookToTarget();
        }
    }

    private IEnumerator EatCoroutine()
    {
        isEndCoroutine = true;
        //SoundManager.Instance.PlayOpenScroll();
        yield return new WaitForSeconds(eatTime);
        gameObject.SetActive(false);
        isEndCoroutine = false;
        OnEndWay?.Invoke();
    }
}