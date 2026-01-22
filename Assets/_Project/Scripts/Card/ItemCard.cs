using DBD.BaseGame;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCard : BaseDragObject
{
    [SerializeField]
    private int CardPackageID;
    // [SerializeField]
    // private int indexPos;


    protected override void OnEnable()
    {
        base.OnEnable();
    }

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
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        base.OnDrag(eventData);
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
        EffectUp();
    }

    public void SetSize(Vector2 size)
    {
        image.rectTransform.sizeDelta = size;
        SetSizeBoxCol2D();
    }

    // public void SetIndexPos(int indexPos)
    // {
    //     this.indexPos = indexPos;
    // }

}