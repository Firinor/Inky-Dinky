using FirAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorBar : MonoBehaviour
{
    public int Count;
    public Image Image;
    public TextMeshProUGUI Text;
    public Button Button;
    public FirRotationAnimation ErrorAnimation;
    public FirPositionAnimation PositionAnimation;
    public FirZoomAnimation ZoomAnimation;
    public static WorkerManager WorkerManager;
    
    public void Instantiate()
    {
        Button.onClick.AddListener(TryWork);
    }

    public void TryWork()
    {
        if (transform.GetSiblingIndex() != 0)
            ErrorAnimation.Play();
        else
        {
            bool isAdded = WorkerManager.TryAddColorBar(this);
            if(isAdded)
                Button.onClick.RemoveAllListeners();
            else
                ErrorAnimation.Play();
        }
    }

    private void OnDestroy()
    {
        Button.onClick.RemoveAllListeners();
    }
}
