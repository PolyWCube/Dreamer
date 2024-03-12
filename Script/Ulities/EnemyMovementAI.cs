using UnityEngine;

public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private ProceduralAnimation animation_handler;
    [SerializeField] private float move_force, height, max_step_distance, max_step_time, min_step_time, ground_drag, air_drag, max_body_velocity, attack_distance, attack_damage, health;
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private Transform spider_orientation;
    private Transform player;
    private Rigidbody spider, player_body;
    private ConfigurableJoint joint;
    private PlayerMovementHandler player_health;
    private void Start()
    {
        spider = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        player = GameObject.FindWithTag("Player").transform;
        player_health = player.parent.GetComponent<PlayerMovementHandler>();
        player_body = player.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (health < 0)
        {
            spider.mass = 10000f;
            spider.angularDrag = 10000f;
            spider.drag = 10000f;
            Destroy(animation_handler);
            LoadSceneManager.Instance.LoadScene(0);
            Destroy(this);
        }
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, 20f, ground_mask);

        Vector3 target_dir = player.position - transform.position;
        target_dir.y = 0;
        target_dir.Normalize();
        Vector3 force_direction = Vector3.ProjectOnPlane(target_dir, hit.normal).normalized;

        spider.AddForce(force_direction * move_force, ForceMode.Force);

        animation_handler.step_distance = Mathf.Lerp(0.1f, max_step_distance, spider.velocity.magnitude / max_body_velocity);
        animation_handler.step_time = Mathf.Lerp(max_step_time, min_step_time, spider.velocity.magnitude / max_body_velocity);

        Quaternion target_rotation = Quaternion.LookRotation(force_direction, hit.normal);
        joint.targetRotation = Quaternion.Inverse(target_rotation);

        spider.drag = Physics.Raycast(transform.position, Vector3.down, height, ground_mask) ? ground_drag : air_drag;
        max_body_velocity = Mathf.Max(max_body_velocity, spider.velocity.magnitude);
         
        if (Vector3.Distance(spider.position, player_body.position) < attack_distance)
        {
            player_health.Damage(attack_damage);
            player_body.AddForce(force_direction * 10f, ForceMode.Impulse);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        health -= 1;
    }
}