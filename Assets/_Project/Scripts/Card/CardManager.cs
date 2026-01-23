using System.Collections.Generic;
using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class CardManager : BaseMonoBehaviour
{
    private static CardManager instance;
    public static CardManager Instance => instance;
    // [SerializeField]
    // private List<CardBase> cardBases = new List<CardBase>();
    // public List<CardBase> CardBases => cardBases;

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

    // public void AddCardBase(CardBase cardBase)
    // {
    //     cardBases.Add(cardBase);
    // }
}