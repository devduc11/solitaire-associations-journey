using System;
using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class GroupCardSpawner : BaseSpawner<GroupCardSpawner, ItemGroupCard>
{
    [SerializeField, FindInAssets, Path("Assets/_Project/Prefab/Item/ItemGroupCard.prefab")]
    private ItemGroupCard itemGroupCardPrefab;
    [SerializeField, GetInParent]
    private CardManager cardManager;
    [SerializeField]
    private List<ItemGroupCard> itemGroupCards = new List<ItemGroupCard>();
    protected override ItemGroupCard GetPrefab()
    {
        return itemGroupCardPrefab;
    }

    int maxColumn = 4;
    int showCount = 0;

    public void CheckSpawnItemGroupCard(int maxColumn)
    {
        this.maxColumn = maxColumn;
        SpawnItemGroupCard();
    }

    public void SpawnItemGroupCard()
    {
        if (itemGroupCards.Count > 0) return;
        maxColumn = maxColumn + 1;
        showCount = maxColumn - 1;

        RectTransform rect = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = rect.rect.size;

        for (int i = 0; i < maxColumn; i++)
        {
            bool isShow = i < showCount;
            Vector2 pos = isShow ? Pos(i).position : transform.position;

            ItemGroupCard itemGroupCard = Spawn(pos, isShow);
            itemGroupCard.SetSizeGroup(size);
            itemGroupCard.OnOffRaycastTarget(!isShow);
            string name = !isShow ? "Move" : $"{i}";
            itemGroupCard.name = $"itemGroupCard_{name}";
            itemGroupCards.Add(itemGroupCard);
        }
    }

    public ItemGroupCard ItemGroupCardMove()
    {
        return itemGroupCards[itemGroupCards.Count - 1];
    }

    public void ActiveItemGroupCardMove(bool bl)
    {
        ItemGroupCardMove().gameObject.SetActive(bl);
        ItemGroupCardMove().ItemCards.Clear();
    }

    public List<ItemGroupCard> GroupContainsCards()
    {
        List<ItemGroupCard> result = new List<ItemGroupCard>();

        for (int i = 0; i < showCount; i++)
        {
            ItemGroupCard group = itemGroupCards[i];

            result.Add(group);
        }

        return result;
    }

    private RectTransform Pos(int index)
    {
        return PosCard.Instance.PosItemPosCard(index);
    }

    public void ResetItemGroupCard()
    {
        foreach (var itemGroupCard in GroupContainsCards())
        {
            itemGroupCard.ResetSizePos();
        }
    }


}
