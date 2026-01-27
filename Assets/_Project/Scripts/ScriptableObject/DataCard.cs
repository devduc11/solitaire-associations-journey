using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataCard", menuName = "ScriptableObject/DataCard", order = 1)]
public class DataCard : ScriptableObject
{
    public List<CardPackage> CardPackages = new List<CardPackage>();
}

[System.Serializable]
public class CardPackage
{
    public string NameCardPackage;
    public int IDCardPackage;
    public List<Sprite> SpriteCardType;
    public List<string> NameType;
}

