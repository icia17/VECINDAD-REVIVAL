using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    private PlayerInputActions _playerInputActions;
    private InputAction _move;
    private InputAction _dash;
    private InputAction _fire;
    private InputAction _light;
    private InputAction _interact;
    private InputAction _pause;
    private InputAction _reload;
    private InputAction _change;
    private InputAction _throw;
    
    private void Awake() {
        _playerInputActions = new PlayerInputActions();

        _move = _playerInputActions.Player.Move;
        _dash = _playerInputActions.Player.Dash;
        _fire = _playerInputActions.Player.Fire;
        _light = _playerInputActions.Player.Light;
        _interact = _playerInputActions.Player.Interact;
        _pause = _playerInputActions.Player.Pause;
        _reload = _playerInputActions.Player.Reload;
        _change = _playerInputActions.Player.Change;
        _throw = _playerInputActions.Player.Throw;
    }

    private void OnEnable() {
        _playerInputActions.Enable();
    }

    private void OnDisable() {
        _playerInputActions.Disable();
    }

    void Update()
    {
        FrameInput = GatherInput();
    }

    private FrameInput GatherInput() {
        return new FrameInput { 
            Move = _move.ReadValue<Vector2>(),
            Dash = _dash.IsPressed(),
            Fire = _fire.IsPressed(),
            Light = _light.WasPressedThisFrame(),
            Interact = _interact.WasPressedThisFrame(),
            Pause = _pause.WasPressedThisFrame(),
            Reload = _reload.WasPressedThisFrame(),
            Change = _change.WasPressedThisFrame(),
            Throw = _throw.WasPressedThisFrame(),
        };
    }
}

public struct FrameInput {
    public Vector2 Move;
    public bool Dash;
    public bool Fire;
    public bool Light;
    public bool Interact;
    public bool Pause;
    public bool Reload;
    public bool Change;
    public bool Throw;
}