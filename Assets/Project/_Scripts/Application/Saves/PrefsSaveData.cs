using System;

[Serializable]
public class PrefsSaveData : SaveData
{
    public int level;
    
    public override int Level
    {
        get => level;
        set => level = value;
    }
    public override void FirstLoad()
    {
        var data = SaveLoadSystem<PrefsSaveData>.Load("Player", new ());
        level = data.level;
    }

    public override void ResetProgress()
    {
        Level = 0;
        Save();
    }

    public override void Save()
    {
        SaveLoadSystem<PrefsSaveData>.Save("Player", this);
    }
}