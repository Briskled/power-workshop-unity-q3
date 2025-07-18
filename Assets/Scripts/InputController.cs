#region

using UnityEngine;
using UnityEngine.InputSystem;

#endregion

/// <summary>
/// This script handles the connection between user input and the game by notifying the <see cref="gameManager"/> about button presses
/// </summary>
public class InputController : MonoBehaviour
{
    private InputActions inputActions;

    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Default.Enable();
        inputActions.Default.Reset.performed += OnReset;
        inputActions.Default.Next.performed += OnNext;
    }

    private void OnDisable()
    {
        inputActions.Default.Disable();
        inputActions.Default.Reset.performed -= OnReset;
        inputActions.Default.Next.performed -= OnNext;
    }

    private void OnReset(InputAction.CallbackContext obj)
    {
        gameManager.ResetState();
    }

    private void OnNext(InputAction.CallbackContext obj)
    {
        gameManager.Next();
    }
}