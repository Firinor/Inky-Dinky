using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int Level;
        public int MaxLevel;
        public int Biom;
    }
}

public class YGSaveData : SaveData
{
    private SavesYG saves;

    public override int Level
    {
        get => saves.Level;
        set => saves.Level = value;
    }
    public override int MaxLevel
    {
        get => saves.MaxLevel;
        set => saves.MaxLevel = value;
    }
    public override int Biom
    {
        get => saves.Biom;
        set => saves.Biom = value;
    }
    
    public override void FirstLoad()
    {
        saves = YG2.saves;
    }

    public override void Save()
    {
        YG2.SaveProgress();
    }
    
    public override void ResetProgress()
    {
        Level = 0;
        Biom = 0;
        MaxLevel = 0;
        Save();
    }
}