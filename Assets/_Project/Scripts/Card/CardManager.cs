using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : BaseMonoBehaviour
{
    private static CardManager instance;
    public static CardManager Instance => instance;
    // [SerializeField]
    // private List<CardBase> cardBases = new List<CardBase>();
    // public List<CardBase> CardBases => cardBases;

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

    // public void AddCardBase(CardBase cardBase)
    // {
    //     cardBases.Add(cardBase);
    // }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameAction.OnMergeItemCard += MergeItemCard;
        GameAction.OnPointerDownItemCard += DownItemCard;
        GameAction.OnDragItemCard += DragItemCard;
        GameAction.OnPointerUpItemCard += PointerUpItemCard;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameAction.OnMergeItemCard -= MergeItemCard;
        GameAction.OnPointerDownItemCard -= DownItemCard;
        GameAction.OnDragItemCard -= DragItemCard;
        GameAction.OnPointerUpItemCard -= PointerUpItemCard;
    }

    private void MergeItemCard(ItemGroupCard itemGroupCard)
    {
        var spawner = GroupCardSpawner.Instance;
        spawner.ActiveItemGroupCardMove(true);

        var moveGroup = spawner.ItemGroupCardMove();
        var cards = itemGroupCard.SameCardTypes();

        var lastCard = cards[^1]; // cards[cards.Count - 1]
        moveGroup.SetSizeGroup(lastCard.rect.rect.size);
        moveGroup.transform.position = lastCard.transform.position;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var card = cards[i];
            card.SetParentItemCard(moveGroup.transform);
            card.OnOffRaycastTarget(false);
            moveGroup.AddItemCard(card, true, card.ItemGroupCard);
        }
    }


    private void DownItemCard(PointerEventData eventData)
    {
        ItemGroupCard itemGroupCardMove = GroupCardSpawner.Instance.ItemGroupCardMove();
        itemGroupCardMove.OnPointerDown(eventData);
    }

    private void DragItemCard(PointerEventData eventData)
    {
        ItemGroupCard itemGroupCardMove = GroupCardSpawner.Instance.ItemGroupCardMove();
        itemGroupCardMove.OnDrag(eventData);
    }

    private void PointerUpItemCard(PointerEventData eventData)
    {
        ItemGroupCard itemGroupCardMove = GroupCardSpawner.Instance.ItemGroupCardMove();
        itemGroupCardMove.OnPointerUp(eventData);
    }
}