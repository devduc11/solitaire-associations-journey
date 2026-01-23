using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class CardSpawner : BaseSpawner<CardSpawner, ItemCard>
{
    [SerializeField, FindInAssets, Path("Assets/_Project/Prefab/Item/ItemCard.prefab")]
    private ItemCard itemCardPrefab;
    [SerializeField, GetInParent]
    private CardManager cardManager;
    [SerializeField]
    private List<ItemCard> itemCards = new List<ItemCard>();
    public List<ItemCard> ItemCards => itemCards;

    protected override ItemCard GetPrefab()
    {
        return itemCardPrefab;
    }

    public int index;
    // public int indexPos;

    public void SpawnItemCard()
    {
        if (GroupCardSpawner.Instance.GroupContainsCards().Count == 0) return;

        ItemCard itemCard = Spawn(transform.position, true);
        ItemGroupCard itemGroupCard = GroupCardSpawner.Instance.GroupContainsCards()[index];
        itemCard.name = $"itemCard_{index}";
        RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
        itemCard.SetSize(size);
        itemCard.transform.SetParent(itemGroupCard.transform);
        itemCards.Add(itemCard);

        itemGroupCard.AddItemCard(itemCard);
        
    }

    /*  public void SpawnItemCard()
     {
         ItemCard itemCard = Spawn(PosItemCard().position, true);
         itemCard.name = $"itemCard_{indexPos}";
         RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
         Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
         itemCard.SetSize(size);
         itemCards.Add(itemCard);
     }  */

    // private RectTransform PosItemCard()
    // {
    //     return PosCard.Instance.PosItemPosCard(indexPos);
    // }
}
