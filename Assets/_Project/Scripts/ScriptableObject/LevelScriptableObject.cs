using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelData",
    menuName = "ScriptableObject/Level",
    order = 1
)]
public class LevelScriptableObject : ScriptableObject
{
    public int Moves;

    // Danh sách các gói bài trong level
    public List<CardPackageData> Packages;

    // Danh sách cột
    public List<int> Columns;
}

[System.Serializable]
public class CardPackageData
{
    public int NormalCardCount; // số thẻ thường
    // public int GoldCardCount;   // số thẻ vàng
}



