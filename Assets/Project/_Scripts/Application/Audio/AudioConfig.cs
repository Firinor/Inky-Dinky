using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Audio")]
public class AudioConfig : ScriptableObject
{
    [Header("Buttons")] 
    public ClipSettings ButtonClick;
    public ClipSettings OpenScroll;
    public ClipSettings CloseScroll;
    public ClipSettings HelpSpell;
    [Header("Pixels")] 
    public ClipSettings BarError;
    public ClipSettings BarPick;
    public ClipSettings BarDespawn;
    public ClipSettings WorkerSpawn;
    public ClipSettings WorkerEat;
    [Header("Win")] 
    public ClipSettings Win;
    public ClipSettings Lose;
    public ClipSettings Unlock;
}