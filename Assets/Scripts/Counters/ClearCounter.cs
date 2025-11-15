using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        // ClearCounter is empty
        if (!kitchenObject)
        {
            // Then we check if player is holding something
            if (player.KitchenObject)
                player.KitchenObject.KitchenObjectParent = this; // And let it there
        }
        // ClearCounter have something on it
        else
        {
            // Then we check if player isn't holding anything
            if (player.KitchenObject)
            {
                // If it is holding something, then we check if it is a plate to add kitchenObject on it
                if (player.KitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Then we check kitchenObject on ClearCounter can go to player Plate
                    if (plateKitchenObject.TryAddIngredient(kitchenObject.KitchenObjectSO))
                        kitchenObject.DestroySelf();
                }
                // If isn't a Plate, then we check if ClearCounter has a Plate
                else if (kitchenObject.TryGetPlate(out plateKitchenObject))
                {
                    // Then we check kitchenObject on player can go to ClearCounter Plate
                    if (plateKitchenObject.TryAddIngredient(player.KitchenObject.KitchenObjectSO))
                        player.KitchenObject.DestroySelf();
                }
            }
            // Then give it to the player
            else
                kitchenObject.KitchenObjectParent = player;
        }
    }
}
