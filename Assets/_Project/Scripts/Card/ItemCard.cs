using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCard : BaseDragObject
{
    [SerializeField]
    private bool isGold;
    public bool IsGold
    {
        get => isGold;
        set => isGold = value;
    }

    [SerializeField]
    private int cardID;
    public int CardID
    {
        get => cardID;
        set => cardID = value;
    }

    [SerializeField]
    private ItemGroupCard itemGroupCard;
    public ItemGroupCard ItemGroupCard => itemGroupCard;

    [SerializeField, GetInChildren, Name("SpriteCardObj")]
    private GameObject spriteCardObj;
    [SerializeField, GetInChildren, Name("NameTypeObj")]
    private GameObject nameTypeObj;

    public override void OnPointerDown(PointerEventData eventData)
    {
        GameAction.OnMergeItemCard?.Invoke(itemGroupCard, this);
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
        return other != null && cardID == other.cardID && !isGold;
    }

    public void SetSize(Vector2 size)
    {
        image.rectTransform.sizeDelta = size;
    }

    public void OnOffRaycastTarget(bool bl)
    {
        image.raycastTarget = bl;
    }

    public void SetItemGroupCard(ItemGroupCard itemGroupCard)
    {
        // if (this.itemGroupCard != null) return;
        this.itemGroupCard = itemGroupCard;
    }

    public void ShowSpriteOrName(int index)
    {
        spriteCardObj.SetActive(index == 0);
        nameTypeObj.SetActive(index == 1);
    }

}