using System;
using System.Collections;
using System.Collections.Generic;
using FirAnimations;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public Action OnEndWay;
    public Action OnStartEat;
    public Action OnFirstPoint;
    public List<Vector3> Way;
    public float speed = 1f;
    public float rotationSpeed = 1f;
    public float eatTime = .2f;
    public Sprite[] MoveFrames;
    private int frameIndex;
    public Sprite EatFrame;
    public SpriteRenderer Sprite;
    public float flipTime = .3f;
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
        frameIndex = 0;
        Sprite.sprite = MoveFrames[0];
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
        if(isEndCoroutine)
            return;
        
        flipTimer -= Time.deltaTime;
        if (flipTimer < 0)
        {
            flipTimer += flipTime;
            frameIndex++;
            frameIndex = frameIndex % MoveFrames.Length;
            Sprite.sprite = MoveFrames[frameIndex];
        }
        
        LookToTarget();
        
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
        Sprite.sprite = EatFrame;
        OnStartEat?.Invoke();
        SoundManager.Instance.PlayWorkerEat();
        yield return new WaitForSeconds(eatTime);
        Sprite.flipX = true;
        SoundManager.Instance.PlayWorkerEat();
        yield return new WaitForSeconds(eatTime);
        Sprite.flipX = false;
        SoundManager.Instance.PlayWorkerEat();
        yield return new WaitForSeconds(eatTime);
        Sprite.flipX = true;
        ZoomAnimation.OnComplete = () =>
        {
            ZoomAnimation.OnComplete = null;
            gameObject.SetActive(false);
            isEndCoroutine = false;
        };
        ZoomAnimation.Reverse();
        OnEndWay?.Invoke();
    }
}