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
    public static WorkerManager WorkerManager;
    
    private void Start()
    {
        Button.onClick.AddListener(TryWork);
    }

    public void TryWork()
    {
        if (transform.GetSiblingIndex() != 0)
            ErrorAnimation.Play();
        else
        {
            Button.onClick.RemoveAllListeners();
            WorkerManager.AddColorBar(this);
        }
    }

    private void OnDestroy()
    {
        Button.onClick.RemoveAllListeners();
    }
}
