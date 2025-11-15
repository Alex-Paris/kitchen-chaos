using System;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;

    public override void Interact(Player player)
    {
        // Check if player is holding something and then destroy it
        if (!player.KitchenObject) return;

        player.KitchenObject.DestroySelf();
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }

    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }
}
