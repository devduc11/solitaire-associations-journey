using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCard : BaseDragObject
{
    [SerializeField]
    private int cardPackageID;
    [SerializeField]
    private int cardID;
    public int CardID
    {
        get => cardID;
        set => cardID = value;
    }
    [SerializeField]
    private ItemCard itemCardSpawner;
    [SerializeField]
    private ItemGroupCard itemGroupCard;
    public ItemGroupCard ItemGroupCard => itemGroupCard;
    [SerializeField]
    private bool isGroup;
    public bool IsGroup => isGroup;

    public override void OnPointerDown(PointerEventData eventData)
    {
        GameAction.OnMergeItemCard?.Invoke(itemGroupCard);
        GameAction.OnPointerDownItemCard?.Invoke(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        GameAction.OnDragItemCard?.Invoke(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        GameAction.OnPointerUpItemCard?.Invoke(eventData);
    }

    public void SetParentItemCard(Transform transformParent)
    {
        transform.SetParent(transformParent);
    }

    public void SetGroupCar(ItemGroupCard itemGroupCard)
    {
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this);
    }

    public void SetGroupCarMoveOff()
    {
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this);
        OnOffRaycastTarget(true);
    }

    public void SetMergeGroupCar(ItemGroupCard itemGroupCard)
    {
        SetGroupCar(itemGroupCard);
        OnOffRaycastTarget(true);
    }

    public bool IsSameCard(ItemCard other)
    {
        return other != null && cardID == other.cardID;
    }





    /*  public override void OnPointerDown(PointerEventData eventData)
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
     }

     public override void OnDrag(PointerEventData eventData)
     {
         if (!isDragging) return;
         base.OnDrag(eventData);

         if (IsConditionItemCard())
         {
             // Debug.Log($"pnad: OnDrag");
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
     } */

    /*  private void OnPointerUp()
     {
         isDragging = false;
         isAnyDragging = false;
         if (itemCardSpawner != null)
         {
             Merge();
         }
         else
         {
             EffectUp();
         }
     } */

    public void SetSize(Vector2 size)
    {
        image.rectTransform.sizeDelta = size;
    }

    #region Item Card Merge Together

    private void Merge()
    {
        // if (itemCardSpawner.isGroup)
        // {
        //     transform.SetParent(itemCardSpawner.itemGroupCard.transform);
        //     itemCardSpawner.itemGroupCard.ItemCards.Add(this);
        //     itemCardSpawner.itemGroupCard.TopCenter();
        // }
        // else
        // {
        //     GroupCardSpawner.Instance.SpawnItemGroupCard1(itemCardSpawner.transform.position, (itemGroupCard, index) =>
        //     {
        //         var groupTf = itemGroupCard.transform;
        //         itemCardSpawner.transform.SetParent(groupTf);

        //         transform.SetParent(groupTf);

        //         itemGroupCard.ItemCards.AddRange(new[]
        //         {
        //         itemCardSpawner,
        //         this
        //             });

        //         itemGroupCard.name = $"itemGroupCard_{index}_(Id_{cardPackageID})";
        //         itemGroupCard.TopCenter();
        //     });
        // }

    }

    public bool IsConditionItemCard(float extraPadding = 50f)
    {
        itemCardSpawner = null;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            Camera.main, transform.position);

        foreach (var spawner in CardSpawner.Instance.ItemCards)
        {
            if (spawner == this)
                continue;

            RectTransform targetRect = spawner.transform as RectTransform;
            if (targetRect == null)
                continue;

            Rect rect = targetRect.rect;
            rect.xMin -= extraPadding;
            rect.xMax += extraPadding;
            rect.yMin -= extraPadding;
            rect.yMax += extraPadding;

            if (rect.Contains(targetRect.InverseTransformPoint(transform.position))
                && spawner.cardPackageID == cardPackageID)
            {
                itemCardSpawner = spawner;
                return true;
            }
        }

        return false;
    }

    public void OnOffRaycastTarget(bool bl)
    {
        image.raycastTarget = bl;
    }

    public void SetIsGroup(bool bl)
    {
        isGroup = bl;
    }

    public void SetItemGroupCard(ItemGroupCard itemGroupCard)
    {
        // if (this.itemGroupCard != null) return;
        this.itemGroupCard = itemGroupCard;
    }


    // public bool IsConditionItemCard()
    // {
    //     itemCardSpawner = null;

    //     Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
    //         Camera.main, transform.position);

    //     foreach (var spawner in CardSpawner.Instance.ItemCards)
    //     {
    //         if (spawner == this)
    //             continue;

    //         RectTransform targetRect = spawner.transform as RectTransform;

    //         if (RectTransformUtility.RectangleContainsScreenPoint(targetRect, screenPos, Camera.main) && !spawner.isMergeItemCardSpawner)
    //         {
    //             itemCardSpawner = spawner;
    //             return true;
    //         }
    //     }

    //     return false;
    // }


    /*  public bool IsConditionItemCard()
     {
         // itemCardSpawner = null; // reset trước

         // foreach (var spawner in CardSpawner.Instance.ItemCards)
         // {
         //     if (spawner == this)
         //         continue;

         //     if (spawner.col2D.OverlapPoint(transform.position))
         //     {
         //         itemCardSpawner = spawner;
         //         return true;
         //     }
         // }

         return false;
     } */

    #endregion

}