using DBD.BaseGame;
using Teo.AutoReference;
using UnityEngine;

public class CardManager : BaseMonoBehaviour
{
    [SerializeField, GetInChildren]
    private CardSpawner cardSpawner;

    protected override void Start()
    {
        base.Start();
        // cardSpawner.SpawnItemCard();
    }
}