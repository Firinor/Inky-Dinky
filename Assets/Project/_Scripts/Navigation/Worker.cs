using System;
using System.Collections;
using System.Collections.Generic;
using FirAnimations;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public Action OnEndWay;
    public Action OnFirstPoint;
    public List<Vector3> Way;
    public float speed = 1f;
    public float rotationSpeed = 1f;
    public float eatTime = .6f;
    public SpriteRenderer Sprite;
    public float flipTime = .2f;
    public float distansToPoint = 1f;
    public FirZoomAnimation ZoomAnimation;

    private bool isEndCoroutine;
    private float flipTimer;
    private int wayIndex;

    public void ToStart()
    {
        wayIndex = 0;
        flipTimer = flipTime;
        LookToTarget();
        ZoomAnimation.Play();
    }

    private void LookToTarget()
    {
        if (wayIndex >= Way.Count)
            return;
        
        float angle = Vector2.SignedAngle(Vector2.up, (Vector2)Way[wayIndex] - (Vector2)transform.position);
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
    
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    public void Update()
    {
        flipTimer -= Time.deltaTime;
        if (flipTimer < 0)
        {
            flipTimer += flipTime;
            Sprite.flipX = !Sprite.flipX;
        }
        
        LookToTarget();
        
        if(isEndCoroutine)
            return;
        
        transform.position = Vector2.MoveTowards(
            transform.position,
            Way[wayIndex],
            speed * Time.deltaTime
        );
        
        if(Vector2.Distance(transform.position, Way[wayIndex]) < distansToPoint)
        {
            wayIndex++;
            OnFirstPoint?.Invoke();
            if (wayIndex >= Way.Count)
                StartCoroutine(EatCoroutine());
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