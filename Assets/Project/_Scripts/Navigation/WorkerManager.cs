using System;
using System.Collections.Generic;
using System.Linq;
using FirAnimations;
using TMPro;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public WorkerPlace[] Places;
    public Worker WorkerPerfab;
    public Transform WorkerPool;
    public Transform PointyPool;
    public Transform ColorBarsFlyParent;
    public MainImageManager MainImage;
    public Transform zoomPlug;
    public TextMeshProUGUI pointsText;

    public GameObject WinPopup;
    public GameObject LosePopup;
    
    private List<Worker> workers;
    private SaveData player;

    private int workerCount = 0;
    private float timerTime = 2f;
    private float loseTimerTime;
    
    public void Initialize(SaveData player)
    {
        this.player = player;
        enabled = true;
        loseTimerTime = timerTime;
        foreach (WorkerPlace workerPlace in Places)
        {
            workerPlace.NearestPlace = MainImage.Places
                .Cast<PointyPlace>()
                .OrderBy(bar => (bar.transform.position - workerPlace.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }
    }

    public bool TryAddColorBar(ColorBar bar)
    {
        foreach (var workerPlace in Places)
        {
            if(workerPlace.bar is not null)
                continue;
            if(workerPlace.transform.childCount > 0)
                continue;
            workerPlace.bar = bar;
            workerPlace.AddCooldown(bar.PositionAnimation.Curve.keys[^1].time);
            RectTransform rectTransform = bar.GetComponent<RectTransform>();
            Transform transformPlug = bar.transform.parent;
            rectTransform.SetParent(ColorBarsFlyParent, worldPositionStays: true);
            bar.PositionAnimation.StartPosition = rectTransform.anchoredPosition3D;
            FirSizeAnimation sizeClone = Instantiate(zoomPlug, transformPlug).GetComponent<FirSizeAnimation>();
            sizeClone.transform.SetAsFirstSibling();
            sizeClone.OnComplete = () =>
                { Destroy(sizeClone.gameObject); };
            sizeClone.Play();
            rectTransform.anchorMin = new Vector2(.5f, .5f);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.localPosition = Vector3.zero;
            bar.transform.SetParent(workerPlace.transform);
            bar.PositionAnimation.EndPosition = -rectTransform.anchoredPosition3D;
            rectTransform.SetParent(ColorBarsFlyParent);
            //bar.transform.localPosition = Vector3.zero;
            bar.PositionAnimation.OnComplete = () =>
            {
                bar.transform.SetParent(workerPlace.transform, worldPositionStays: true);
                bar.enabled = true;
            };
            bar.PositionAnimation.Play();
            bar.ZoomAnimation.Play();
            return true;
        }
        
        return false;
    }

    private void Update()
    {
        foreach (WorkerPlace workerPlace in Places)
        {
            if (workerPlace.bar is null)
                continue;
            if (workerPlace.enabled)
            {
                workerPlace.HandleUpdate();
                continue;
            }
            if (workerPlace.bar.Count <= 0)
                continue;
            if (workerPlace.isWarningNumber == MainImage.PixelCount)
                continue;
            
            PointyPlace targer = GetColorBar(workerPlace, out List<Vector3> way);
            if (targer is null)
            {
                workerPlace.isWarningNumber = MainImage.PixelCount;
                //workerPlace.enabled = true;
                //workerPlace.AddCooldown(1f);
                continue;
            }

            loseTimerTime = timerTime;
            workerPlace.bar.Count--;
            workerPlace.bar.Text.text = workerPlace.bar.Count.ToString();
            
            Worker newWorker = GetWorker();
            workerCount++;
            newWorker.transform.position = workerPlace.transform.position;
            newWorker.Way = way;
            newWorker.Sprite.color = workerPlace.bar.Image.color;
            newWorker.ToStart();
            newWorker.OnFirstPoint = () =>
            {
                newWorker.OnFirstPoint = null;
                targer.CheckWayCost();
            };
            newWorker.OnEndWay = () =>
            {
                workerCount--;
                MainImage.PixelCount--;
                targer.Eat();
                FlyLootManager.instance.AnimateGoods(targer.Renderer, targer.transform, pointsText.transform);
                CheckWin();
            };
            targer.InGame = false;
            newWorker.gameObject.SetActive(true);
            
            if (workerPlace.bar.Count <= 0)
            {
                AnimationCurve curveZoom = new AnimationCurve(
                    new Keyframe(0f, 0f, 0f, 0f),
                    new Keyframe(.4f, 1f, 2f, 2f)
                );
                
                var animationZoom = workerPlace.bar.gameObject.AddComponent<FirZoomAnimation>();
                animationZoom.StartZoom = Vector3.one;
                animationZoom.EndZoom = Vector3.zero;
                animationZoom.OnComplete += () =>
                {
                    Destroy(workerPlace.bar.gameObject);
                    workerPlace.bar = null;
                };
                animationZoom.Curve = curveZoom;
                animationZoom.Play();
            }
            else
            {
                workerPlace.enabled = true;
            }
        }
        
        if(workerCount > 0)
            return;

        CheckLose();
    }

    private void CheckLose()
    {
        if(MainImage.PixelCount <= 0)
            return;
        foreach (WorkerPlace workerPlace in Places)
        {
            if(workerPlace.isWarningNumber != MainImage.PixelCount
               && workerPlace.transform.childCount == 0)
                return;
        }

        loseTimerTime -= Time.deltaTime;
        if(loseTimerTime <= 0)
            LosePopup.SetActive(true);
    }
    
    private void CheckWin()
    {
        if(MainImage.PixelCount > 0)
            return;

        player.Level++;
        player.Save();
        WinPopup.SetActive(true);
    }

    private Worker GetWorker()
    {
        Worker newWorker = null;
        for(int i=0; i<WorkerPool.childCount; i++)
        {
            if(WorkerPool.GetChild(i).gameObject.activeSelf)
                continue;
            newWorker = WorkerPool.GetChild(i).GetComponent<Worker>();
        }
        if(newWorker is null)
            newWorker = Instantiate(WorkerPerfab, WorkerPool);
        return newWorker;
    }
    private PointyPlace GetColorBar(WorkerPlace workerPlace, out List<Vector3> way)
    {
        way = new();

        var queue = new Queue<PointyPlace>();
        var cameFrom = new Dictionary<PointyPlace, PointyPlace>();
        var originOf = new Dictionary<PointyPlace, PointyPlace>();
        
        var sorted = Enumerable.Range(0, MainImage.Places.GetLength(0))
            .Select(x => MainImage.Places[x, 0])
            .OrderBy(p => (p.transform.position - workerPlace.transform.position).sqrMagnitude)
            .ToArray();
        
        foreach (var start in sorted)
        {
            Debug.Log(start.transform.position, start);
            if (start == null) continue;
            if (cameFrom.ContainsKey(start)) continue;

            cameFrom[start] = null;
            originOf[start] = start;
            queue.Enqueue(start);
        }
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var origin = originOf[current];
            
            if (current != origin
                && current.Renderer.color == origin.Renderer.color
                && current.IsOpen
                && current.InGame)
            {
                way = ReconstructPath(cameFrom, current);
                return current;
            }

            foreach (var neighbor in current.Neighbors)
            {
                if (neighbor == null) continue;
                if (cameFrom.ContainsKey(neighbor)) continue;
                if (!neighbor.IsOpen) continue;
                if (!neighbor.InGame) continue;

                cameFrom[neighbor] = current;
                originOf[neighbor] = origin;
                queue.Enqueue(neighbor);
            }
        }

        return null;
        
        /*result = MainImage.Places
            .Cast<PointyPlace>()
            .Where(bar => bar.IsOpen)
            .Where(bar => bar.InGame)
            .Where(bar => bar.Renderer.color == workerPlace.bar.Image.color)
            .OrderBy(bar => bar.BestNeighborWayCost)
            .ThenBy(bar => (bar.transform.position - workerPlace.transform.position).sqrMagnitude)
            .FirstOrDefault();
        
        if (result is null)
            return result;
        
        way = GetWay(result);
        
        return result;*/
    }

    private static List<Vector3> ReconstructPath(Dictionary<PointyPlace, PointyPlace> cameFrom, PointyPlace target)
    {
        var path = new List<Vector3>();
        PointyPlace current = target;
        while (current != null)
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
    
    /*private List<Vector3> GetWay(PointyPlace startPoint)
    {
        List<PointyPlace> result = new List<PointyPlace> { startPoint };

        PointyPlace currentPoint = startPoint;
        while (currentPoint.WayCost > 0)
        {
            var bestNeighbor = currentPoint.Neighbors
                .Where(place => place != null
                                && !place.IsDeadEnd
                                && !result.Contains(place))
                .OrderBy(n => n.WayCost)
                .ThenBy(bar => bar.transform.position.y)
                .FirstOrDefault();

            if (bestNeighbor == null)
                throw new Exception();
            
            currentPoint = bestNeighbor;
            result.Add(currentPoint);
        }

        List<Vector3> resultVector = new();

        result.Reverse();
        
        foreach (PointyPlace place in result)
        {
            resultVector.Add(place.transform.position);
        }
        
        return resultVector;
    }*/
}