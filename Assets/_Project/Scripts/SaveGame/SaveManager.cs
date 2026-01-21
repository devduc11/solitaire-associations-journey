using DBD.BaseGame;
using DBD.SaveGame;
using UnityEngine;

public class SaveManager : BaseSaveManager<SaveManager, DataSave>
{
    [ContextMenu("Clear Data")]
    public override void ClearData()
    {
        base.ClearData();
    }

    [ContextMenu("Save Data")]
    public override void SaveData()
    {
        base.SaveData();
    }

    #region Music

    public bool Music
    {
        get => DataSave.IsMusicOff;
        set { DataSave.IsMusicOff = value; }
    }

    public bool Sfx
    {
        get => DataSave.IsSfxOff;
        set { DataSave.IsSfxOff = value; }
    }

    public bool Vibrate
    {
        get => DataSave.IsVibrateOff;
        set { DataSave.IsVibrateOff = value; }
    }

    #endregion
    protected override void Migrate(int fromVersion)
    {
       
    }

    protected override int Version()
    {
        return 0;
    }
}