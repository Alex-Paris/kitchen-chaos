using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler<OnRecipeChangedEventArgs> OnRecipeSpawned; 
    public event EventHandler<OnRecipeChangedEventArgs> OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public class OnRecipeChangedEventArgs : EventArgs
    {
        public RecipeSO recipeSO;
    }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private Coroutine coroutine;
    private readonly float spawnRecipeTimer = 4f;
    private readonly int waitingRecipesMax = 4;
    private int successfulRecipesAmout;

    public int SuccessfulRecipesAmout => successfulRecipesAmout;

    private void Awake()
    {
        if (Instance != null) Debug.LogError("There is more than one DeliveryManager instance");

        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Start()
    {
        StartCoroutine(); // Start Coroutine to make recipes
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // Recipe doesn't have the same number of ingredients, continue the loop
            if (waitingRecipeSO.kitchenObjectSOList.Count != plateKitchenObject.KitchenObjectSOList.Count) continue;

            bool hasTheSameKitchenContents = true;

            foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
            {
                bool hasFoundIngredient = false;
                foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.KitchenObjectSOList)
                {
                    // Check if plate actual object is the same from the recipe 
                    if (plateKitchenObjectSO == recipeKitchenObjectSO)
                    {
                        // Then we can check the next plate kitchenObject
                        hasFoundIngredient = true;
                        break;
                    }
                }
                
                if (!hasFoundIngredient)
                {
                    // If the plate kitchenObject list doesn't have the ingredient, so this is not the recipe
                    hasTheSameKitchenContents = false;
                    break;
                }
            }

            // The plate has the right objects
            if (hasTheSameKitchenContents)
            {
                OnRecipeCompleted?.Invoke(this, new OnRecipeChangedEventArgs { recipeSO = waitingRecipeSOList[i] });
                waitingRecipeSOList.RemoveAt(i);
                successfulRecipesAmout++;
                StartCoroutine();
                return;
            }
        }

        // If we got here, then the plate doesn't match with any other waiting recipe list
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator CoroutineProcess()
    {
        while (waitingRecipeSOList.Count < waitingRecipesMax)
        {
            yield return new WaitForSeconds(spawnRecipeTimer); // Wait for time before including new recipe
            
            if (!LevelGameManager.Instance.IsGamePlaying()) continue;

            RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
            waitingRecipeSOList.Add(waitingRecipeSO);

            OnRecipeSpawned?.Invoke(this, new OnRecipeChangedEventArgs { recipeSO = waitingRecipeSO });
        }

        StopCoroutine();
    }

    public void StartCoroutine()
    {
        // If coroutine already being executed, than let it go
        if (coroutine != null) return;

        coroutine = StartCoroutine(CoroutineProcess());
    }

    public void StopCoroutine()
    {
        if (coroutine == null) return;

        StopCoroutine(coroutine); // Stop the coroutine
        coroutine = null;
    }
}
