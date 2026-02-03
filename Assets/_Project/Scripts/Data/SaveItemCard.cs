using DBD.BaseGame;

[System.Serializable]
public class SaveItemCard
{
    public bool IsGold = false;
    public bool IsGroup = false;
    public bool IsGroupMerge = false;
    public int CardID = -1;
    public int IDTypeCard = -1;
    public int SlotIndexNoGroup = -1;
    public int Target = -1;
    public int IndexGroup = -1;
    public int IndexGroupMerge = -1;
    public int IndexSpriteOrName = -1;

    public void SaveCard(ItemCard itemCard)
    {
        IsGold = itemCard.IsGold;
        IsGroup = itemCard.IsGroup;
        IsGroupMerge = itemCard.IsGroupMerge;
        CardID = itemCard.CardID;
        IDTypeCard = itemCard.IDTypeCard;
        SlotIndexNoGroup = itemCard.SlotIndex;
        Target = itemCard.Target;
        IndexGroup = itemCard.IndexGroup;
        IndexGroupMerge = itemCard.IndexGroupMerge;
        IndexSpriteOrName = itemCard.IndexSpriteOrName;
    }

    public void ResetCard()
    {
        IsGold = false;
        IsGroup = false;
        IsGroupMerge = false;
        CardID = -1;
        IDTypeCard = -1;
        SlotIndexNoGroup = -1;
        Target = -1;
        IndexGroup = -1;
        IndexGroupMerge = -1;
        IndexSpriteOrName = -1;
    }
}