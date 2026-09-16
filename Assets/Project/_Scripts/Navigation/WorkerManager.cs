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
        float floor = MainImage.Places[0, 0].transform.position.y;
        MainImage.Places.TryFindIndex(startPoint, out int i, out int j);

        if (i == -1)
            return null;
        
        if (startPoint.transform.position.y - floor < 0.01f)
            return new List<Vector3> { startPoint.transform.position };

        Dictionary<PointyPlace, int> costs = new();
        
        
        
        return null;
    }
}