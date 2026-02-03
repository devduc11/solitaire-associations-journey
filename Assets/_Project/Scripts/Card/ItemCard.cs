using System;
using System.Collections.Generic;
using DBD.BaseGame;
using DG.Tweening;
using Teo.AutoReference;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private bool isGroup;
    public bool IsGroup
    {
        get => isGroup;
        set => isGroup = value;
    }
    [SerializeField]
    private bool isGroupMerge;
    public bool IsGroupMerge
    {
        get => isGroupMerge;
        set => isGroupMerge = value;
    }
    [SerializeField]
    private int cardID;
    public int CardID
    {
        get => cardID;
        set => cardID = value;
    }

    [SerializeField]
    private int slotIndex;
    public int SlotIndex
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    [SerializeField]
    private int target;
    public int Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField]
    private int idTypeCard;
    public int IDTypeCard
    {
        get => idTypeCard;
        set => idTypeCard = value;
    }

    [SerializeField]
    private int indexGroup;
    public int IndexGroup
    {
        get => indexGroup;
        set => indexGroup = value;
    }

    [SerializeField]
    private int indexGroupMerge;
    public int IndexGroupMerge
    {
        get => indexGroupMerge;
        set => indexGroupMerge = value;
    }
    [SerializeField]
    private int indexSpriteOrName;
    public int IndexSpriteOrName
    {
        get => indexSpriteOrName;
        set => indexSpriteOrName = value;
    }

    [SerializeField]
    private ItemGroupCard itemGroupCard;
    public ItemGroupCard ItemGroupCard => itemGroupCard;

    [SerializeField, GetInChildren, Name("SpriteCardObj")]
    private GameObject spriteCardObj;
    [SerializeField, GetInChildren, Name("NameTypeObj")]
    private GameObject nameTypeObj;
    [SerializeField, GetInChildren, Name("SpriteCardTypeS", "SpriteCardTypeL")]
    private List<Image> spriteCards = new List<Image>();
    public List<Image> SpriteCards => spriteCards;
    [SerializeField, GetInChildren, Name("NameTypeS", "NameTypeL")]
    private List<TextMeshProUGUI> nameTypes = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> NameTypes => nameTypes;

    [SerializeField]
    private Transform parentNoGroup;

    [SerializeField, GetInChildren, Name("TargetText")]
    private TextMeshProUGUI targetText;


    public override void OnPointerDown(PointerEventData eventData)
    {

        if (!isGroup)
        {
            parentNoGroup = transform.parent;
        }
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

    public void SetGroupCarSpawn(ItemGroupCard itemGroupCard)
    {
        SetSlotIndex(-1);
        SetIsGroup(true);
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this, isSpawn: true, isGroup: isGroup);
        SetIndexGroup(itemGroupCard.IndexGroup);
        SetIndexGroupMerge(-1);
    }

    public void SetGroupCarSpawnLevelProgress(ItemGroupCard itemGroupCard)
    {
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this, isSpawn: true, isGroup: isGroup);
    }

    public void SetGroupCarMoveOff()
    {
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this);
        SetIndexGroup(itemGroupCard.IndexGroup);
    }

    public void SetMergeGroupCard(ItemGroupCard itemGroupCard)
    {
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this, isGroup: isGroup);
        SetIndexGroup(itemGroupCard.IndexGroup);
    }

    public void SetMergeNoGroupCard(ItemGroupCard itemGroupCard)
    {
        SetIsGroup(true);
        SetParentItemCard(itemGroupCard.transform);
        itemGroupCard.AddItemCard(this, isGroup: isGroup);
        SetIndexGroup(itemGroupCard.IndexGroup);
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
        indexSpriteOrName = index;
        spriteCardObj.SetActive(index == 0);
        nameTypeObj.SetActive(index == 1);
    }

    public void SetSpriteCards(Sprite sprite)
    {
        foreach (var spriteCard in spriteCards)
        {
            spriteCard.sprite = sprite;
            // spriteCard.SetNativeSize();
        }
    }

    public void SetNameTypes(string name)
    {
        foreach (var nameType in nameTypes)
        {
            nameType.text = name;
        }
    }

    public void SetIdTypeCard(int index)
    {
        idTypeCard = index;
    }

    public void SetIsGold(bool bl)
    {
        isGold = bl;
        ShowTargetText(bl);
    }

    public void ShowTargetText(bool bl)
    {
        targetText.gameObject.SetActive(bl);
    }

    public void SetTarget(int index)
    {
        targetText.text = $"0/{index}";
        target = index;
    }

    public void SetIsGroup(bool bl)
    {
        isGroup = bl;
    }

    public void MoveNoGroup(Action endAction = null)
    {
        SetSlotIndex(-1);
        transform.DOMove(startPosition, 0.15f)
           .SetEase(Ease.OutCubic)
           .OnComplete(() =>
           {
               gameObject.SetActive(false);
               endAction?.Invoke();
           });
    }

    public void ItemNoGroup()
    {
        if (!isGroup)
        {
            SetParentItemCard(parentNoGroup);
        }
    }

    public void ResetItem()
    {
        SetIsGold(false);
        SetIsGroup(false);
        SetIsGroupMerge(false);
        SetTarget(0);
        SetIndexGroup(-1);
        SetIndexGroupMerge(-1);
    }

    public void SetPos()
    {
        startPosition = transform.position;
    }

    public void SetStatus(CardPackage cardPackage, int spriteOrName, bool isGold, int target = 0, Vector2 size = default)
    {
        SetPos();
        ShowSpriteOrName(spriteOrName);
        SetIsGold(isGold);
        SetTarget(target);
        cardID = cardPackage.IDCardPackage;
        transform.name = $"itemCard_{cardPackage.NameCardPackage}";
        SetSize(size);
        OnOffRaycastTarget(false);
    }

    public void SetStatusLevelProgress(CardPackage cardPackage, SaveItemCard saveItemCard, Vector2 size = default)
    {
        SetPos();
        ShowSpriteOrName(saveItemCard.IndexSpriteOrName);
        SetIsGold(saveItemCard.IsGold);
        SetIsGroup(saveItemCard.IsGroup);
        SetIsGroupMerge(saveItemCard.IsGroupMerge);
        SetTarget(saveItemCard.Target);
        cardID = saveItemCard.CardID;
        transform.name = $"itemCard_{cardPackage.NameCardPackage}";
        SetSize(size);
        OnOffRaycastTarget(false);
        if (saveItemCard.IndexSpriteOrName == 0)
        {
            SetSpriteCards(cardPackage.SpriteCardType[saveItemCard.IDTypeCard]);
        }
        else if (saveItemCard.IndexSpriteOrName == 1)
        {
            if (!isGold)
            {
                SetNameTypes(cardPackage.NameType[saveItemCard.IDTypeCard]);
            }
            else
            {
                SetNameTypes(cardPackage.NameCardPackage);
            }
        }
        SetIndexGroup(saveItemCard.IndexGroup);
        SetIndexGroupMerge(saveItemCard.IndexGroupMerge);
        SetSlotIndex(saveItemCard.SlotIndexNoGroup);
        SetIdTypeCard(saveItemCard.IDTypeCard);
    }

    public void GroupMerge(ItemGroupMerge itemGroupMerge)
    {
        SetIsGroupMerge(true);
        ShowTargetText(false);
        SetIsGroup(true);
        SetParentItemCard(itemGroupMerge.transform);
        OnOffRaycastTarget(false);
        SetIndexGroupMerge(itemGroupMerge.IndexGroupMerge);
    }

    private void SetIsGroupMerge(bool bl)
    {
        isGroupMerge = bl;
    }

    public void SetIndexGroup(int index)
    {
        indexGroup = index;
    }

    public void SetIndexGroupMerge(int index)
    {
        indexGroupMerge = index;
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

}