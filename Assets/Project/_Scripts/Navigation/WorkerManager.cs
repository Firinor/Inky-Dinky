using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public WorkerPlace[] Places;
    public Worker WorkerPerfab;
    public Transform WorkerPool;
    public Transform PointyPool;
    public MainImageManager MainImage;

    private List<Worker> workers;

    public void AddColorBar(ColorBar bar)
    {
        foreach (var workerPlace in Places)
        {
            if(workerPlace.bar is not null)
                continue;

            workerPlace.bar = bar;
            bar.transform.SetParent(workerPlace.transform);
            bar.transform.localPosition = Vector3.zero;
            return;
        }
        
        bar.ErrorAnimation.Play();
    }

    private void Update()
    {
        foreach (WorkerPlace workerPlace in Places)
        {
            if(workerPlace.bar is null)
                continue;
            if(workerPlace.enabled)
                continue;
            
            PointyPlace targer = GetColorBar(workerPlace, out List<Vector3> way);
            if (targer is null)
            {
                workerPlace.enabled = true;
                workerPlace.AddCooldown(1f);
                continue;
            }

            workerPlace.bar.Count--;
            workerPlace.bar.Text.text = workerPlace.bar.Count.ToString();
            
            Worker newWorker = GetWorker();
            newWorker.transform.position = workerPlace.transform.position;
            newWorker.Way = way;
            newWorker.Sprite.color = workerPlace.bar.Image.color;
            newWorker.ToStart();
            newWorker.OnEndWay = () =>
            {
                targer.Eat();
            };
            newWorker.gameObject.SetActive(true);

            targer.InGame = false;
            targer.CheckWayCost();
            if (workerPlace.bar.Count <= 0)
            {
                Destroy(workerPlace.bar.gameObject);
                workerPlace.bar = null;
            }
            else
            {
                workerPlace.enabled = true;
            }
        }
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
        PointyPlace result = null;
        way = new();
        
        result = MainImage.Places
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
        
        return result;
    }

    private List<Vector3> GetWay(PointyPlace startPoint)
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
    }
}