using UnityEngine;

public class BreakableObjectHandler : MonoBehaviour
{
    public GameObject[] affected_gameobjects;
    public bool is_break;
    [SerializeField] private AudioClip break_fx;
    [SerializeField] private float break_force, break_radius;
    private float mass;
    private AudioSource audio_source;
    private void Start()
    {
        mass = GetComponent<Rigidbody>().mass;
        if (break_fx)
        {
            audio_source = GetComponent<AudioSource>();
            audio_source.clip = break_fx;
        }
    }
    private void Update()
    {
        if (is_break)
        {
            foreach (GameObject game_object in affected_gameobjects)
            {
                Rigidbody rigidbody = game_object.AddComponent<Rigidbody>();
                rigidbody.mass = mass;
                rigidbody.AddExplosionForce(break_force, transform.position, break_radius);
            }
            audio_source?.Play();
            
            Destroy(this);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        is_break = true;
    }
}
