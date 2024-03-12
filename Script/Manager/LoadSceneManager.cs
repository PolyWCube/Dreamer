using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance;
    [SerializeField] private Image screen;
    [SerializeField] private GameObject[] dont_destroy_gameobjects;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform right_arm_target, left_arm_target;
    [SerializeField] private TMP_Text text;
    private Vector3 base_left_hand_position, base_right_hand_position;
    private void Start()
    {
        if (Instance == null) Instance = this;
        else
        {
            foreach (GameObject gameobject in dont_destroy_gameobjects) Destroy(gameobject);
        }
        StartCoroutine(StartScene());
        foreach (GameObject gameobject in dont_destroy_gameobjects) DontDestroyOnLoad(gameobject);
        base_left_hand_position = left_arm_target.localPosition;
        base_right_hand_position = right_arm_target.localPosition;
    }
    private IEnumerator StartScene()
    {
        float normalized_time = 0;
        Color screen_transparency = Color.black;
        screen.color = screen_transparency;
        while (normalized_time < 5f)
        {
            normalized_time += Time.deltaTime;

            screen_transparency.a = Mathf.Lerp(1f, 0f, normalized_time / 5f);
            screen.color = screen_transparency;
            yield return null;
        }
    }
    public void LoadScene(int scene_index)
    {
        StopAllCoroutines();
        StartCoroutine(EndScene(scene_index));
    }
    private IEnumerator EndScene(int index, bool is_reset = false)
    {
        float normalized_time = 0;
        Color screen_transparency = Color.black;
        screen.color = screen_transparency;
        while (normalized_time < 5f)
        {
            normalized_time += Time.deltaTime;

            screen_transparency.a = Mathf.Lerp(0f, 1f, normalized_time / 5f);
            screen.color = screen_transparency;
            yield return null;
        }
        if (index == 2)
        {
            SetText("...my dream...");
        }
        else if (index == 3)
        {
            SetText("...it's always there...");
        }
        else if (index == 4)
        {
            SetText("...wait for me...");
        }
        else if (index == 0)
        {
            SetText("...finally...");
        }

        yield return new WaitForSeconds(5f);

        if (SceneManager.sceneCount < 2f) SceneManager.LoadSceneAsync(index);
        if (is_reset || index == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            foreach (GameObject gameobject in dont_destroy_gameobjects) Destroy(gameobject);
        }
        StartCoroutine(StartScene());
        player.transform.position = new Vector3(0f, 10f, 0f);
        Invoke(nameof(SetHandPosition), 0.5f);
        
    }
    public void ResetScene()
    {
        StartCoroutine(EndScene(1, true));
    }
    private void SetText(string text)
    {
        StartCoroutine(DisplayText(text));
    }
    private IEnumerator DisplayText(string txt)
    {
        text.text = txt;
        Color alpha = Color.white;
        float normalized_time = 0f;
        while (normalized_time <= 1f)
        {
            normalized_time += Time.deltaTime;
            alpha.a = Mathf.Lerp(0f, 1f, normalized_time);
            text.color = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        normalized_time = 0f;
        while (normalized_time <= 3f)
        {
            normalized_time += Time.deltaTime;
            alpha.a = Mathf.Lerp(1f, 0f, normalized_time / 3f);
            text.color = alpha;
            yield return null;
        }
    }
    private void SetHandPosition()
    {
        left_arm_target.localPosition = base_left_hand_position;
        right_arm_target.localPosition = base_right_hand_position;
    }
}
