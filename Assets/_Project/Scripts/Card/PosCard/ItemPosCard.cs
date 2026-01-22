using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.UI;

public class ItemPosCard : BaseMonoBehaviour
{
    [SerializeField, Get] private RectTransform rect;
   

    public RectTransform SizeImg()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        return rect;
    }

    public RectTransform Pos()
    {
        return rect;
    }
   
}
