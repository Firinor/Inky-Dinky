using System.Collections.Generic;
using System.Linq;
using FirAnimations;
using UnityEngine;

#if IS_YANDEX
using YG;
#endif

using Random = UnityEngine.Random;

public class WorkerManager : MonoBehaviour
{
    public WorkerPlace[] Places;
    public Worker WorkerPerfab;
    public Transform WorkerPool;
    public Transform PointyPool;
    public Transform ColorBarsFlyParent;
    public MainImageManager MainImage;
    public ColorJarsManager ColorJarsManager;
    public Transform zoomPlug;
    public Transform NoLockPlaces;
    public float deltaColor = 0.1f;

    public int ColorBarCount;
    public float fastWorkersSpeed = 6;
    public float slowWorkersSpeed = 2;
    private bool isWorkersSpeed = false;
    public float fastWorkersSpawnMultiplicator = 2;
    
    public GameObject WinPopup;
    public GameObject LosePopup;
    
    private List<Worker> workers;
    private SaveData player;
    
    private int workerCount = 0;
    private float timerTime = 2f;
    private float loseTimerTime;

    private int workerSortingLayer = 100;
    
    public void Initialize(SaveData player)
    {
        this.player = player;
        enabled = true;
        loseTimerTime = timerTime;
        foreach (WorkerPlace workerPlace in Places)
        {
            workerPlace.Initialize();
            Vector2 point1 = workerPlace.transform.position;
            workerPlace.NearestPlaces = Enumerable.Range(0, MainImage.Places.GetLength(0))
                .Select(x => MainImage.Places[x, 0])
                .OrderBy(place => (place.transform.position - workerPlace.transform.position).sqrMagnitude)
                .ToDictionary(
                    place => place, 
                    place => Vector2.Distance(place.transform.position, point1) / MainImage.deltaX
                    );
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
            //workerPlace.AddCooldown(bar.PositionAnimation.Curve.keys[^1].time);
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
            bar.InAnimation = true;
            bar.PositionAnimation.OnComplete = () =>
            {
                bar.transform.SetParent(workerPlace.transform, worldPositionStays: true);
                bar.enabled = true;
                bar.InAnimation = false;
            };
            bar.PositionAnimation.Play();
            bar.ZoomAnimation.Play();
            ColorBarCount--;
            if (ColorBarCount <= 0)
                isWorkersSpeed = true;
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
            if (workerPlace.bar.InAnimation)
                continue;
            if (workerPlace.enabled)
            {
                workerPlace.HandleUpdate(isWorkersSpeed ? fastWorkersSpawnMultiplicator : 1);
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
            newWorker.speed = isWorkersSpeed ? fastWorkersSpeed : slowWorkersSpeed;
            newWorker.Sprite.color = workerPlace.bar.Image.color;
            SoundManager.Instance.PlayWorkerSpawn();
            newWorker.ToStart();
            newWorker.OnFirstPoint = () =>
            {
                newWorker.OnFirstPoint = null;
                targer.CheckWayCost();
            };
            newWorker.OnStartEat = () =>
            {
                Transform targetPlace = NoLockPlaces;
                WorkerPlace targetWorkPlace = Places.FirstOrDefault(p => p.IsLocked);
                if (targetWorkPlace != null)
                {
                    targetWorkPlace.UnlockCounterForAnimations++;
                    if (targetWorkPlace.UnlockCounterForAnimations >= targetWorkPlace.UnlockCount)
                    {
                        targetWorkPlace.IsLocked = false;
                    }
                    targetPlace = targetWorkPlace.transform;
                }
                FlyLootManager.instance.AnimateGoods(targer.Renderer, 
                    targer.transform, 
                    targetPlace,
                    OnEndAction: () =>
                    {
                        targetWorkPlace?.ToUnlock(1);
                    },
                    3);
            };
            newWorker.OnEndWay = () =>
            {
                workerCount--;
                MainImage.PixelCount--;
                targer.Eat();
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
                GameObject bar = workerPlace.bar.gameObject;
                animationZoom.OnComplete += () =>
                {
                    Destroy(bar);
                };
                workerPlace.bar = null;
                bar.transform.SetParent(workerPlace.transform.parent, worldPositionStays:true);
                animationZoom.Curve = curveZoom;
                animationZoom.Play();
                SoundManager.Instance.PlayBarDespawn();
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
        if(loseTimerTime > 0)
            return;
        
        LosePopup.SetActive(true);
        SoundManager.Instance.PlayLose();
    }

    public void ReviewGame()
    {
#if IS_YANDEX
        YG2.RewardedAdvShow("Shuffle", ClearWorkplace);
#else
        ClearWorkplace();
#endif
    }

    private void ClearWorkplace()
    {
        List<ColorBar> candidats = new();
        foreach (WorkerPlace workerPlace in Places)
        {
            ColorBar bar = workerPlace.GetComponentInChildren<ColorBar>();
            if (bar == null) 
                continue;
            candidats.Add(bar);
        }

        ColorBar chosen = candidats.GetRandom();
        WorkerPlace place = chosen.GetComponentInParent<WorkerPlace>();
        place.bar = null;
        place.isWarningNumber = int.MaxValue;
        ColorJarsManager.AddBar(chosen);
        LosePopup.SetActive(false);
        loseTimerTime = timerTime;
    }
    
    private void CheckWin()
    {
        if(MainImage.PixelCount > 0)
            return;

        player.Level++;
        player.Save();
        SoundManager.Instance.PlayWin();
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

        if (newWorker is null)
        {
            newWorker = Instantiate(WorkerPerfab, WorkerPool);
            newWorker.GetComponent<SpriteRenderer>().sortingOrder = workerSortingLayer;
            workerSortingLayer++;
        }
        return newWorker;
    }
    private PointyPlace GetColorBar(WorkerPlace workerPlace, out List<Vector3> way)
    {
        Color targetColor = workerPlace.bar.Image.color;
        way = new();
        
        PointyPlace result = MainImage.Places
            .Cast<PointyPlace>()
            .Where(bar => bar.IsOpen)
            .Where(bar => bar.InGame)
            .Where(bar => bar.Renderer.color == targetColor)
            .OrderBy(bar =>
            {
                List<PointyPlace> points = GetWayPointyPlace(bar);
                return points.Count + workerPlace.NearestPlaces[points[0]];
            })
            .FirstOrDefault();
        
        if (result is null)
            return result;
        
        way = getWayVector3(result);
        
        return result;
        
        /*var checkedPoints = new List<PointyPlace>(candidats);
        var checkedPoints = new List<PointyPlace>(candidats);

        while (candidats.Count > 0)
        {
            foreach (var candidat in candidats)
            {
                if (candidat.Renderer.color == targetColor
                    && candidat.IsOpen
                    && candidat.InGame)
                {
                    candidats.Add(start);
                    continue;
                }
                
                cameFrom[start] = null;
                originOf[start] = start;
                queue.Enqueue(start);
            }
        }
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var origin = originOf[current];
            
            if (current.Renderer.color == targetColor
                && current.IsOpen
                && current.InGame)
            {
                if (current == origin)
                    way = new List<Vector3>{current.transform.position};
                else
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
        }*/
        
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
    private List<Vector3> getWayVector3(PointyPlace startPoint)
    {
        List<PointyPlace> result = new List<PointyPlace> { startPoint };

        PointyPlace currentPoint = startPoint;
        while (true)
        {
            if(currentPoint.Neighbors.Any(neighbor => neighbor == null))
                break;
            
            var bestNeighbor = currentPoint.Neighbors
                .Where(place => !place.IsDeadEnd
                                && !result.Contains(place))
                .OrderBy(n => n.WayCost)
                .ThenBy(bar => bar.transform.position.y)
                .FirstOrDefault();
            
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
    private List<PointyPlace> GetWayPointyPlace(PointyPlace startPoint)
    {
        List<PointyPlace> result = new List<PointyPlace> { startPoint };

        PointyPlace currentPoint = startPoint;
        while (currentPoint != null)
        {
            if(currentPoint.Neighbors.Any(neighbor => neighbor == null))
                break;
            
            var bestNeighbor = currentPoint.Neighbors
                .Where(place => !place.IsDeadEnd
                                && !result.Contains(place)
                                && place.WayCost < int.MaxValue)
                .OrderBy(n => n.WayCost)
                .ThenBy(bar => bar.transform.position.y)
                .FirstOrDefault();
            
            currentPoint = bestNeighbor;
            result.Add(currentPoint);
        }
        result.Reverse();
        return result;
    }
    public static Color GetSimilarColorRGB(Color baseColor, float maxDelta = 0.1f)
    {
        return new Color(
            Mathf.Clamp01(baseColor.r + Random.Range(-maxDelta, maxDelta)),
            Mathf.Clamp01(baseColor.g + Random.Range(-maxDelta, maxDelta)),
            Mathf.Clamp01(baseColor.b + Random.Range(-maxDelta, maxDelta)),
            baseColor.a
        );
    }
}