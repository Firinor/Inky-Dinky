using System;

[Serializable]
public class PrefsSaveData : SaveData
{
    public int level;
    public int maxLevel;
    public int biom;

    public override int MaxLevel
    {
        get => maxLevel;
        set => maxLevel = value;
    }

    public override int Level
    {
        get => level;
        set => level = value;
    }

    public override int Biom
    {
        get => biom;
        set => biom = value;
    }

    public override void FirstLoad()
    {
        var data = SaveLoadSystem<PrefsSaveData>.Load("Player", new ());
        level = data.level;
        maxLevel = data.MaxLevel;
        biom = data.Biom;
    }

    public override void ResetProgress()
    {
        Level = 0;
        Biom = 0;
        MaxLevel = 0;
        Save();
    }

    public override void Save()
    {
        SaveLoadSystem<PrefsSaveData>.Save("Player", this);
    }
}