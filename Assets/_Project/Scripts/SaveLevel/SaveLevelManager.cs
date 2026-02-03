using System;
using System.Collections.Generic;
using DBD.BaseGame;
using UnityEngine;
public class SaveLevelManager : BaseMonoBehaviour
{
    private static SaveLevelManager instance;
    public static SaveLevelManager Instance => instance;

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
        // GameAction.OnDownItemGroupCardMove += DownItemGroupCardMove;
        // GameAction.OnDragItemGroupCardMove += DragItemGroupCardMove;
        GameAction.OnUpItemGroupCardMove += UpUpItemGroupCardMove;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // GameAction.OnDownItemGroupCardMove -= DownItemGroupCardMove;
        // GameAction.OnDragItemGroupCardMove -= DragItemGroupCardMove;
        GameAction.OnUpItemGroupCardMove -= UpUpItemGroupCardMove;
    }

    private void UpUpItemGroupCardMove()
    {
        SaveLevelProgress();
    }

    public void SaveLevelProgress()
    {
        List<ItemCard> itemCards = CardSpawner.Instance.ItemCards;
        var dataSave = SaveManager.Instance.DataSave;

        int itemCardCount = itemCards.Count;
        int saveCount = dataSave.SaveItemCards.Count;

        if (saveCount > itemCardCount)
        {
            dataSave.SaveItemCards.RemoveRange(itemCardCount, saveCount - itemCardCount);
        }
        else if (saveCount < itemCardCount)
        {
            while (dataSave.SaveItemCards.Count < itemCardCount)
            {
                dataSave.SaveItemCards.Add(new SaveItemCard());
            }
        }

        for (int i = 0; i < itemCardCount; i++)
        {
            ItemCard itemCard = itemCards[i];
            SaveItemCard saveItemCard = dataSave.SaveItemCards[i];
            saveItemCard.SaveCard(itemCard);
        }

        SaveManager.Instance.SaveData();
    }

    public void LoadLevelProgress(Action action = null)
    {
       
        Debug.Log($"pnad:  LoadLevelProgress");
        action?.Invoke();
    }

    public bool IsSaveLevel()
    {
        var dataSave = SaveManager.Instance.DataSave;
        int saveCount = dataSave.SaveItemCards.Count;
        return saveCount > 0 ? true : false;
    }
}
