using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int Level;
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
        Save();
    }
}