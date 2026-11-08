using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class ReviewYG : MonoBehaviour
{
    public Button reviewButton;
    void Start()
    {
#if IS_YANDEX
        YG2.onReviewSent += ReviewSent;
        
        if (YG2.reviewCanShow)
            reviewButton.onClick.AddListener(YG2.ReviewShow);
        else
            Destroy(reviewButton);
#else
        Destroy(reviewButton);        
#endif
    }

    private void ReviewSent(bool sent)
    {
        if (sent)
            Destroy(reviewButton);
    }
    
    private void OnDestroy()
    {
        reviewButton?.onClick.RemoveAllListeners();
#if IS_YANDEX
        YG2.onReviewSent -= ReviewSent;
#endif
    }
}
