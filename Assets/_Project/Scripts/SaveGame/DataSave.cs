using System.Collections.Generic;
using DBD.BaseGame;
using DBD.SaveGame;
using UnityEngine;

[System.Serializable]
public partial class DataSave : BaseDataSave
{

    [SerializeField] private SaveValue<bool> isSfxOn = new(true);
    [SerializeField] private SaveValue<bool> isMusicOn = new(true);
    [SerializeField] private SaveValue<bool> isVibrateOn = new(true);

    [Header("---SaveLevel---")]
    [SerializeField] private SaveList<SaveItemCard> saveItemCards = new(new List<SaveItemCard>());
}