using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] protected KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        // Check if player isn't holding anything
        if (player.KitchenObject) return;

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        kitchenObjectTransform.GetComponent<KitchenObject>().KitchenObjectParent = player;

        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
