using System;
using System.Collections.Generic;
using System.Linq;
using DBD.BaseGame;
using DG.Tweening;
using Teo.AutoReference;
using Unity.Properties;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemGroupCard : BaseDragObject
{
    [SerializeField, GetInChildren, Name("Outline")] private Image outline;
    [SerializeField]
    private Vector2 startSizeGroup = Vector2.zero;
    [SerializeField]
    private bool isMove;
    [SerializeField]
    private List<ItemCard> itemCards = new List<ItemCard>();
    public List<ItemCard> ItemCards
    {
        get => itemCards;
        set => itemCards = value;
    }

    public ItemGroupCard excludeItemGroupCard;
    public ItemGroupCard nearestItemGroupCard;
    public ItemGroupMerge nearestItemGroupCardMerge;

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
        GameAction.OnDownItemGroupCardMove?.Invoke(IsNoGroupCardMove());
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        base.OnDrag(eventData);

        IsConditionItemGroupMerge();
        IsConditionItemGroupCard();

        GameAction.OnDragItemGroupCardMove?.Invoke();
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
        if (IsConditionItemGroupMerge())
        {
            if (IsGoldItemCard() && nearestItemGroupCardMerge.CardID == -1)
            {
                CheckItemGroupCardMerge();
            }
            else if (!IsGoldItemCard() && nearestItemGroupCardMerge.CardID != -1)
            {
                bool found = false;

                foreach (var itemCard in itemCards)
                {
                    if (itemCard.CardID == nearestItemGroupCardMerge.CardID)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    CheckItemGroupCardMerge();
                }
                else
                {
                    CheckGroupCard();
                }
            }
            else
            {
                CheckGroupCard();
            }
        }
        else
        {
            CheckGroupCard();
        }
    }

    private void CheckItemGroupCardMerge()
    {
        nearestItemGroupCardMerge.CheckMerge(itemCards, () =>
        {
            // Debug.Log($"pnad: CheckItemGroupCardMerge");
            GroupCardSpawner.Instance.ResetItemGroupCard();
            GameAction.OnUpItemGroupCardMove?.Invoke();
        });
        ResetGroupCardMove();
    }

    private void CheckGroupCard()
    {
        if (IsConditionItemGroupCard())
        {
            CheckMergeItemCard();
        }
        else
        {
            CheckEffectUp();
        }
    }

    public void CheckEffectUp()
    {
        EffectUp(() =>
        {
            if (IsNoGroupCardMove())
            {
                CheckItemNoGroupCardMove();
            }
            else
            {
                CheckGroupCarMoveOff();
            }
            GameAction.OnUpItemGroupCardMove?.Invoke();
        });
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

        ResetGroupCardMove();
        GroupCardSpawner.Instance.ResetItemGroupCard();
    }

    private void CheckItemNoGroupCardMove()
    {
        if (isMove)
        {
            foreach (var itemCard in itemCards)
            {
                itemCard.ItemNoGroup();
            }
        }
        ResetGroupCardMove();
    }

    private void ResetGroupCardMove()
    {
        GroupCardSpawner.Instance.ActiveItemGroupCardMove(false);
        SetIsMove(false);
        excludeItemGroupCard = null;
        nearestItemGroupCard = null;
        nearestItemGroupCardMerge = null;
    }

    private bool IsNoGroupCardMove()
    {
        if (isMove)
        {
            foreach (var itemCard in itemCards)
            {
                if (!itemCard.IsGroup) return true;
            }
        }
        return false;
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
        SetSizePos(size, transform.position);
        image.pixelsPerUnitMultiplier = temp / image.rectTransform.rect.width;

        if (outline.pixelsPerUnitMultiplier != 1) return;
        outline.pixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier;
    }

    public void OnOffRaycastTargetItemGroup(bool bl)
    {
        image.raycastTarget = bl;
        ShowOutline(bl);
    }

    public void AddItemCard(ItemCard itemCard, bool isMove = false, ItemGroupCard itemGroupCard = null, bool isSpawn = false, bool isGroup = false)
    {
        itemCards.Add(itemCard);

        if (!isGroup && isMove)
        {
            // Debug.Log($"pnad: NoGroup");
            TopCenterNoGroup();
            return;
        }

        TopCenter(isMove, isSpawn);
        if (isMove)
        {
            excludeItemGroupCard = itemGroupCard;
        }

        CheckAllItemCardOnOffRaycastTarget();
    }

    float overlapRatio = 0.25f;
    public void TopCenter(bool isMove = false, bool isSpawn = false)
    {
        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];
            if (!isMove)
            {
                itemCard.SetItemGroupCard(this);
            }
            SetTopCenter(itemCard.rect, i, overlapRatio, isSpawn: isSpawn);
        }

        ResizeParentToStack(overlapRatio);
    }

    public void SetTopCenter(RectTransform rt, int index, float overlapRatio, float duration = 0.25f, bool isSpawn = false)
    {
        rt.parent.SetAsLastSibling(); //ĐƯA ITEM LÊN TRÊN CÙNG
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float height = rt.rect.height;
        float y = -height * 0.5f - height * overlapRatio * index;

        // Kill tween cũ để tránh giật
        rt.DOKill();

        // Tween vị trí
        // rt.DOAnchorPos(new Vector2(0f, y), duration)
        //   .SetEase(Ease.OutQuad);

        if (isSpawn)
        {
            float startY = y + 200;

            rt.anchoredPosition = new Vector2(0f, startY);

            rt.DOAnchorPosY(y, duration)
              .SetEase(Ease.OutQuad);
        }
        else
        {
            rt.DOAnchorPos(new Vector2(0f, y), duration)
            .SetEase(Ease.OutQuad);
        }
    }

    public void TopCenterNoGroup()
    {
        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];
            SetTopCenterNoGroup(itemCard.rect, i, overlapRatio);
        }

        ResizeParentToStack(overlapRatio);
    }

    public static void SetTopCenterNoGroup(RectTransform rt, int index, float overlapRatio)
    {
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float height = rt.rect.height;
        float y = -height * 0.5f - height * overlapRatio * index;

        rt.anchoredPosition = new Vector2(0f, y);
    }

    // public static void SetTopCenter(RectTransform rt, int index, float overlapRatio)
    // {
    //     rt.anchorMin = new Vector2(0.5f, 1f);
    //     rt.anchorMax = new Vector2(0.5f, 1f);
    //     rt.pivot = new Vector2(0.5f, 0.5f);

    //     float height = rt.rect.height;
    //     float y = -height * 0.5f - height * overlapRatio * index;

    //     rt.anchoredPosition = new Vector2(0f, y);
    // }

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

    public List<ItemCard> SameCardTypes(bool isGold)
    {
        List<ItemCard> result = new();

        if (itemCards.Count == 0)
            return result;

        // Trường hợp Gold: chỉ lấy 1 lá trên cùng
        if (isGold)
        {
            ItemCard top = itemCards[^1];
            result.Add(top);
            RemoveItemCards(top);
            return result;
        }

        int lastIndex = itemCards.Count - 1;
        int targetID = itemCards[lastIndex].CardID;

        // Duyệt từ trên xuống
        for (int i = lastIndex; i >= 0; i--)
        {
            ItemCard card = itemCards[i];

            // Nếu khác ID thì dừng
            if (card.CardID != targetID) break;

            if (card.IsGold)
            {
                if (i == lastIndex)
                {
                    // Thẻ Gold ở trên cùng thì lấy và cho phép xét tiếp bên dưới
                    result.Add(card);
                }
                else
                {
                    // Thẻ Gold ở giữa hoặc dưới cùng thì dừng chuỗi tại đây
                    break;
                }
            }
            else
            {
                // Thẻ thường cùng ID thì lấy
                result.Add(card);
            }
        }

        // Remove riêng để tránh lỗi khi loop
        foreach (var card in result)
            RemoveItemCards(card);

        return result;
    }
    /*  public List<ItemCard> SameCardTypes(bool isGold)
     {
         List<ItemCard> result = new();

         if (itemCards.Count == 0)
             return result;

         // Trường hợp Gold: chỉ lấy 1 lá trên cùng
         if (isGold)
         {
             ItemCard top = itemCards[^1];
             result.Add(top);
             RemoveItemCards(top);
             return result;
         }

         int targetID = itemCards[^1].CardID;

         // Duyệt từ trên xuống, nhưng chỉ REMOVE sau
         for (int i = itemCards.Count - 1; i >= 0; i--)
         {
             if (itemCards[i].CardID != targetID)
                 break;

             result.Add(itemCards[i]);
         }

         // Remove riêng để tránh lỗi khi loop
         foreach (var card in result)
             RemoveItemCards(card);

         return result;
     } */

    /*  public List<ItemCard> SameCardTypes()
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
     } */

    private void RemoveItemCards(ItemCard itemCard)
    {
        itemCards.Remove(itemCard);
        CheckAllItemCardOnOffRaycastTarget();
    }

    public bool IsConditionItemGroupCard()
    {
        nearestItemGroupCard = null;

        RectTransform selfRect = transform as RectTransform;
        if (selfRect == null) return false;

        Rect selfWorldRect = GetWorldRect(selfRect);

        float maxOverlapArea = 0f;

        foreach (var spawner in GroupCardSpawner.Instance.GroupContainsCards())
        {
            if (spawner == this || spawner == excludeItemGroupCard)
                continue;

            RectTransform targetRect = spawner.transform as RectTransform;
            if (targetRect == null)
                continue;

            Rect targetWorldRect = GetWorldRect(targetRect);

            // Tính overlap X
            float overlapWidth =
                Mathf.Min(selfWorldRect.xMax, targetWorldRect.xMax) -
                Mathf.Max(selfWorldRect.xMin, targetWorldRect.xMin);

            if (overlapWidth <= 0f)
                continue;

            // Tính overlap Y
            float overlapHeight =
                Mathf.Min(selfWorldRect.yMax, targetWorldRect.yMax) -
                Mathf.Max(selfWorldRect.yMin, targetWorldRect.yMin);

            if (overlapHeight <= 0f)
                continue;

            // Diện tích overlap
            float overlapArea = overlapWidth * overlapHeight;

            // Lấy cái đè nhiều nhất
            if (overlapArea > maxOverlapArea)
            {
                maxOverlapArea = overlapArea;
                nearestItemGroupCard = spawner;
            }
        }

        return nearestItemGroupCard != null;
    }

    /* public bool IsConditionItemGroupCard()
    {
        nearestItemGroupCard = null;

        RectTransform selfRect = transform as RectTransform;
        if (selfRect == null) return false;

        Rect selfWorldRect = GetWorldRect(selfRect);

        foreach (var spawner in GroupCardSpawner.Instance.GroupContainsCards())
        {
            if (spawner == this || spawner == excludeItemGroupCard)
                continue;

            RectTransform targetRect = spawner.transform as RectTransform;
            if (targetRect == null)
                continue;

            Rect targetWorldRect = GetWorldRect(targetRect);

            // 1️⃣ Overlap ngang (BẮT BUỘC)
            if (!IsHorizontalOverlapEnough(selfWorldRect, targetWorldRect, 0.5f))
                continue;

            // 2️⃣ Overlap dọc (BẮT BUỘC)
            if (!IsVerticalOverlapEnough(selfWorldRect, targetWorldRect, 0.5f))
                continue;

            nearestItemGroupCard = spawner;
            return true;
        }

        return false;
    } */

    public bool IsConditionItemGroupMerge()
    {
        // Debug.Log($"pnad: {isGoldItemCard}");
        nearestItemGroupCardMerge = null;

        RectTransform selfRect = transform as RectTransform;
        if (selfRect == null) return false;

        // Rect của object đang kéo (world)
        Rect selfWorldRect = GetWorldRect(selfRect);

        foreach (var spawner in GroupMergeSpawner.Instance.ItemGroupMerges)
        {
            // if (!isGoldItemCard) return false;
            // if (spawner == this || spawner == excludeItemGroupCard)
            //     continue;

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

            nearestItemGroupCardMerge = spawner;
            return true;
        }

        return false;
    }

    private bool IsGoldItemCard()
    {
        if (isMove)
        {
            foreach (var itemCard in itemCards)
            {
                if (!itemCard.IsGold) return false;
            }
        }

        return true;
    }

    /* public bool IsConditionItemGroupCard()
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
   } */

    /*  Rect GetWorldRect(RectTransform rt)
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
     } */

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

    bool IsVerticalOverlapEnough(Rect a, Rect b, float requiredRatio)
    {
        float overlapHeight =
            Mathf.Min(a.yMax, b.yMax) -
            Mathf.Max(a.yMin, b.yMin);

        if (overlapHeight <= 0f)
            return false;

        float minHeight = Mathf.Min(a.height, b.height);
        float ratio = overlapHeight / minHeight;

        return ratio >= requiredRatio;
    }


    private void CheckMergeItemCard()
    {
        if (nearestItemGroupCard == null) return;
        if (nearestItemGroupCard.ItemCards.Count == 0)
        {
            CheckGroupCarMoveMerge();
        }
        else if (nearestItemGroupCard.ItemCards.Count > 0 && GetTopItemCardNearest() != null)
        {
            // Debug.Log($"pnad: {GetTopItemCardNearest().name}");
            // Debug.Log($"pnad: {GetTopItemCardMove().name}");
            if (GetTopItemCardNearest().IsSameCard(GetTopItemCardMove()))
            {
                CheckGroupCarMoveMerge();
            }
            else
            {
                CheckEffectUp();
            }

        }
    }

    private void CheckGroupCarMoveMerge()
    {
        if (IsNoGroupCardMove())
        {
            if (isMove)
            {
                foreach (var itemCard in itemCards)
                {
                    itemCard.SetMergeNoGroupCar(nearestItemGroupCard);
                }
                ResetGroupCardMove();
                GroupCardSpawner.Instance.ResetItemGroupCard();
            }
        }
        else
        {
            ActionGroupCardMove(() =>
            {
                foreach (var itemCard in itemCards)
                {
                    itemCard.SetMergeGroupCar(nearestItemGroupCard);
                }
            });
        }

        GameAction.OnUpItemGroupCardMove?.Invoke();
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

    public void SetSizePos(Vector2 size, Vector2 pos)
    {
        startSizeGroup = size;
        startPosition = pos;
    }

    public void ResetSizePos()
    {
        if (!isMove && itemCards.Count == 0)
        {
            image.rectTransform.sizeDelta = startSizeGroup;
            transform.position = startPosition;
        }
        else
        {
            ResizeParentToStack(overlapRatio);
        }
    }

    public void ShowOutline(bool bl)
    {
        outline.gameObject.SetActive(bl);
    }

    private void CheckAllItemCardOnOffRaycastTarget()
    {
        for (int i = 0; i < itemCards.Count; i++)
        {
            itemCards[i].OnOffRaycastTarget(false);
        }

        if (itemCards.Count == 0 || isMove) return;

        int lastIndex = itemCards.Count - 1;
        int targetID = itemCards[lastIndex].CardID;

        for (int i = lastIndex; i >= 0; i--)
        {
            ItemCard card = itemCards[i];

            if (card.CardID != targetID) break;

            if (card.IsGold)
            {
                if (i == lastIndex)
                {
                    card.OnOffRaycastTarget(true);
                }
                else
                {
                    break;
                }
            }
            else
            {
                card.OnOffRaycastTarget(true);
            }
        }
    }
}