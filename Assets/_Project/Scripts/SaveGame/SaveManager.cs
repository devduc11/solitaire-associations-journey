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

    #endregion
    protected override void Migrate(int fromVersion)
    {
       
    }

    protected override int Version()
    {
        return 0;
    }
}