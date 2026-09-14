using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public WorkerPlace[] Places;
    public Worker WorkerPerfab;
    public Transform WorkerPool;

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
}

public class Worker : MonoBehaviour
{
    
}