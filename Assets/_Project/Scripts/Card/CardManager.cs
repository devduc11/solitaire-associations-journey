using System.Collections.Generic;
using System.Linq;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : BaseMonoBehaviour
{
    private static CardManager instance;
    public static CardManager Instance => instance;
    [SerializeField, FindInAssets, Path("Assets/_Project/ScriptableObject/Level/Level1.asset")]
    private LevelScriptableObject loadLevel;
    [SerializeField, FindInAssets, Path("Assets/_Project/ScriptableObject/DataCard.asset")]
    private DataCard dataCard;
    [SerializeField, GetInChildren]
    private NoGroupManager noGroupManager;
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

    public void _TestLoadLv()
    {
        LoadLevel();
    }

    public void LoadLevel()
    {
        int maxColumn = loadLevel.Columns.Count;
        GroupCardSpawner.Instance.CheckSpawnItemGroupCard(maxColumn);
        GroupMergeSpawner.Instance.SpawnItemGroupMerge(maxColumn);
        LoadCar();
        PosCard.Instance.SetSizeObj();
        GroupMergeSpawner.Instance.SetSizeObj();
    }

    private void LoadCar()
    {
        if (SaveLevelManager.Instance.IsSaveLevel())
        {
            SaveLevelManager.Instance.LoadLevelProgress(() =>
            {
                var dataSave = SaveManager.Instance.DataSave;
                CardSpawner cardSpawner = CardSpawner.Instance;

                for (int i = 0; i < dataSave.SaveItemCards.Count; i++)
                {
                    SaveItemCard saveItemCard = dataSave.SaveItemCards[i];
                    CardPackage cardPackage = dataCard.CardPackages[saveItemCard.CardID];
                    cardSpawner.SpawnItemCardLevelProgress(cardPackage, saveItemCard);
                }

                cardSpawner.CheckGroupLevelProgress();

            });
        }
        else
        {
            LoadCardNew();
        }
    }

    private void LoadCardNew()
    {
        List<int> typeCards = TypeCards(); // ✅ random 1 lần
        Debug.Log($"pnad: {string.Join(", ", typeCards)}");
        CardSpawner cardSpawner = CardSpawner.Instance;

        for (int i = 0; i < typeCards.Count; i++)
        {
            int cardTypeIndex = typeCards[i];
            CardPackage cardPackage = dataCard.CardPackages[cardTypeIndex];
            CardPackageData package = loadLevel.Packages[i];

            int normalCount = package.NormalCardCount;
            // int goldCount = package.GoldCardCount;

            // 👉 Spawn card thường
            for (int n = 0; n < normalCount; n++)
            {
                cardSpawner.SpawnItemCard(cardPackage, false, normalCount);
            }
            cardSpawner.SpawnItemCard(cardPackage, true, normalCount); //Spawn card vàng

            // // 👉 Spawn card vàng
            // for (int g = 0; g < goldCount; g++)
            // {
            // }
        }
        cardSpawner.CheckGroup(loadLevel.Columns);
    }

    public List<int> TypeCards()
    {
        // int count = loadLevel.Packages.Count;

        // List<int> pool = new List<int>();
        // for (int i = 0; i < dataCard.CardPackages.Count; i++)
        // {
        //     pool.Add(i);
        // }

        // // Shuffle
        // for (int i = 0; i < pool.Count; i++)
        // {
        //     int rand = Random.Range(i, pool.Count);
        //     (pool[i], pool[rand]) = (pool[rand], pool[i]);
        // }

        // // Lấy count phần tử đầu
        // return pool.GetRange(0, count);
        return new List<int> { 0, 1, 2 };

    }

    private void MergeItemCard(ItemGroupCard itemGroupCard, ItemCard itemCard)
    {
        var spawner = GroupCardSpawner.Instance;
        spawner.ActiveItemGroupCardMove(true);

        var moveGroup = spawner.ItemGroupCardMove();

        List<ItemCard> itemCardNoGroups = new List<ItemCard>();
        if (!itemCard.IsGroup)
        {
            itemCardNoGroups.Add(noGroupManager.ItemCardNoGroup());
        }

        // var cards = itemGroupCard.SameCardTypes(itemCard.IsGold);
        var cards = !itemCard.IsGroup ? itemCardNoGroups : itemGroupCard.SameCardTypes(itemCard.IsGold);

        var lastCard = cards[^1]; // cards[cards.Count - 1]
        moveGroup.SetSizeGroup(lastCard.rect.rect.size);
        moveGroup.transform.position = lastCard.transform.position;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var card = cards[i];
            card.SetParentItemCard(moveGroup.transform);
            card.OnOffRaycastTarget(false);
            moveGroup.AddItemCard(card, true, card.ItemGroupCard, isGroup: itemCard.IsGroup);
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

    private void CheckWin()
    {

    }
}