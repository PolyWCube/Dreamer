using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementHandler : MonoBehaviour
{

    [Header("References"), Space(5, order = 0)]
    private InputManager input_manager;
    [SerializeField] private ProceduralAnimation animation_handler;


    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform p_orientation, left_foot, right_foot;

    [Header("Movement"), Space(5, order = 0)]
    [Header("Walk"), Space(0, order = 1)]
    public float walk_force;

    [Header("Sprint"), Space(0, order = 1)]
    public float sprint_force;

    [Header("Crouch"), Space(0, order = 1)]
    public float crouch_force;

    [Header("Jump"), Space(0, order = 1)]
    public float j_force;
    public float j_cooldown, air_mul;

    [Header("Setting"), Space(5, order = 0)]
    public float ground_drag;
    public float air_drag, height, max_step_distance, min_step_distance, max_step_time, min_step_time;

    private float inp_x, inp_z, inp_j, inp_p, m_force, max_player_velocity = 0;
    private bool on_ground, on_anticipate, on_crouch;
    private Vector3 movement_direction;

    private Ray downRay;

    [SerializeField] private Rigidbody p_body;
    [SerializeField] private AudioClip[] foot_step_fx;
    [SerializeField] private Slider health_bar;
    private FootHandler left_foot_handler, right_foot_handler;
    private int player_shader_id;
    private float health = 100;
    private void Start()
    {
        input_manager = InputManager.Instance;

        downRay = new Ray(transform.position, -transform.up);

        left_foot_handler = left_foot.gameObject.AddComponent<FootHandler>();
        right_foot_handler = right_foot.gameObject.AddComponent<FootHandler>();
        left_foot_handler.ground_layer = right_foot_handler.ground_layer = ground;
        left_foot_handler.foot_steps_fx = right_foot_handler.foot_steps_fx = foot_step_fx;

        player_shader_id = Shader.PropertyToID("_player");
        ResetJump();
    }
    private void Update()
    {
        if (health < 0)
        {
            LoadSceneManager.Instance.ResetScene();
        }
        health_bar.value = health / 100f;
        // get input
        inp_x = input_manager.xz_movement.x;
        inp_z = input_manager.xz_movement.y;
        inp_j = input_manager.y_movement;
        inp_p = input_manager.sprint;

        if (on_ground)
        {
            // limit velocity if it eceeded the movement speed and change the max movement force relative to the input
            switch (inp_p)
            {
                case 0: m_force = walk_force; break;
                case 1: m_force = sprint_force; break;
                case -1: m_force = crouch_force; break;
            }
        }

        // calculate movement direction
        movement_direction = p_orientation.forward * inp_z + p_orientation.right * inp_x;

        max_player_velocity = Mathf.Max(max_player_velocity, p_body.velocity.magnitude);
        Shader.SetGlobalVector(player_shader_id, p_body.position);

        Vector3 player_position = p_body.position;
        if (Mathf.Abs(player_position.x) > 500f && Mathf.Abs(player_position.z) > 500f || player_position.y < -10f) LoadSceneManager.Instance.ResetScene();
    }
    private void FixedUpdate()
    {
        float interpolate = p_body.velocity.magnitude / max_player_velocity;
        animation_handler.step_distance = Mathf.Lerp(min_step_distance, max_step_distance, interpolate);
        animation_handler.step_time = Mathf.Lerp(max_step_time, min_step_time, interpolate);
        // check on ground
        on_ground = left_foot_handler.on_ground || right_foot_handler.on_ground;

        // handle drag
        p_body.drag = (on_ground) ? ground_drag : air_drag;

        // handle crouch
        if (!on_crouch && inp_p < 0)
        {
            on_crouch = true;
        }
        else if (on_crouch && inp_p >= 0)
        {
            on_crouch = false;
        }
        // handle movement
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, 100f, ground);
        Vector3 player_normal = hit.normal.normalized;
        Vector3 force_direction = Vector3.ProjectOnPlane(movement_direction, player_normal).normalized;
        p_body.AddForce((on_ground) ? force_direction * m_force : force_direction * m_force * air_mul, ForceMode.Force);

        // handler jump
        if (inp_j != 0 && on_anticipate && on_ground)
        {
            on_anticipate = false;

            p_body.velocity = new Vector3(p_body.velocity.x, 0, p_body.velocity.z);
            p_body.AddForce(transform.up * j_force, ForceMode.Impulse);

            Invoke(nameof(ResetJump), j_cooldown);
        }
    }
    private void ResetJump() { on_anticipate = true; }
    private class FootHandler : MonoBehaviour
    {
        public bool on_ground;
        public LayerMask ground_layer;
        private Ray ray;
        private AudioSource audio_source;
        public AudioClip[] foot_steps_fx;
        private bool is_stepped = true;
        private void Awake()
        {
            audio_source = gameObject.AddComponent<AudioSource>();
            audio_source.volume = 0.6f;
            StartCoroutine(CheckGround());
        }
        private IEnumerator CheckGround()
        {
            while (true)
            {
                ray = new Ray(transform.position, -transform.up);
                on_ground = Physics.Raycast(ray, transform.lossyScale.y / 2 + 0.2f, ground_layer);
                if (!is_stepped && on_ground)
                {
                    PlayRandomSoundFX();
                }
                is_stepped = on_ground;
                yield return null;
            }
        }
        private void PlayRandomSoundFX()
        {
            float random = Random.value;
            for (int i = 0; i < foot_steps_fx.Length; i++)
            {
                if (1 / foot_steps_fx.Length * i > random)
                {
                    audio_source.clip = foot_steps_fx[i];
                    break;
                }
                else audio_source.clip = foot_steps_fx[foot_steps_fx.Length - 1];
            }
            if (!audio_source.isPlaying) audio_source.Play();
        }
    }
    public void Damage(float damage)
    {
        health -= damage;
    }
}