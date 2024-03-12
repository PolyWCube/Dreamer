using System.Collections;
using UnityEngine;

public class ShotGunHandler : MonoBehaviour
{
    public static ShotGunHandler Instance;
    public Transform player_right_hand, player_right_hand_target, player_left_hand_target;
    public Rigidbody player_rigidbody;
    [SerializeField] private ParticleSystem shot_fx;
    [SerializeField] private AudioSource audio_source;
    public int ammo = 10;
    public bool is_equiped;
    private Vector3 player_hand_base_position, player_hand_target_position;
    private bool is_recoil = false;
    private InputManager input_manager;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        input_manager = InputManager.Instance;
        player_hand_base_position = player_right_hand_target.transform.localPosition;
        player_hand_target_position = player_right_hand_target.transform.localPosition - player_right_hand_target.forward * 3f;
    }
    private void Update()
    {
        if (is_equiped)
        {
            float delta_time = Time.deltaTime * 20f;
            transform.position = Vector3.Lerp(transform.position, player_right_hand.position, delta_time);
            transform.rotation = Quaternion.Lerp(transform.rotation, player_right_hand.rotation, delta_time);

            player_left_hand_target.position = transform.position + transform.forward * 1f;

            if (input_manager.mouse_left_button > .5f && !is_recoil && ammo > 0)
            {
                player_right_hand_target.localPosition = player_hand_target_position;
                player_rigidbody.AddForce(-transform.forward * 30f, ForceMode.Impulse);
                shot_fx.Emit(10);
                audio_source.Play();
                is_recoil = true;
                ammo -= 1;
                StartCoroutine(RecoilAnimation());
                Invoke(nameof(ResetRecoil), 1.2f);
            }
        }
    }
    private void ResetRecoil()
    {
        is_recoil = false;
    }
    private IEnumerator RecoilAnimation()
    {
        float normalize_time = 0;
        while (normalize_time <= 1f)
        {
            normalize_time += 0.1f;
            player_right_hand_target.localPosition = Vector3.Lerp(player_hand_target_position, player_hand_base_position, normalize_time);
            yield return new WaitForSeconds(0.1f);
        }
        player_right_hand_target.localPosition = player_hand_base_position;
    }
}