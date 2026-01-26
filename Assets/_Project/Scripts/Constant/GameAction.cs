using System;
using UnityEngine;

public class GameAction
{
    public static Action<ItemGroupCard> OnMergeItemCard;
    public static Action<UnityEngine.EventSystems.PointerEventData> OnPointerDownItemCard;
    public static Action<UnityEngine.EventSystems.PointerEventData> OnDragItemCard;
    public static Action<UnityEngine.EventSystems.PointerEventData> OnPointerUpItemCard;
}
