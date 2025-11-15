using System;
using System.Collections;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStoveBeingUsedEventArgs> OnStoveBeingUsed;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStoveBeingUsedEventArgs : EventArgs
    {
        public bool stoveBeingUsed;
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    private Coroutine stoveCoroutine;
    private FryingRecipeSO fryingRecipeSO;

    public override void Interact(Player player)
    {
        // Counter is empty
        if (!kitchenObject)
        {
            // Then we check if player is holding something
            if (!player.KitchenObject) return;

            // Get the recipe for the kitchen Object on player
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(player.KitchenObject.KitchenObjectSO);

            // If there isn't a recipe, then we ignore it
            if (!fryingRecipeSO) return;

            // It can be fried
            SetFryingCounterRecipe(fryingRecipeSO);
            player.KitchenObject.KitchenObjectParent = this;
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
                    {
                        kitchenObject.DestroySelf();
                        StopStove();
                        SetFryingCounterRecipe(null);
                    }
                }
            }
            else
            {
                // Player can grab it
                StopStove();
                kitchenObject.KitchenObjectParent = player;
                SetFryingCounterRecipe(null);
            }
        }
    }

    private IEnumerator StoveProcess()
    {
        for (float elapsed = 0; elapsed < fryingRecipeSO.fryingTimerMax; elapsed += Time.deltaTime)
        {
            // Update progress bar or inform about progress
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = elapsed / fryingRecipeSO.fryingTimerMax
            });

            Debug.Log($"Stoving... {elapsed}");

            yield return new WaitForSeconds(0.01f); // Wait for 0.01 milliseconds
        }

        Debug.Log("Stove process complete!");
        OnStoveComplete();
    }

    public void StartStove()
    {
        // Stop any existing coroutine
        if (stoveCoroutine != null) StopCoroutine(stoveCoroutine);

        stoveCoroutine = StartCoroutine(StoveProcess());

        // Inform stove is being used
        OnStoveBeingUsed?.Invoke(this, new OnStoveBeingUsedEventArgs
        {
            stoveBeingUsed = true
        });
    }

    public void StopStove()
    {
        if (stoveCoroutine == null) return;

        StopCoroutine(stoveCoroutine); // Stop the coroutine
        stoveCoroutine = null;

        // Inform stove is not being used
        OnStoveBeingUsed?.Invoke(this, new OnStoveBeingUsedEventArgs
        {
            stoveBeingUsed = false
        });
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    private void OnStoveComplete()
    {
        // Destroy the input object and spawn the output one
        kitchenObject.DestroySelf();

        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

        StopStove();

        SetFryingCounterRecipe(GetFryingRecipeSOWithInput(fryingRecipeSO.output));
    }

    private void SetFryingCounterRecipe(FryingRecipeSO fryingRecipeSO)
    {
        this.fryingRecipeSO = fryingRecipeSO;

        if (fryingRecipeSO) StartStove();
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
            if (fryingRecipeSO.input == inputKitchenObjectSO)
                return fryingRecipeSO;

        return null;
    }

    public bool IsFried()
    {
        return this.fryingRecipeSO.willBurn;
    }
}
