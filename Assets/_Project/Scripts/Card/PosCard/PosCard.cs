using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class PosCard : BaseMonoBehaviour
{
    private static PosCard instance;
    public static PosCard Instance => instance;
    [SerializeField, Get] public RectTransform rect;
    [SerializeField, GetInChildren]
    private List<ItemPosCard> itemPosCards = new List<ItemPosCard>();
    // public List<ItemPosCard> ItemPosCards => itemPosCards;

    #region LoadComponents

    protected override void LoadComponents()
    {
        base.LoadComponents();

        // for (int i = 0; i < itemPosCards.Count; i++)
        // {
        //     itemPosCards[i].name = $"ItemPosCard_{i}";
        // }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public RectTransform SizeImgItemPosCard()
    {
        return itemPosCards[0].SizeImg();
    }

    public RectTransform PosItemPosCard(int indexPos)
    {
        return itemPosCards[indexPos].Pos();
    }

    public void _Test()
    {
        SetSizeObj();
    }

    private void SetSizeObj()
    {
        SizeObj(rect, rect.rect.width, SizeImgItemPosCard().rect.height);
    }

    private void SizeObj(RectTransform rectTarget, float width, float height)
    {
        rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

}