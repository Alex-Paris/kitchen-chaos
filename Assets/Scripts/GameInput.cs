using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event EventHandler OnPauseAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnGrabAction;
    public event EventHandler OnBindingRebind;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause
    }

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));

        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += Interact_performed;
        inputActions.Player.Grab.performed += Grab_performed;
        inputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.performed -= Interact_performed;
        inputActions.Player.Grab.performed -= Grab_performed;
        inputActions.Player.Pause.performed -= Pause_performed;

        inputActions.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // The question mark is used to verify if OnInteractAction is not null
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Grab_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnGrabAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        // Already normalized by input system
        //inputVector = inputVector.normalized;

        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        return binding switch
        {
            Binding.Move_Up => inputActions.Player.Move.bindings[1].ToDisplayString(),
            Binding.Move_Down => inputActions.Player.Move.bindings[3].ToDisplayString(),
            Binding.Move_Left => inputActions.Player.Move.bindings[5].ToDisplayString(),
            Binding.Move_Right => inputActions.Player.Move.bindings[7].ToDisplayString(),
            Binding.Interact => inputActions.Player.Grab.bindings[0].ToDisplayString(),
            Binding.InteractAlternate => inputActions.Player.Interact.bindings[0].ToDisplayString(),
            Binding.Pause => inputActions.Player.Pause.bindings[0].ToDisplayString(),
            _ => throw new NotImplementedException(),
        };
    }

    public void RebindBinding(Binding binding, Action OnActionRebound)
    {
        inputActions.Player.Disable();

        InputAction input;
        int keyBinding;

        switch (binding)
        {
            default:
            case Binding.Move_Up:
                keyBinding = 1;
                input = inputActions.Player.Move;
                break;
            case Binding.Move_Down:
                keyBinding = 3;
                input = inputActions.Player.Move;
                break;
            case Binding.Move_Left:
                keyBinding = 5;
                input = inputActions.Player.Move;
                break;
            case Binding.Move_Right:
                keyBinding = 7;
                input = inputActions.Player.Move;
                break;
            case Binding.Interact:
                keyBinding = 0;
                input = inputActions.Player.Grab;
                break;
            case Binding.InteractAlternate:
                keyBinding = 0;
                input = inputActions.Player.Interact;
                break;
            case Binding.Pause:
                keyBinding = 0;
                input = inputActions.Player.Pause;
                break;
        }

        input
            .PerformInteractiveRebinding(keyBinding)
            .OnComplete(callback =>
            {
                callback.Dispose();
                inputActions.Player.Enable();
                OnActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, inputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}
