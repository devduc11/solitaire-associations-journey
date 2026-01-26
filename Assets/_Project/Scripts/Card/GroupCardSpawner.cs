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
    public int index = -1;

    // public void SpawnItemGroupCard1(Vector2 position, Action<ItemGroupCard, int> action = null)
    // {
    //     index++;
    //     ItemGroupCard itemGroupCard = Spawn(position, true);
    //     itemGroupCard.transform.SetParent(CardSpawner.Instance.transform);
    //     RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
    //     Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
    //     itemGroupCard.SetSizeGroup(size);
    //     action?.Invoke(itemGroupCard, index);
    //     // cardManager.AddCardBase(itemGroupCard);
    // }

    int max = 4;
    int showCount = 0;

    public void SpawnItemGroupCard()
    {
        if (itemGroupCards.Count > 0) return;
        showCount = max - 1;

        RectTransform rect = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = rect.rect.size;

        for (int i = 0; i < max; i++)
        {
            bool isShow = i < showCount;
            Vector2 pos = isShow ? Pos(i).position : transform.position;

            ItemGroupCard itemGroupCard = Spawn(pos, isShow);
            itemGroupCard.SetSizeGroup(size);
            itemGroupCard.OnOffRaycastTarget(!isShow);
            itemGroupCard.name = $"itemGroupCard_{i}";
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


}
