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
}
