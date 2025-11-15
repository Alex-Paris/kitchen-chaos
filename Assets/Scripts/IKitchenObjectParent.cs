using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform KitchenObjectFollowTransform { get; }
    public KitchenObject KitchenObject { get; set; }

    public void ClearKitchenObject();
}
