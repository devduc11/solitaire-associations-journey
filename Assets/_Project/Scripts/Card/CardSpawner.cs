using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class CardSpawner : BaseSpawner<CardSpawner, ItemCard>
{
    [SerializeField, FindInAssets, Path("Assets/_Project/Prefab/Item/ItemCard.prefab")]
    private ItemCard itemCardPrefab;
    [SerializeField]
    private List<ItemCard> itemCards = new List<ItemCard>();
    public List<ItemCard> ItemCards => itemCards;
    protected override ItemCard GetPrefab()
    {
        return itemCardPrefab;
    }

    public int indexPos = -1;

    public void SpawnItemCard()
    {
        ItemCard itemCard = Spawn(PosItemCard().position, true);
        itemCard.name = $"itemCard_{indexPos}";
        RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
        itemCard.SetSize(size);
        itemCards.Add(itemCard);
    }

    private RectTransform PosItemCard()
    {
        indexPos++;
        return PosCard.Instance.PosItemPosCard(indexPos);
    }
}
