using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffectsController : MonoBehaviour
{  
    public void TransformBounce(Transform goTransform, Vector3 direction, float duration, Ease ease, int times)
    {
        goTransform.DOLocalMove(direction, duration).SetEase(Ease.InOutSine).SetLoops(times, LoopType.Yoyo);
    }

    public void TextFade(TextMeshProUGUI text, float value, float duration)
    {
        text.DOFade(value, duration);
    }

    public void ImageFade(Image image, float value, float duration)
    {
        image.DOFade(value, duration);
    }
}
