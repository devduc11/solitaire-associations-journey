using System;
using System.Collections.Generic;
using DBD.BaseGame;
using DG.Tweening;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoGroupManager : BaseMonoBehaviour, IPointerClickHandler
{
    private static NoGroupManager instance;
    public static NoGroupManager Instance => instance;

    [SerializeField, GetInParent]
    private CardSpawner cardSpawner;
    [SerializeField, Get]
    private Image image;
    [SerializeField, GetInChildren, Name("ParentPosCard")]
    private RectTransform parentPosCardRect;
    [SerializeField, GetInChildren, Name("Pos_1", "Pos_2", "Pos_3")]
    private List<RectTransform> posCards = new List<RectTransform>();
    [SerializeField]
    private List<ItemCard> itemCards = new();
    public bool isNoGroupCard;
    private bool isMoveLeft;
    private bool isPauseClick;
    public int indexPos = -1;
    public int maxCount = -1;

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

    protected override void OnEnable()
    {
        base.OnEnable();
        GameAction.OnDownItemGroupCardMove += DownItemGroupCardMove;
        GameAction.OnDragItemGroupCardMove += DragItemGroupCardMove;
        GameAction.OnUpItemGroupCardMove += UpUpItemGroupCardMove;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameAction.OnDownItemGroupCardMove -= DownItemGroupCardMove;
        GameAction.OnDragItemGroupCardMove -= DragItemGroupCardMove;
        GameAction.OnUpItemGroupCardMove -= UpUpItemGroupCardMove;
    }

    private void DownItemGroupCardMove(bool isNoGroupCard)
    {
        this.isNoGroupCard = isNoGroupCard;
        isDrag = false;
    }
    public bool isDrag;
    private void DragItemGroupCardMove()
    {
        isDrag = true;
        if (!isNoGroupCard) return;
        MoveLeft();
    }

    private void UpUpItemGroupCardMove()
    {
        RemoveItemCards();
        if (!isDrag || indexPos <= posCards.Count - 1)
        {
            CheckItemLast();
            return;
        }

        if (!isNoGroupCard) return;
        MoveRight(() =>
        {
            CheckItemLast();
        });
    }

    private void CheckItemLast()
    {
        if (itemCards.Count == 0) return;
        itemCards[^1].OnOffRaycastTarget(true);
        isDrag = false;
    }

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

    private void AddItemCardNoGroup()
    {
        var cards = cardSpawner.NoGroupItemCards;
        if (cards.Count == 0) return;
        isNoGroupCard = false;

        // maxCount = maxCount == -1 ? cards.Count - 1 : maxCount;
        maxCount = cards.Count - 1;

        indexPos = Mathf.Min(indexPos + 1, cards.Count - 1);

        ItemCard card = cards[indexPos];
        ActiveCard(card);

        int slot = Mathf.Min(indexPos, posCards.Count - 1);
        // card.SlotIndex = slot;
        card.SetSlotIndex(slot);

        MoveCard(card, posCards[slot], 0.3f, () =>
        {
            SaveLevelManager.Instance.SaveLevelProgress();
        });

        if (indexPos >= posCards.Count)
            MoveRightClick();

        isPauseClick = indexPos == maxCount;

        if (!itemCards.Contains(card))
        {
            itemCards.Add(card);
            // itemCards[^1].OnOffRaycastTarget(true);
            CheckItemLast();
        }
    }

    private void MoveRightClick()
    {
        foreach (var card in itemCards)
        {
            card.SlotIndex = Mathf.Max(0, card.SlotIndex - 1);
            MoveCard(card, posCards[card.SlotIndex], 0.15f);
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
        SaveLevelManager.Instance.SaveLevelProgress();
    }

    private void MoveRight(Action endAction = null)
    {
        if (!isMoveLeft) return;
        isMoveLeft = false;

        if (indexPos <= posCards.Count - 1) return;

        int startIndex = Mathf.Max(0, indexPos - (posCards.Count - 1));
        for (int i = startIndex; i < itemCards.Count; i++)
        {
            var card = itemCards[i];
            int naturalSlot = Mathf.Clamp(i - startIndex, 0, posCards.Count - 1);
            card.SlotIndex = naturalSlot;
            MoveCard(card, posCards[card.SlotIndex], 0.15f, () =>
            {
                endAction?.Invoke();
            });
        }
    }

    private void MoveLeft()
    {
        if (isMoveLeft) return;
        isMoveLeft = true;

        if (indexPos <= posCards.Count - 1) return;

        int startIndex = Mathf.Max(0, indexPos - (posCards.Count - 1));
        for (int i = startIndex; i < itemCards.Count - 1; i++)
        {
            var card = itemCards[i];
            int naturalSlot = Mathf.Clamp(i - startIndex, 0, posCards.Count - 1);
            int shiftedSlot = ClampSlot(naturalSlot + 1);

            card.SlotIndex = shiftedSlot;
            MoveCard(card, posCards[card.SlotIndex], 0.15f);
        }
    }

    int ClampSlot(int index)
    {
        return Mathf.Clamp(index, 0, posCards.Count - 1);
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
        return itemCards[^1]; // itemCards[itemCards.Count - 1]
    }

    private void RemoveItemCards()
    {
        bool removed = false;

        for (int i = itemCards.Count - 1; i >= 0; i--)
        {
            if (!itemCards[i].IsGroup) continue;

            var removedCard = itemCards[i];
            itemCards.RemoveAt(i);
            cardSpawner.NoGroupItemCards.Remove(removedCard);
            removed = true;
        }

        if (removed)
        {
            indexPos = itemCards.Count - 1;
            maxCount = cardSpawner.NoGroupItemCards.Count - 1;
        }
    }

    public void AddItemCardNoGroupLevelProgress()
    {
        var cards = cardSpawner.NoGroupItemCards;
        maxCount = cards.Count - 1;
        for (int i = 0; i < cards.Count; i++)
        {
            for (int k = 0; k < posCards.Count; k++)
            {
                if (cards[i].SlotIndex == k)
                {
                    indexPos += 1;
                    cards[i].gameObject.SetActive(true);
                    cards[i].rect.position = posCards[k].position;
                    itemCards.Add(cards[i]);
                }
            }
        }

        isPauseClick = indexPos == maxCount;

        CheckItemLast();
    }
}