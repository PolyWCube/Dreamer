using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private Transform pivot_transform;
    [SerializeField] private float interact_angle = 90f, interact_speed = 2f;
    [SerializeField] private bool is_vertical;

    private Quaternion initial_rotation;
    private Quaternion target_rotation;
    private bool is_interacted = false, is_interactable = true;
    private void Start()
    {
        initial_rotation = pivot_transform.rotation;
        if (is_vertical) target_rotation = Quaternion.Euler(pivot_transform.eulerAngles + new Vector3(0f, 0f, interact_angle));
        else target_rotation = Quaternion.Euler(pivot_transform.eulerAngles + new Vector3(0f, interact_angle, 0f));
    }
    public void Interact()
    {
        if (!is_interacted && is_interactable)
        {
            is_interacted = true;
            StartCoroutine(UsingInteractableObject(initial_rotation, target_rotation));
            is_interactable = false;
            Invoke(nameof(Reset), 1f);
        }
        else if (is_interacted && is_interactable)
        {
            is_interacted = false;
            StartCoroutine(UsingInteractableObject(target_rotation, initial_rotation));
            is_interactable = false;
            Invoke(nameof(Reset), 1f);
        }
    }

    private IEnumerator UsingInteractableObject(Quaternion initial_rotation, Quaternion target_rotation)
    {
        float normalized_time = 0f;

        while (normalized_time < 1f)
        {
            normalized_time += Time.deltaTime * interact_speed;
            pivot_transform.rotation = Quaternion.Lerp(initial_rotation, target_rotation, normalized_time);
            yield return null;
        }
    }
    private void Reset()
    {
        is_interactable = true;
    }
}
