using UnityEngine;

public class KitchenObject : MonoBehaviour
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    // Encapsulated Fields
    public IKitchenObjectParent KitchenObjectParent
    {
        get => kitchenObjectParent;
        set
        {
            // If this object is on a counter, we remove it's reference on there
            kitchenObjectParent?.ClearKitchenObject();

            kitchenObjectParent = value;

            if (value.KitchenObject) Debug.LogError("Counter Already has a KitchenObject!");

            value.KitchenObject = this;

            transform.parent = kitchenObjectParent.KitchenObjectFollowTransform;
            transform.localPosition = Vector3.zero;
        }
    }
    public KitchenObjectSO KitchenObjectSO { get => kitchenObjectSO; }

    public void DestroySelf()
    {
        KitchenObjectParent.ClearKitchenObject();

        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        // Check if it is a Plate
        if (this is not PlateKitchenObject)
        {
            plateKitchenObject = null;
            return false;
        }

        plateKitchenObject = this as PlateKitchenObject;
        return true;
    }

    // Static functions
    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.KitchenObjectParent = kitchenObjectParent;
        return kitchenObject;
    }
}
