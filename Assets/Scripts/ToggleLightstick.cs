using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// Toggles the lightstick visual on a button press.
/// Put this on the controller (or a parent), NOT on the visual itself —
/// disabling its own GameObject would unsubscribe the input and lock it off.
public class ToggleLightstick : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleAction; // e.g. <XRController>{RightHand}/primaryButton
    [SerializeField] private GameObject lightstickVisual;
    [SerializeField] private bool startOn = false;

    public bool IsOn { get; private set; }
    public event Action<bool> Toggled;

    private void Awake() => SetState(startOn);

    private void OnEnable()
    {
        toggleAction.action.performed += OnTogglePerformed;
        toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= OnTogglePerformed;
    }

    private void OnTogglePerformed(InputAction.CallbackContext ctx) => SetState(!IsOn);

    private void SetState(bool on)
    {
        IsOn = on;
        lightstickVisual.SetActive(on);
        Toggled?.Invoke(on);
    }
}