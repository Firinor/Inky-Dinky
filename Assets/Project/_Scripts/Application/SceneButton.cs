using FirAnimations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string SceneName;

    public FirAnimation closeCurtain;    
    
    public void SwitchToScene()
    {
        if(closeCurtain == null)
            SceneManager.LoadScene(SceneName);
        else
        {
            closeCurtain.OnComplete = null;
            closeCurtain.OnComplete = () => { SceneManager.LoadScene(SceneName);};
            closeCurtain.Play();
        }
    }
}
