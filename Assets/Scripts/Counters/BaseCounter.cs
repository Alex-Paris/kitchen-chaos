using System;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;

    [SerializeField] protected Transform counterTopPoint;

    protected KitchenObject kitchenObject;

    // Encapsulation Fields
    public Transform KitchenObjectFollowTransform { get => counterTopPoint; }
    public KitchenObject KitchenObject
    {
        get => kitchenObject;
        set
        {
            kitchenObject = value;
            if (value != null) OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    // Virtual means that this method is overridable by their class childrens
    public virtual void Interact(Player player)
    {
        throw new System.NotImplementedException();
    }

    public virtual void InteractAlternate(Player player)
    {
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }
}
