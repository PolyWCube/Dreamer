using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    [HideInInspector] public Vector2 mouse_delta, xz_movement;
    [HideInInspector] public float y_movement, sprint, mouse_right_button, mouse_left_button, esc;

    private CustomInputAction input_action;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        input_action = new CustomInputAction();

        input_action.GamePlay.MouseDelta.performed += cxt => mouse_delta = cxt.ReadValue<Vector2>();
        input_action.GamePlay.MouseDelta.canceled += cxt => mouse_delta = cxt.ReadValue<Vector2>();
        input_action.GamePlay.XZMovement.performed += cxt => xz_movement = cxt.ReadValue<Vector2>();
        input_action.GamePlay.XZMovement.canceled += cxt => xz_movement = cxt.ReadValue<Vector2>();
        input_action.GamePlay.YMovement.performed += cxt => y_movement = cxt.ReadValue<float>();
        input_action.GamePlay.YMovement.canceled += cxt => y_movement = cxt.ReadValue<float>();
        input_action.GamePlay.Sprint.performed += cxt => sprint = cxt.ReadValue<float>();
        input_action.GamePlay.Sprint.canceled += cxt => sprint = cxt.ReadValue<float>();
        input_action.GamePlay.RightMouseButton.performed += cxt => mouse_right_button = cxt.ReadValue<float>();
        input_action.GamePlay.RightMouseButton.canceled += cxt => mouse_right_button = cxt.ReadValue<float>();
        input_action.GamePlay.LeftMouseButton.performed += cxt => mouse_left_button = cxt.ReadValue<float>();
        input_action.GamePlay.LeftMouseButton.canceled += cxt => mouse_left_button = cxt.ReadValue<float>();
        input_action.GamePlay.ESC.performed += cxt => esc = cxt.ReadValue<float>();
        input_action.GamePlay.ESC.canceled += cxt => esc = cxt.ReadValue<float>();

        input_action.Enable();
    }
}