using System;
using System.Linq;
using System.Text.RegularExpressions;

[Serializable]
public abstract class SaveData
{
    public abstract int Level { get; set; }
    
    public event Action<int> OnLevelChange;

    public abstract void FirstLoad();
    
    public abstract void ResetProgress();
    public abstract void Save();
    
    protected void InvokeLevelChange(int level)
    {
        OnLevelChange?.Invoke(level);
    }

    public static SaveData GetPlayer()
    {
#if IS_YANDEX
        return new YGSaveData();
#elif IS_MIRRA
        return new MirraSaveData();
#else
        return new PrefsSaveData();
#endif
    }
}