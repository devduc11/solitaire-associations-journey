using System;
using System.Collections.Generic;
using System.Linq;
using DBD.BaseGame;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemGroupCard : BaseDragObject
{
    [SerializeField]
    private bool isMove;
    [SerializeField]
    private List<ItemCard> itemCards = new List<ItemCard>();
    public List<ItemCard> ItemCards
    {
        get => itemCards;
        set => itemCards = value;
    }

    ItemGroupCard excludeItemGroupCard;
    public ItemGroupCard nearestItemGroupCard;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isAnyDragging)
        {
            return;
        }

        // Nếu dùng nhiều ngón tay thì cũng không cho kéo
        if (Input.touchCount > 1)
        {
            return;
        }

        base.OnPointerDown(eventData);
        SetIsMove(true);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        base.OnDrag(eventData);

        if (IsConditionItemGroupCard())
        {
            // if (isMove && nearestItemGroupCard != null)
            // {
            //     // Debug.Log($"pnad: itemCards{itemCards.Count} nearestItemGroupCard {nearestItemGroupCard.name}");
            //     nearestItemGroupCard.SetSizeGroup(rect.rect.size);
            //     foreach (var lastCard in itemCards)
            //     {
            //         if (!nearestItemGroupCard.ItemCards.Contains(lastCard))
            //         {
            //             nearestItemGroupCard.ItemCards.Add(lastCard);
            //         }
            //     }
            //     nearestItemGroupCard.ResizeParentToStack(0.25f);
            // }

        }
    }

    protected override void Update()
    {
        base.Update();
        if (isDragging && Input.touchCount <= 0)
        {
            OnPointerUp();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        OnPointerUp();
    }

    private void OnPointerUp()
    {
        isDragging = false;
        isAnyDragging = false;
        if (IsConditionItemGroupCard())
        {
            CheckMergeItemCard();
        }
        else
        {
            EffectUp(() =>
            {
                CheckGroupCarMoveOff();
            });
        }
    }

    private void CheckGroupCarMoveOff()
    {
        ActionGroupCardMove(() =>
        {
            GroupCarMoveOff();
        });
    }

    private void GroupCarMoveOff()
    {
        foreach (var itemCard in itemCards)
        {
            itemCard.SetGroupCarMoveOff();
        }
    }

    public void ActionGroupCardMove(Action actionOn = null, Action actionOff = null)
    {
        if (isMove)
        {
            actionOn?.Invoke();
        }
        else
        {
            actionOff?.Invoke();
        }

        GroupCardSpawner.Instance.ActiveItemGroupCardMove(false);
        SetIsMove(false);
        nearestItemGroupCard = null;
    }

    public void SetIsMove(bool bl)
    {
        isMove = bl;
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


    public void AddItemCard(ItemCard itemCard, bool isMove = false, ItemGroupCard itemGroupCard = null)
    {
        itemCards.Add(itemCard);
        TopCenter(isMove);
        if (isMove)
        {
            excludeItemGroupCard = itemGroupCard;
        }
    }

    public void TopCenter(bool isMove = false)
    {
        float overlapRatio = 0.25f;

        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];
            if (!isMove)
            {
                itemCard.SetItemGroupCard(this);
            }
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

    public List<ItemCard> SameCardTypes()
    {
        List<ItemCard> result = new List<ItemCard>();

        if (itemCards.Count == 0)
            return result;

        int targetID = itemCards[itemCards.Count - 1].CardID;

        for (int i = itemCards.Count - 1; i >= 0; i--)
        {
            if (itemCards[i].CardID == targetID)
            {
                result.Add(itemCards[i]);
                RemoveItemCards(itemCards[i]);
            }
            else
                break;
        }

        return result;
    }

    private void RemoveItemCards(ItemCard itemCard)
    {
        itemCards.Remove(itemCard);
    }

    public bool IsConditionItemGroupCard()
    {
        nearestItemGroupCard = null;

        RectTransform selfRect = transform as RectTransform;
        if (selfRect == null) return false;

        // Rect của object đang kéo (world)
        Rect selfWorldRect = GetWorldRect(selfRect);

        foreach (var spawner in GroupCardSpawner.Instance.GroupContainsCards())
        {
            if (spawner == this || spawner == excludeItemGroupCard)
                continue;

            RectTransform targetRect = spawner.transform as RectTransform;
            if (targetRect == null)
                continue;

            Rect targetWorldRect = GetWorldRect(targetRect);

            // 1️⃣ Check overlap theo trục X (BẮT BUỘC)
            if (!IsHorizontalOverlapEnough(selfWorldRect, targetWorldRect, 0.5f))
                continue;

            // 2️⃣ Check có giao nhau theo trục Y
            if (!selfWorldRect.Overlaps(targetWorldRect))
                continue;

            nearestItemGroupCard = spawner;
            return true;
        }

        return false;
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        return new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );
    }

    bool IsHorizontalOverlapEnough(Rect a, Rect b, float requiredRatio)
    {
        float overlapWidth =
            Mathf.Min(a.xMax, b.xMax) -
            Mathf.Max(a.xMin, b.xMin);

        if (overlapWidth <= 0f)
            return false;

        float minWidth = Mathf.Min(a.width, b.width);
        float ratio = overlapWidth / minWidth;

        return ratio >= requiredRatio;
    }

    private void CheckMergeItemCard()
    {
        if (nearestItemGroupCard == null) return;
        if (nearestItemGroupCard.ItemCards.Count == 0)
        {
            EffectUpMerge(nearestItemGroupCard.transform.position, () =>
            {
                CheckGroupCarMoveMerge();
            });
        }
        else if (nearestItemGroupCard.ItemCards.Count > 0 && GetTopItemCardNearest() != null)
        {

            // Debug.Log($"pnad: {GetTopItemCardNearest().name}");
            // Debug.Log($"pnad: {GetTopItemCardMove().name}");
            if (GetTopItemCardNearest().IsSameCard(GetTopItemCardMove()))
            {
                EffectUpMerge(nearestItemGroupCard.transform.position, () =>
                {
                    CheckGroupCarMoveMerge();
                });
            }
            else
            {
                EffectUp(() =>
                {
                    CheckGroupCarMoveOff();
                });
            }

        }
    }

    private void CheckGroupCarMoveMerge()
    {
        ActionGroupCardMove(() =>
        {
            foreach (var itemCard in itemCards)
            {
                itemCard.SetMergeGroupCar(nearestItemGroupCard);
            }
        });
    }

    public void EffectUpMerge(Vector2 pos, Action endAction = null)
    {
        if (!isMove) return;
        float z = transform.position.z; // GIỮ Z HIỆN TẠI

        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMove(
               new Vector3(pos.x, pos.y, z),
               0.2f
        ));

        sequence.OnComplete(() =>
      {
          endAction?.Invoke();
      });
    }

    public ItemCard GetTopItemCardNearest()
    {
        if (nearestItemGroupCard.itemCards.Count == 0) return null;

        return nearestItemGroupCard.itemCards[^1]; // itemCards[itemCards.Count - 1]
    }

    public ItemCard GetTopItemCardMove()
    {
        if (itemCards.Count == 0 || !isMove) return null;

        return itemCards[^1]; // itemCards[itemCards.Count - 1]
    }
}