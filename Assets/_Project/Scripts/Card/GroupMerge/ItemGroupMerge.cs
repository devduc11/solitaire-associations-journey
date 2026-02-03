using System;
using System.Collections.Generic;
using DBD.BaseGame;
using DG.Tweening;
using Teo.AutoReference;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGroupMerge : BaseMonoBehaviour
{
    [SerializeField, Get] protected Image image;
    [SerializeField, Get] private RectTransform rect;
    public RectTransform Rect => rect;
    [SerializeField, GetInChildren, Name("TargetText")]
    private TextMeshProUGUI targetText;
    [SerializeField]
    private int cardID;
    public int CardID
    {
        get => cardID;
        set => cardID = value;
    }

    [SerializeField]
    private int indexGroupMerge;
    public int IndexGroupMerge
    {
        get => indexGroupMerge;
        set => indexGroupMerge = value;
    }

    [SerializeField]
    private List<ItemCard> itemCardGroups = new List<ItemCard>();
    protected override void OnEnable()
    {
        base.OnEnable();
        SetCarID(-1);
    }

    public void SetSizeGroupMerge(Vector2 size)
    {
        // SizeObj(rect, size.x, size.y);
        float temp = image.rectTransform.rect.width;
        image.rectTransform.sizeDelta = size;
        image.pixelsPerUnitMultiplier = temp / image.rectTransform.rect.width;
    }

    public void SetCarID(int id)
    {
        cardID = id;
    }

    int target;
    public void SetTarget(int index)
    {
        int min = itemCardGroups.Count - 1 < 0 ? 0 : itemCardGroups.Count - 1;
        targetText.text = $"{min}/{index}";
        target = index;
    }

    public void CheckMerge(List<ItemCard> itemCards, Action onComplete = null)
    {
        transform.parent.SetAsLastSibling();
        transform.SetAsLastSibling();
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];

            SetCarID(itemCard.CardID);
            itemCard.GroupMerge(this);
            itemCardGroups.Add(itemCard);
            SetTarget(itemCard.Target);
            Tween tween = SetTopCenterTween(itemCard.rect, i, 0.25f);
            seq.Join(tween); // chạy song song
        }

        seq.OnComplete(() =>
        {
            Sequence mergeSeq = DOTween.Sequence();
            foreach (var itemCard in itemCardGroups)
            {
                mergeSeq.Join(itemCard.rect.DOMove(transform.position, 0.25f).SetEase(Ease.OutQuad));
            }

            mergeSeq.OnComplete(() =>
            {
                onComplete?.Invoke();
                CheckTarget();
                // Debug.Log("🔥 All tweens finished");
            });
        });
        targetText.transform.SetAsLastSibling();
    }

    public void CheckMergeLevelProgress(List<ItemCard> itemCards)
    {
        // CheckMerge(itemCards);
        transform.parent.SetAsLastSibling();
        transform.SetAsLastSibling();
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < itemCards.Count; i++)
        {
            ItemCard itemCard = itemCards[i];

            SetCarID(itemCard.CardID);
            itemCard.GroupMerge(this);
            itemCardGroups.Add(itemCard);
            SetTarget(itemCard.Target);
            Tween tween = SetTopCenterTween(itemCard.rect, 1, 0);
            seq.Join(tween); // chạy song song
        }
        targetText.transform.SetAsLastSibling();
    }

    private void CheckTarget()
    {
        if (itemCardGroups.Count - 1 == target)
        {
            Debug.Log($"pnad: 111111111111111");
            ResetGroup();
        }
    }

    public static Tween SetTopCenterTween(RectTransform rt, int index, float overlapRatio)
    {
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float height = rt.rect.height;
        float y = -height * 0.5f - height * overlapRatio * index;

        rt.DOKill();

        return rt.DOAnchorPos(new Vector2(0f, y), 0.25f)
                 .SetEase(Ease.OutQuad);
    }

    private void ResetGroup()
    {
        SetCarID(-1);
        foreach (var itemCardGroup in itemCardGroups)
        {
            CardSpawner.Instance.DespawnItemCard(itemCardGroup);
        }
        itemCardGroups.Clear();
        SetTarget(0);
    }

    public void SetIndexGroupMerge(int index)
    {
        indexGroupMerge = index;
    }

}