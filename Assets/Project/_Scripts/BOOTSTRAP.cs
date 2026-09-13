using UnityEngine;

public class BOOTSTRAP : MonoBehaviour
{
    public MainImageManager MainImageManager;
    public ColorJarsManager ColorJarsManager;

    //private Player player;
    
    private void Awake()
    {
        //LoadPlayer();
        MainImageManager.Initialize();
        ColorJarsManager.Initialize();
    }
}