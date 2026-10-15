using UnityEngine;
#if IS_YANDEX
using YG;
#endif

public class DestroyADS : MonoBehaviour
{
    public void DoDestroy()
    {
#if IS_YANDEX
        YG2.RewardedAdvShow("OpenWorkerPlace", OnEndAdvShow);
#else
         OnEndAdvShow()
#endif
    }

    private void OnEndAdvShow()
    {
        Destroy(gameObject);
    }
}
