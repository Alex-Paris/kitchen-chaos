using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        Transform iconTransform = Instantiate(iconTemplate, transform);
        iconTransform.GetComponent<PlateIconsSingleUI>().image.sprite = e.kitchenObjectSO.sprite;
        iconTransform.gameObject.SetActive(true);
    }
}
