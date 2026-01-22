using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCard : BaseDragObject
{
    [SerializeField, GetInChildren, Name("Pos")]
    protected RectTransform posRect;
    [SerializeField]
    private int CardPackageID;
    [SerializeField]
    private ItemCard itemCardSpawner;
    [SerializeField]
    private bool isMergeItemCardSpawner;



    protected override void OnEnable()
    {
        base.OnEnable();
        Debug.Log($"pnad: {posRect.position}");
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isMergeItemCardSpawner) return;
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

        // 🔥 ĐƯA ITEM ĐANG DRAG LÊN TRÊN CÙNG
        transform.SetAsLastSibling();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isMergeItemCardSpawner) return;
        if (!isDragging) return;
        base.OnDrag(eventData);

        if (IsConditionItemCard())
        {
            Debug.Log($"pnad: OnDrag");
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
        if (isMergeItemCardSpawner) return;
        isDragging = false;
        isAnyDragging = false;
        if (itemCardSpawner != null)
        {
            Move();
        }
        else
        {
            EffectUp();
        }
    }

    public void SetSize(Vector2 size)
    {
        image.rectTransform.sizeDelta = size;
        SetSizeBoxCol2D();
    }

    #region Item Card Merge Together

    private void Move()
    {
        SetIsMergeItemCardSpawner(true);
    }

    private void SetIsMergeItemCardSpawner(bool bl)
    {
        isMergeItemCardSpawner = bl;
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
                && !spawner.isMergeItemCardSpawner)
            {
                itemCardSpawner = spawner;
                return true;
            }
        }

        return false;
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