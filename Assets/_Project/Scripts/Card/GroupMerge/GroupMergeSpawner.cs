using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class GroupMergeSpawner : BaseSpawner<GroupMergeSpawner, ItemGroupMerge>
{
    [SerializeField, FindInAssets, Path("Assets/_Project/Prefab/Item/ItemGroupMerge.prefab")]
    private ItemGroupMerge itemGroupMergePrefab;
    protected override ItemGroupMerge GetPrefab()
    {
        return itemGroupMergePrefab;
    }

    [SerializeField, Get] private RectTransform rect;
    [SerializeField]
    private List<ItemGroupMerge> itemGroupMerges = new List<ItemGroupMerge>();
    public List<ItemGroupMerge> ItemGroupMerges => itemGroupMerges;

    public void SpawnItemGroupMerge(int maxColumn)
    {
        RectTransform rect = PosCard.Instance.SizeImgItemPosCard();
        Vector2 size = rect.rect.size;

        for (int i = 0; i < maxColumn; i++)
        {
            ItemGroupMerge itemGroupMerge = Spawn(Pos(i).position, true);
            itemGroupMerge.SetSizeGroupMerge(size);
            itemGroupMerge.name = $"itemGroupMerge_{i}";
            itemGroupMerges.Add(itemGroupMerge);
        }

        BottomCenter();
    }

    public void SetSizeObj()
    {
        SizeObj(rect, rect.rect.width, PosCard.Instance.SizeImgItemPosCard().rect.height);
    }

    private void SizeObj(RectTransform rectTarget, float width, float height)
    {
        rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private RectTransform Pos(int index)
    {
        return PosCard.Instance.PosItemPosCard(index);
    }

    public void BottomCenter()
    {
        for (int i = 0; i < itemGroupMerges.Count; i++)
        {
            ItemGroupMerge itemGroupMerge = itemGroupMerges[i];
            SetBottomCenter(itemGroupMerge.Rect, 1, 1.05f);
        }

    }

    public static void SetBottomCenter(RectTransform rt, int index, float overlapRatio)
    {
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float height = rt.rect.height;
        float y = height * 0.5f + height * overlapRatio * index;

        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
    }
}
