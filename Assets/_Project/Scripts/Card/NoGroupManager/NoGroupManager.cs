using System.Collections.Generic;
using DBD.BaseGame;
using DG.Tweening;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoGroupManager : BaseMonoBehaviour, IPointerClickHandler
{
    [SerializeField, GetInParent]
    private CardSpawner cardSpawner;
    [SerializeField, Get]
    private Image image;
    [SerializeField, GetInChildren, Name("ParentPosCard")]
    private RectTransform parentPosCardRect;
    [SerializeField, GetInChildren, Name("Pos_1", "Pos_2", "Pos_3")]
    private List<RectTransform> posCards = new List<RectTransform>();

    public void SetSize(Vector2 size)
    {
        image.rectTransform.sizeDelta = size;

        SizeObj(parentPosCardRect, image.rectTransform.rect.width, image.rectTransform.rect.height);

        RightCenterParentPosCard();
    }

    private void RightCenterParentPosCard()
    {
        SetRightCenter(parentPosCardRect, 1, 1.25f);
        RightCenterPosCard();
    }

    private void RightCenterPosCard()
    {
        for (int i = 0; i < posCards.Count; i++)
        {
            RectTransform pos = posCards[i];
            SizeObj(pos, parentPosCardRect.rect.width, parentPosCardRect.rect.height);
            SetRightCenter(pos, i, 0.35f);
        }
    }

    public void SetRightCenter(RectTransform rt, int index, float overlapRatio)
    {
        rt.anchorMin = new Vector2(1f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float width = rt.rect.width;
        float x = -width * 0.5f - width * overlapRatio * index;

        rt.anchoredPosition = new Vector2(x, 0f);
    }

    private void SizeObj(RectTransform rectTarget, float width, float height)
    {
        rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public bool isPauseClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemCards.Count > 0)
        {
            foreach (var itemCard in itemCards)
            {
                itemCard.OnOffRaycastTarget(false);
            }
        }

        if (isPauseClick)
        {
            ReplayClick();
            return;
        }
        isPauseClick = true;
        AddItemCardNoGroup();
    }

    public int indexPos = -1;
    public int maxCount = -1;

    public List<ItemCard> itemCards = new();

    private void AddItemCardNoGroup()
    {
        var cards = cardSpawner.NoGroupItemCards;
        if (cards.Count == 0) return;

        maxCount = maxCount == -1 ? cards.Count - 1 : maxCount;

        indexPos = Mathf.Min(indexPos + 1, cards.Count - 1);

        ItemCard card = cards[indexPos];
        ActiveCard(card);

        int slot = Mathf.Min(indexPos, posCards.Count - 1);
        card.slotIndex = slot;

        MoveCard(card, posCards[slot], 0.3f, () =>
        {
            if (indexPos >= posCards.Count)
                MoveRight();

            isPauseClick = indexPos == maxCount;

            if (!itemCards.Contains(card))
            {
                itemCards.Add(card);
                itemCards[^1].OnOffRaycastTarget(true);
            }
        });
    }

    private void MoveRight()
    {
        foreach (var card in itemCards)
        {
            card.slotIndex = Mathf.Max(0, card.slotIndex - 1);
            MoveCard(card, posCards[card.slotIndex], 0.15f);
        }

        isPauseClick = indexPos == maxCount;
    }

    private void ReplayClick()
    {
        if (indexPos != maxCount) return;

        indexPos = -1;

        foreach (var card in itemCards)
            card.MoveNoGroup();

        itemCards.Clear();
        isPauseClick = false;
    }

    private void ActiveCard(ItemCard card)
    {
        card.gameObject.SetActive(true);
        card.transform.SetAsLastSibling();
    }

    private void MoveCard(ItemCard card, RectTransform target, float time, TweenCallback onComplete = null)
    {
        var tween = card.rect.DOMove(target.position, time).SetEase(Ease.OutCubic);
        if (onComplete != null)
            tween.OnComplete(onComplete);
    }

     public ItemCard ItemCardNoGroup()
    {
        return itemCards[^1]; // noGroupItemCards[noGroupItemCards.Count - 1]
    }



    /*  public int indexPos = -1;
     public int maxCount = -1;

     public List<ItemCard> itemCards = new List<ItemCard>();
     private void AddItemCardNoGroup()
     {
         List<ItemCard> cards = cardSpawner.NoGroupItemCards;
         if (maxCount == -1)
         {
             maxCount = cards.Count - 1;
         }

         if (cards.Count == 0) return;

         indexPos += 1;
         // clamp để tránh out of range
         indexPos = Mathf.Min(indexPos, cards.Count - 1);

         ItemCard card = cards[indexPos];
         card.gameObject.SetActive(true);
         card.transform.SetAsLastSibling();

         int posIndexNew = Mathf.Min(indexPos, posCards.Count - 1);
         card.slotIndex = posIndexNew;
         RectTransform targetPos = posCards[posIndexNew];

         card.rect
             .DOMove(targetPos.position, 0.3f)
             .SetEase(Ease.OutCubic)
             .OnComplete(() =>
             {
                 if (indexPos >= posCards.Count)
                 {
                     MoveRight();
                     if (indexPos == maxCount)
                     {
                         isPauseClick = true;
                     }
                 }
                 else
                 {
                     isPauseClick = false;
                 }
                 if (!itemCards.Contains(card))
                 {
                     itemCards.Add(card);
                 }
             });
     }

     private void MoveRight()
     {
         for (int i = 0; i < itemCards.Count; i++)
         {
             ItemCard itemCard = itemCards[i];
             if (itemCard.slotIndex > 0)
             {
                 itemCard.slotIndex -= 1;
             }
             RectTransform targetPos = posCards[itemCard.slotIndex];
             itemCard.rect
           .DOMove(targetPos.position, 0.15f)
           .SetEase(Ease.OutCubic);
             if (i == itemCards.Count - 1)
             {
                 if (indexPos != maxCount)
                 {
                     isPauseClick = false;
                 }
                 else if (indexPos == maxCount)
                 {
                     isPauseClick = true;
                 }
             }
         }
     }

     private void ReplayClick()
     {
         if (indexPos != maxCount) return;
         indexPos = -1;
         for (int i = 0; i < itemCards.Count; i++)
         {
             itemCards[i].MoveNoGroup();
             if (i == itemCards.Count - 1)
             {
                 itemCards.Clear();
                 isPauseClick = false;
             }
         }
     } */

}