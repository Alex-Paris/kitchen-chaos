using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    // Encapsulated Fields
    public bool IsWalking { get => isWalking; }
    public Transform KitchenObjectFollowTransform => kitchenObjectHoldPoint;
    public KitchenObject KitchenObject
    {
        get => kitchenObject;
        set
        {
            kitchenObject = value;
            if (value != null) OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    private BaseCounter SelectedCounter
    {
        set
        {
            selectedCounter = value;

            OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
            {
                selectedCounter = value
            });
        }
    }

    private void Awake()
    {
        if (Instance != null) Debug.LogError("There is more than one Player instance");
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameInput.OnGrabAction += GameInput_OnGrabAction;
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    // Custom functions
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);

        // Set the ray to the player's direction
        if (moveDir != Vector3.zero) lastInteractDir = moveDir;

        float interactDistance = 2f;
        // Applying LayerMask we garantee that the Ray will collide only with the desired mask. Not every single object
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            // Check if object is a ClearCounter
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SelectedCounter = baseCounter;
                }
            }
            else
            {
                SelectedCounter = null;
            }
        }
        else
        {
            SelectedCounter = null;
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        // Since no other objects are moving, we don't need a rigidyBody cand a Capsule Collider
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        // Since 'Update' is called every frame and each computer have it's own frame,
        // we have to use deltaTime since it's value is the same for each computer
        if (canMove) transform.position += moveDistance * moveDir;

        // This is one way to turn object
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        isWalking = moveDir != Vector3.zero;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    // Event listeners
    private void GameInput_OnGrabAction(object sender, System.EventArgs e)
    {
        if (!LevelGameManager.Instance.IsGamePlaying()) return;
        if (!selectedCounter) return;

        selectedCounter.Interact(this);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!LevelGameManager.Instance.IsGamePlaying()) return;
        if (!selectedCounter) return;

        if (selectedCounter) selectedCounter.InteractAlternate(this);
    }
}
