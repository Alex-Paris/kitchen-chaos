using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) Debug.LogError("There is more than one DeliveryCounter instance");
        Instance = this;
    }

    public override void Interact(Player player)
    {
        // Check if player is holding a plate and then destroy it
        if (player.KitchenObject)
            if (player.KitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                player.KitchenObject.DestroySelf();
            }
    }
}
