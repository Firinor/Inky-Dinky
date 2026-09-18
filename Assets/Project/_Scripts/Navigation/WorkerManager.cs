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
            if(targer is null)
                continue;
            
            Worker newWorker = GetWorker();
            newWorker.transform.position = workerPlace.transform.position;
            newWorker.Way = way;
            newWorker.Sprite.color = workerPlace.bar.Image.color;
            newWorker.ToStart();
            newWorker.OnEndWay = () =>
            {
                foreach (PointyPlace place in MainImage.Places)
                {
                    place.InCheck = false;
                }
                targer.Eat();
            };
            newWorker.gameObject.SetActive(true);

            targer.InGame = false;
            workerPlace.enabled = true;
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
        
        IOrderedEnumerable<PointyPlace> listBars = MainImage.Places
            .Cast<PointyPlace>()
            .Where(bar => bar.IsOpen)
            .Where(bar => bar.InGame)
            .Where(bar => bar.Renderer.color == workerPlace.bar.Image.color)
            .OrderBy(bar => (bar.transform.position - workerPlace.transform.position).sqrMagnitude);

        result = listBars.FirstOrDefault();
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
            PointyPlace nextPoint = null;
            foreach (PointyPlace pointNeighbor in currentPoint.Neighbors)
            {
                if(pointNeighbor == null)
                    continue;
                if(pointNeighbor.InGame)
                    continue;
                if (nextPoint == null)
                {
                    nextPoint = pointNeighbor;
                    continue; 
                }
                if(pointNeighbor.WayCost < nextPoint.WayCost)
                    nextPoint = pointNeighbor;
            }

            currentPoint = nextPoint;
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