using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class CardSpawner : BaseSpawner<CardSpawner, ItemCard>
{
    [SerializeField, FindInAssets, Path("Assets/_Project/Prefab/Item/ItemCard.prefab")]
    private ItemCard itemCardPrefab;
    protected override ItemCard GetPrefab()
    {
        return itemCardPrefab;
    }

    public int indexPos = -1;

    public void SpawnItemCard()
    {

        ItemCard itemCard = Spawn(PosItemCard().position, true);
        RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
        // Debug.Log($"pnad: {size}");
        itemCard.SetSize(size);
    }

    private RectTransform PosItemCard()
    {
        indexPos++;
        return PosCard.Instance.PosItemPosCard(indexPos);
    }
}
