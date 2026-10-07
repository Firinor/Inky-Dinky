using TMPro;
using UnityEngine;
#if Cheats
using UnityEngine.SceneManagement;
#endif
using UnityEngine.UI;

public class SityManager :MonoBehaviour
{
    public Toggle Biom1Toggle;
    public Slider Biom1Slider;
    public TextMeshProUGUI Biome1Text1;
    public TextMeshProUGUI Biome1Text2;
    [Space]
    public Toggle Biom2Toggle;
    public Slider Biom2Slider;
    public TextMeshProUGUI Biome2Text1;
    public TextMeshProUGUI Biome2Text2;
    [Space]
    public Toggle Biom3Toggle;
    public Slider Biom3Slider;
    public TextMeshProUGUI Biome3Text1;
    public TextMeshProUGUI Biome3Text2;
    [Space]
    public Button CheatButton1;
    
    private SaveData player;
    
    public void Initialize(SaveData player)
    {
#if !Cheats
        DestroyImmediate(CheatButton1.gameObject);
#endif
        this.player = player;
        SetBiomData(Biom1Toggle, Biom1Slider, Biome1Text1, Biome1Text2, 0);
        SetBiomData(Biom2Toggle, Biom2Slider, Biome2Text1, Biome2Text2, 1);
        SetBiomData(Biom3Toggle, Biom3Slider, Biome3Text1, Biome3Text2, 2);
    }

    private void SetBiomData(Toggle t, Slider s, TextMeshProUGUI t1, TextMeshProUGUI t2, int biomNumber)
    {
        t.isOn = player.Biom == biomNumber;
        int number = biomNumber;
        t.onValueChanged.AddListener(v =>
        {
            if(!v) return;
            ChangeBiom(number);
        });
        
        float biomprogress;
        if(player.MaxLevel < biomNumber * CONSTANTS.LevelsInBiom)
            biomprogress = -1;
        else if(player.MaxLevel > (biomNumber+1) * CONSTANTS.LevelsInBiom)
            biomprogress = 1;
        else
            biomprogress = (player.MaxLevel - biomNumber * CONSTANTS.LevelsInBiom) / (float)CONSTANTS.LevelsInBiom;
        
        if (biomprogress >= 0)
        {
            s.value = biomprogress;
            t1.text = biomprogress.ToString("P0");
            t2.text = biomprogress.ToString("P0");
        }
        else
        {
            t.gameObject.SetActive(false);
            s.gameObject.SetActive(false);
            t1.gameObject.SetActive(false);
            t2.gameObject.SetActive(false);
        }
    }
    
    private void ChangeBiom(int biomNumber)
    {
        if(player.Biom == biomNumber)
            return;
        
        player.Biom = biomNumber;
        player.Save();
    }

    private void OnDestroy()
    {
        Biom1Toggle.onValueChanged.RemoveAllListeners();
        Biom2Toggle.onValueChanged.RemoveAllListeners();
        Biom3Toggle.onValueChanged.RemoveAllListeners();
    }

#if Cheats
    public void CheatsAddLevel()
    {
        player.MaxLevel++;
        player.Save();
        SceneManager.LoadScene("WorldMapScene");
    }
#endif
}