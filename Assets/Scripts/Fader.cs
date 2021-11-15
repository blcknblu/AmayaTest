using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] float time = 1f;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeOut()
    {
        while (canvasGroup.alpha < 1)
        {
            yield return null;
            canvasGroup.alpha += Time.deltaTime / time;
        }
    }

    public IEnumerator FadeIn()
    {
        while (canvasGroup.alpha > 0)
        {
            yield return null;
            canvasGroup.alpha -= Time.deltaTime / time;
        }
    }
}
