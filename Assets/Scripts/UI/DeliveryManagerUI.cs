using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Start()
    {
        Hide();
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        LevelGameManager.Instance.OnStateChanged += LevelGameManager_OnStateChanged;
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, DeliveryManager.OnRecipeChangedEventArgs e)
    {
        Transform recipeTransform = Instantiate(recipeTemplate, container);
        recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(e.recipeSO);
        recipeTransform.gameObject.SetActive(true);
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, DeliveryManager.OnRecipeChangedEventArgs e)
    {
        foreach (Transform child in container)
        {
            if (child.GetComponent<DeliveryManagerSingleUI>().RecipeNameText != e.recipeSO.recipeName) continue;

            Destroy(child.gameObject);
            break;
        }
    }

    private void LevelGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (LevelGameManager.Instance.IsGamePlaying())
            Show();
        else
            Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
