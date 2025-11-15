using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private CuttingRecipeSO cuttingRecipeSO;
    private int cuttingProgress;

    public override void Interact(Player player)
    {
        // Counter is empty
        if (!kitchenObject)
        {
            // Then we check if player is holding something
            if (!player.KitchenObject) return;

            // Get the recipe for the kitchen Object on player
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(player.KitchenObject.KitchenObjectSO);

            // If there isn't a recipe, then we ignore it
            if (!cuttingRecipeSO) return;

            // It can be cut
            SetCuttingCounterRecipe(cuttingRecipeSO);
            player.KitchenObject.KitchenObjectParent = this;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                // We should use Float to convert at least one of the integers, so result will be float too.
                // Otherwise, result of integers will be a integer.
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });
        }
        // Counter have something on it
        else
        {
            // Then we check if player isn't holding anything
            if (player.KitchenObject)
            {
                // If it is holding something, then we check if it is a plate to add kitchenObject on it
                if (player.KitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(kitchenObject.KitchenObjectSO))
                        kitchenObject.DestroySelf();
                }
            }
            else
            {
                // Player can grab it
                kitchenObject.KitchenObjectParent = player;
                SetCuttingCounterRecipe(null);
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        // ClearCounter doesn't have anything on it
        if (!kitchenObject) return;

        // There isn't a recipe. So object can't be cut
        if (!cuttingRecipeSO) return;

        // Cut it
        cuttingProgress++;
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            // We should use Float to convert at least one of the integers, so result will be float too.
            // Otherwise, result of integers will be a integer.
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

        // Check if kitchenObject got the maximum cut progress
        if (cuttingProgress < cuttingRecipeSO.cuttingProgressMax) return;

        // Destroy the input object and spawn the output one
        kitchenObject.DestroySelf();

        KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, this);

        SetCuttingCounterRecipe(GetCuttingRecipeSOWithInput(cuttingRecipeSO.output));
    }

    private void SetCuttingCounterRecipe(CuttingRecipeSO cuttingRecipeSO)
    {
        cuttingProgress = 0;
        this.cuttingRecipeSO = cuttingRecipeSO;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
                return cuttingRecipeSO;

        return null;
    }

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
}
