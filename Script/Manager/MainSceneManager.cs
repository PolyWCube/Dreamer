using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private Transform camera_root, main_camera, cube;
    [SerializeField] private Button start_button, quit_button;
    [SerializeField] private Image[] images;
    [SerializeField] private TMP_Text[] texts;
    [SerializeField] private Image screen_image;
    private void Start()
    {
        start_button.onClick.AddListener(LoadScene1);
        quit_button.onClick.AddListener(Quit);
    }
    private void Update()
    {
        camera_root.RotateAround(cube.position, Vector3.up, Time.deltaTime * 20f);
        main_camera.LookAt(cube.position);
    }
    private void LoadScene1()
    {
        StartCoroutine(ChangeScene());
        
    }
    private IEnumerator ChangeScene()
    {
        float normalized_time = 0;
        
        while (normalized_time <= 1f)
        {
            normalized_time += Time.deltaTime;
            foreach (Image image in images)
            {
                Color color = image.color;
                color.a = 1 / normalized_time;
                image.color = color;
            }
            foreach (TMP_Text text in texts)
            {
                Color color = text.color;
                color.a = 1 / normalized_time;
                text.color = color;
            }

            Color screen_alpha = screen_image.color;
            screen_alpha.a = normalized_time;
            screen_image.color = screen_alpha;

            main_camera.position = Vector3.Lerp(main_camera.position, cube.position, normalized_time / 100f);
            yield return null;
        }

        SceneManager.LoadScene(1);
    }
    private void Quit()
    {
        Application.Quit();
    }
}
