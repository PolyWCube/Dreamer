using System.Collections;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct Leg
    {
        public Transform local_leg_target, world_leg_target, ray_root;
        [HideInInspector] public LegHandler leg_handler;
    }
    [System.Serializable]
    public struct Arm
    {
        public Transform target, local_arm_target, world_arm_target;
        [HideInInspector] public ArmHandler arm_handler;
    }
    public float step_distance, step_time, max_height;
    [SerializeField] public float step_offset, step_direction_multiplier;
    [SerializeField] private Leg[] legs;
    [SerializeField] private Arm[] arms;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform body_orientation;
    [SerializeField] private Rigidbody body;

    private Vector3 movement_direction;
    private int leg_count;
    [SerializeField] private bool can_walk_on_wall;

    private void Awake()
    {
        leg_count = legs.Length;

        for (int i = 0; i < legs.Length; ++i)
        {
            LegHandler leg_handler = legs[i].leg_handler = legs[i].world_leg_target.gameObject.AddComponent<LegHandler>();
            leg_handler.max_height = max_height;
            leg_handler.step_time = step_time;
            leg_handler.walkable = ground;
            leg_handler.y_offset = step_offset;
            leg_handler.body_orientation = body_orientation;
            leg_handler.ray_root = legs[i].ray_root;
            leg_handler.world_leg_target = legs[i].world_leg_target;
            leg_handler.local_leg_target = legs[i].local_leg_target;
            if (i % 2 == 0) leg_handler.is_moved = true;
            else leg_handler.on_move = false;
        }
        for (int i = 0; i < arms.Length; ++i)
        {
            ArmHandler arm_handler = arms[i].arm_handler = arms[i].world_arm_target.gameObject.AddComponent<ArmHandler>();
            arm_handler.target = arms[i].target;
            arm_handler.world_hand_target = arms[i].world_arm_target;
            arm_handler.local_hand_target = arms[i].local_arm_target;
        }
        StartCoroutine(AnimationUpdate());
    }
    private IEnumerator AnimationUpdate()
    {
        while (true)
        {
            movement_direction = new Vector3(body.velocity.x, 0, body.velocity.z).normalized * step_direction_multiplier;

            for (int i = 0; i < leg_count / 2; i++)
            {
                TryMove(legs[i * 2 + 1], legs[i * 2]);
                TryMove(legs[i * 2], legs[i * 2 + 1]);
            }
            yield return null;
        }
    }
    private void TryMove(Leg leg, Leg paired_leg)
    {
        if (leg.leg_handler.on_move) return;
        leg.leg_handler.step_time = step_time;
        if (paired_leg.leg_handler.is_moved)
        {
            RaycastHit hit;
            Vector3 base_direction = can_walk_on_wall ? -body_orientation.up : Vector3.down;
            Physics.Raycast(leg.ray_root.position, base_direction + movement_direction, out hit, max_height, ground);

            leg.local_leg_target.rotation = Quaternion.LookRotation(body_orientation.forward, hit.normal);

            if (Vector3.Distance(hit.point, leg.world_leg_target.position) > step_distance)
            {
                leg.leg_handler.MoveToRoot();
                paired_leg.leg_handler.is_moved = false;
            }
        }
        leg.leg_handler.movement_direction = movement_direction;
        leg.local_leg_target.position = leg.world_leg_target.position;
    }
    public class LegHandler : MonoBehaviour
    {
        public float max_height, y_offset, step_time;
        public LayerMask walkable;
        public Transform body_orientation, ray_root, local_leg_target, world_leg_target;
        public bool can_walk_on_wall, on_move, is_moved;
        public Vector3 movement_direction;

        private RaycastHit hit;
        public void MoveToRoot()
        {
            StartCoroutine(LegProceduralAnimation());
        }
        private IEnumerator LegProceduralAnimation()
        {
            on_move = true;
            Vector3 base_direction = can_walk_on_wall ? -body_orientation.up : Vector3.down;
            Physics.Raycast(ray_root.position, base_direction + movement_direction, out hit, max_height, walkable);

            // determine the start and end point
            Vector3 start_position = world_leg_target.position;
            Vector3 end_position = hit.point;

            // determine the center point
            Vector3 center_pos = (start_position + end_position) / 2 + hit.normal * y_offset;

            float time_elapse = 0f;
            do
            {
                // normalize time
                time_elapse += Time.deltaTime;
                float normal_time = time_elapse / step_time;

                // interpolate position
                local_leg_target.position = Vector3.Lerp(Vector3.Lerp(start_position, center_pos, normal_time), Vector3.Lerp(center_pos, end_position, normal_time), normal_time);

                yield return null;
            }
            while (time_elapse < step_time);

            // set the start world position to start local position
            world_leg_target.position = hit.point;
            on_move = false;
            is_moved = true;
        }
    }
    public class ArmHandler : MonoBehaviour
    {
        public Transform target, local_hand_target, world_hand_target;
        public void Update()
        {
            world_hand_target.position = Vector3.Lerp(world_hand_target.position, target.position, Time.deltaTime * 8f);
            local_hand_target.position = world_hand_target.position;
        }
    }
}
