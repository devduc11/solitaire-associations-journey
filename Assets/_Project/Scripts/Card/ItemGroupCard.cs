using System.Collections.Generic;
using System.Linq;
using DBD.BaseGame;
using DG.Tweening;
using UnityEngine;

public class ItemGroupCard : BaseDragObject
{
    [SerializeField]
    private List<ItemCard> itemCards = new List<ItemCard>();
    public List<ItemCard> ItemCards
    {
        get => itemCards;
        set => itemCards = value;
    }

    public void SetSizeGroup(Vector2 size)
    {
        // Debug.Log($"pnad: {image.rectTransform.sizeDelta}");
        float temp = image.rectTransform.rect.width;
        image.rectTransform.sizeDelta = size;
        image.pixelsPerUnitMultiplier = temp / image.rectTransform.rect.width;
    }

    public void OnOffRaycastTarget(bool bl)
    {
        image.raycastTarget = bl;
    }

    public void AddItemCard(ItemCard itemCard)
    {
        itemCards.Add(itemCard);
        TopCenter();
    }

    public void TopCenter()
    {
        float overlapRatio = 0.25f;

        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];
            // itemCard.SetIsGroup(true);
            // itemCard.OnOffRaycastTarget(false);
            itemCard.SetItemGroupCard(this);
            SetTopCenter(itemCard.rect, i, overlapRatio);
        }

        ResizeParentToStack(overlapRatio);
    }

    public static void SetTopCenter(RectTransform rt, int index, float overlapRatio)
    {
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float height = rt.rect.height;
        float y = -height * 0.5f - height * overlapRatio * index;

        rt.anchoredPosition = new Vector2(0f, y);
    }

    void ResizeParentToStack(float overlapRatio)
    {
        if (itemCards.Count == 0) return;

        RectTransform parent = itemCards[0].rect.parent as RectTransform;
        if (parent == null) return;

        float cardHeight = itemCards[0].rect.rect.height;
        int count = itemCards.Count;

        float stackHeight =
            cardHeight +
            cardHeight * overlapRatio * (count - 1);

        // 1️⃣ Lưu đỉnh trên (world)
        float topBefore =
            parent.position.y +
            parent.rect.height * (1f - parent.pivot.y) * parent.lossyScale.y;

        // 2️⃣ Resize
        parent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            stackHeight
        );

        // 3️⃣ Bù vị trí
        float topAfter =
            parent.position.y +
            parent.rect.height * (1f - parent.pivot.y) * parent.lossyScale.y;

        float delta = topBefore - topAfter;
        parent.position += new Vector3(0f, delta, 0f);
    }

    public List<ItemCard> SameCardType()
    {
        List<ItemCard> result = new List<ItemCard>();

        if (itemCards.Count == 0)
            return result;

        int targetID = itemCards[0].CardID;

        for (int i = 0; i < itemCards.Count; i++)
        {
            if (itemCards[i].CardID == targetID)
                result.Add(itemCards[i]);
        }

        return result;
    }


}