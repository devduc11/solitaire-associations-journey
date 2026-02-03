using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class CardSpawner : BaseSpawner<CardSpawner, ItemCard>
{
    [SerializeField, FindInAssets, Path("Assets/_Project/Prefab/Item/ItemCard.prefab")]
    private ItemCard itemCardPrefab;
    [SerializeField, GetInChildren]
    private NoGroupManager noGroupManager;
    // [SerializeField, GetInChildren, Name("NoGroupManager")]
    // private RectTransform noGroupRect;
    [SerializeField]
    private List<ItemCard> itemCards = new List<ItemCard>();
    public List<ItemCard> ItemCards => itemCards;

    [SerializeField]
    private List<ItemCard> noGroupItemCards = new List<ItemCard>();
    public List<ItemCard> NoGroupItemCards
    {
        get => noGroupItemCards;
        set => noGroupItemCards = value;
    }

    protected override ItemCard GetPrefab()
    {
        return itemCardPrefab;
    }

    public int index;
    public int indexPos;

    public void SpawnItemCard(CardPackage cardPackage, bool isGold, int target = 0)
    {
        if (GroupCardSpawner.Instance.GroupContainsCards().Count == 0) return;
        ItemCard itemCard = Spawn(transform.position, true);
        int index = isGold ? 1 : (cardPackage.SpriteCardType.Count > 0 ? 0 : 1);
        RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
        itemCard.SetStatus(cardPackage, index, isGold, target, size);
        noGroupManager.SetSize(size);
        LoadDataCard(index, cardPackage, itemCard);
        itemCards.Add(itemCard);
    }

    private void LoadDataCard(int index, CardPackage cardPackage, ItemCard itemCard)
    {
        if (index == 0)
        {
            List<Sprite> sprites = new List<Sprite>(cardPackage.SpriteCardType);
            if (sprites.Count == 0) return;

            ShuffleList(sprites);
            Sprite selectedSprite = sprites[0];

            foreach (var spriteData in sprites)
            {
                bool isUsed = false;
                foreach (var card in itemCards)
                {
                    if (card.CardID == cardPackage.IDCardPackage && card.SpriteCards.Count > 0 && card.SpriteCards[0].sprite == spriteData)
                    {
                        isUsed = true;
                        break;
                    }
                }

                if (!isUsed)
                {
                    selectedSprite = spriteData;
                    int originalIndex = cardPackage.SpriteCardType.IndexOf(selectedSprite);
                    itemCard.SetIdTypeCard(originalIndex);
                    // Debug.Log($"pnad: {originalIndex}");
                    break;
                }
            }

            itemCard.SetSpriteCards(selectedSprite);
        }
        else if (index == 1 && !itemCard.IsGold)
        {
            List<string> names = new List<string>(cardPackage.NameType);
            if (names.Count == 0) return;

            ShuffleList(names);

            string selectedName = names[0];
            foreach (var nameData in names)
            {
                bool isUsed = false;
                foreach (var card in itemCards)
                {
                    if (card.CardID == cardPackage.IDCardPackage && card.NameTypes.Count > 0 && card.NameTypes[0].text == nameData)
                    {
                        isUsed = true;
                        break;
                    }
                }

                if (!isUsed)
                {
                    selectedName = nameData;
                    int originalIndex = cardPackage.NameType.IndexOf(selectedName);
                    // Debug.Log($"pnad: {originalIndex}");
                    itemCard.SetIdTypeCard(originalIndex);
                    break;
                }
            }

            itemCard.SetNameTypes(selectedName);
        }
        else if (itemCard.IsGold)
        {
            itemCard.SetNameTypes(cardPackage.NameCardPackage);
        }
    }

    public void CheckGroup(List<int> columns)
    {
        ShuffleList(itemCards);
        List<ItemGroupCard> itemGroupCards = GroupCardSpawner.Instance.GroupContainsCards();
        int cardIndex = 0;

        for (int i = 0; i < columns.Count; i++)
        {
            if (i >= itemGroupCards.Count) break;

            int sumCard = columns[i]; // tổng số thẻ có trong mỗi cột
            ItemGroupCard group = itemGroupCards[i];

            for (int j = 0; j < sumCard; j++)
            {
                if (cardIndex < itemCards.Count)
                {
                    ItemCard itemCard = itemCards[cardIndex];
                    itemCard.SetGroupCarSpawn(group);
                    cardIndex++;
                }
            }
        }

        AllItemCardNoGroup();
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void AllItemCardNoGroup()
    {
        foreach (var itemCard in itemCards)
        {
            if (!itemCard.IsGroup)
            {
                itemCard.gameObject.SetActive(false);
                itemCard.SetSlotIndex(-1);
                itemCard.SetIndexGroup(-1);
                itemCard.SetIndexGroupMerge(-1);
                // itemCard.transform.SetParent(noGroupRect);
                noGroupItemCards.Add(itemCard);
            }
        }
        noGroupManager.gameObject.SetActive(true);
        SaveLevelManager.Instance.SaveLevelProgress();
    }

    public void SpawnItemCardLevelProgress(CardPackage cardPackage, SaveItemCard saveItemCard)
    {
        ItemCard itemCard = Spawn(transform.position, true);
        RectTransform rectItemPosCard = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = new Vector2(rectItemPosCard.rect.width, rectItemPosCard.rect.height);
        itemCard.SetStatusLevelProgress(cardPackage, saveItemCard, size);
        noGroupManager.SetSize(size);
        itemCards.Add(itemCard);
    }

    public void CheckGroupLevelProgress()
    {
        List<ItemGroupCard> itemGroupCards = GroupCardSpawner.Instance.GroupContainsCards();
        List<ItemGroupMerge> itemGroupMerges = GroupMergeSpawner.Instance.ItemGroupMerges;

        Dictionary<int, ItemGroupCard> groupByIndex = new Dictionary<int, ItemGroupCard>(itemGroupCards.Count);
        Dictionary<int, ItemGroupMerge> groupMergeByIndex = new Dictionary<int, ItemGroupMerge>(itemGroupMerges.Count);

        for (int i = 0; i < itemGroupCards.Count; i++)
        {
            ItemGroupCard itemGroupCard = itemGroupCards[i];
            groupByIndex[itemGroupCard.IndexGroup] = itemGroupCard;
        }

        for (int i = 0; i < itemGroupMerges.Count; i++)
        {
            ItemGroupMerge itemGroupMerge = itemGroupMerges[i];
            groupMergeByIndex[itemGroupMerge.IndexGroupMerge] = itemGroupMerge;
        }

        Dictionary<int, List<ItemCard>> itemCardsByGroupMergeIndex = new Dictionary<int, List<ItemCard>>();
        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];
            if (itemCard.IsGroup && !itemCard.IsGroupMerge)
            {
                if (groupByIndex.TryGetValue(itemCard.IndexGroup, out ItemGroupCard itemGroupCard))
                {
                    itemCard.SetGroupCarSpawnLevelProgress(itemGroupCard);
                }
            }
            else if (itemCard.IsGroup && itemCard.IsGroupMerge)
            {
                if (!groupMergeByIndex.ContainsKey(itemCard.IndexGroupMerge))
                {
                    continue;
                }

                if (!itemCardsByGroupMergeIndex.TryGetValue(itemCard.IndexGroupMerge, out List<ItemCard> mergeCards))
                {
                    mergeCards = new List<ItemCard>();
                    itemCardsByGroupMergeIndex[itemCard.IndexGroupMerge] = mergeCards;
                }

                mergeCards.Add(itemCard);
            }
        }

        if (itemCardsByGroupMergeIndex.Count > 0)
        {
            List<int> groupMergeIndexes = new List<int>(itemCardsByGroupMergeIndex.Keys);
            groupMergeIndexes.Sort();
            for (int i = 0; i < groupMergeIndexes.Count; i++)
            {
                int index = groupMergeIndexes[i];
                if (groupMergeByIndex.TryGetValue(index, out ItemGroupMerge itemGroupMerge))
                {
                    itemGroupMerge.CheckMergeLevelProgress(itemCardsByGroupMergeIndex[index]);
                }
            }
        }

        AllItemCardNoGroup();
    }

    public void _ResetListItemCard()
    {
        foreach (var itemCard in itemCards)
        {
            DespawnItemCard(itemCard);
            itemCard.ResetItem();
        }
        itemCards.Clear();
        noGroupItemCards.Clear();
    }

    public void DespawnItemCard(ItemCard itemCard)
    {
        itemCard.SetParentItemCard(transform);
        Despawn(itemCard);
    }
}
