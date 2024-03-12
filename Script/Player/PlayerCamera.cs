using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraHandler : MonoBehaviour
{
    private const float max_y_rot = 50f;
    public float sen_x, sen_y, smoothness;

    [SerializeField] private Transform player_orientation, camera_position, camera_root, player_right_hand, player_right_hand_target, player_left_hand_target;
    [SerializeField] private ConfigurableJoint player_body;

    private float rotation_x, rotation_y, mouse_x, mouse_y, esc;
    private InputManager input_manager;
    private Camera player_camera;
    [SerializeField] private LayerMask environment_collider, interactable_object;
    [SerializeField] private TMP_Text object_name_display;
    [SerializeField] private GameObject enemy_spawner;
    [SerializeField] private DescriptionHandler description_handler;
    [SerializeField] private GameObject game_setting;
    [SerializeField] private Slider mouse_sensivity_x_slider, mouse_sensivity_y_slider;
    private Vector3 center_screen_point;
    private bool is_cooldown = true, is_locked_cursor = true, is_pressed;

    private void Awake()
    {
        player_camera = GetComponent<Camera>();
        center_screen_point = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }
    private void Start()
    {
        input_manager = InputManager.Instance;

    }
    private void Update()
    {
        esc = input_manager.esc;
        if (esc > .5f && !is_pressed)
        {
            is_locked_cursor = !is_locked_cursor;
            is_pressed = true;
        }
        if (is_pressed && esc == 0f)
        {
            is_pressed = false;
        }
        if (is_locked_cursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            game_setting.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            game_setting.SetActive(true);
            Time.timeScale = 0f;
            sen_x = mouse_sensivity_x_slider.value;
            sen_y = mouse_sensivity_y_slider.value;
        }

        float delta_time = Time.deltaTime;
        Vector2 mouse_delta = input_manager.mouse_delta;
        mouse_x = mouse_delta.x * sen_x * delta_time;
        mouse_y = mouse_delta.y * sen_y * delta_time;

        // update the target Euler rotation
        rotation_y += mouse_x;
        rotation_x -= mouse_y;
        rotation_x = Mathf.Clamp(rotation_x, -max_y_rot, max_y_rot);

        float smoothness_time = delta_time * smoothness;
        player_orientation.rotation = Quaternion.Lerp(player_orientation.rotation, Quaternion.Euler(0, rotation_y, 0), smoothness_time);
        Quaternion target_rotation = Quaternion.Euler(rotation_x, rotation_y, 0);
        player_body.targetRotation = Quaternion.Lerp(player_body.targetRotation, Quaternion.Inverse(target_rotation), smoothness);

        camera_root.SetPositionAndRotation(Vector3.Lerp(camera_root.position, camera_position.position, smoothness_time * 2f), Quaternion.Lerp(camera_root.rotation, target_rotation, smoothness_time));

        transform.LookAt(camera_root);
        CheckInteractable();
    }
    private void CheckInteractable()
    {
        Ray ray = player_camera.ScreenPointToRay(center_screen_point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 4f, interactable_object))
        {
            object_name_display.text = hit.collider.gameObject.name;
            if (input_manager.mouse_right_button > .5f && is_cooldown)
            {
                GameObject interactable_object = hit.collider.gameObject;

                if (interactable_object.name == "Locked Box")
                {
                    description_handler.SetText("It's locked! There might be a way to open it.");
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == "Shotgun")
                {
                    ShotGunHandler shotgun_handler = interactable_object.GetComponent<ShotGunHandler>();
                    shotgun_handler.is_equiped = true;
                    shotgun_handler.GetComponent<Collider>().enabled = false;
                    enemy_spawner.SetActive(true);
                    description_handler.SetText("What can I do with it?");
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == " Shotgun ")
                {
                    ShotGunHandler shotgun_handler = interactable_object.GetComponent<ShotGunHandler>();
                    shotgun_handler.is_equiped = true;
                    shotgun_handler.GetComponent<Collider>().enabled = false;
                    shotgun_handler.player_right_hand = player_right_hand;
                    shotgun_handler.player_left_hand_target = player_left_hand_target;
                    shotgun_handler.player_right_hand_target = player_right_hand_target;
                    shotgun_handler.player_rigidbody = player_body.GetComponent<Rigidbody>();
                    description_handler.SetText("...");
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == "Shotgun Shell")
                {
                    ShotGunHandler.Instance.ammo += 1;
                    Destroy(interactable_object.transform.parent.gameObject);
                    description_handler.SetText("Some shotgun shells...");
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == " Shotgun Shell ")
                {
                    ShotGunHandler.Instance.ammo += 1;
                    Destroy(interactable_object.transform.parent.gameObject);
                    description_handler.SetText("...");
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == "???")
                {
                    description_handler.SetText("I remember something...");
                    LoadSceneManager.Instance.LoadScene(2);
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == " ??? ")
                {
                    description_handler.SetText("It's deep inside...");
                    LoadSceneManager.Instance.LoadScene(3);
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else if (interactable_object.name == "  ???  ")
                {
                    description_handler.SetText("But I should wake up...");
                    LoadSceneManager.Instance.LoadScene(4);
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
                else
                {
                    InteractableObject interactable_gameobject = interactable_object.GetComponentInParent<InteractableObject>();
                    if (interactable_gameobject != null) interactable_gameobject.Interact();
                    is_cooldown = false;
                    Invoke(nameof(Cooldown), 1f);
                }
            }
        }
        else
        {
            object_name_display.text = null;
        }
    }
    private void Cooldown()
    {
        is_cooldown = true;
    }
}
