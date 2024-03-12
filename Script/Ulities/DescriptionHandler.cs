using System.Collections;
using TMPro;
using UnityEngine;

public class DescriptionHandler : MonoBehaviour
{
    private TMP_Text text;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }
    public void SetText(string text)
    {
        StopAllCoroutines();
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
}