using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellButton : MonoBehaviour, IPointerClickHandler
{
    UnityEvent myEvent;

    public bool correctAnswer;
    VisualEffectsController visualEffects;
    Image buttonImage;

    private void Awake()
    {
        visualEffects = FindObjectOfType<VisualEffectsController>();
        buttonImage = GetComponent<CellSettings>().cellValue;

        if (myEvent == null)
            myEvent = new UnityEvent();
    }

    private void Start()
    {
        myEvent.AddListener(FindObjectOfType<StartOptions>().MoveToNextLevel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (correctAnswer)
            myEvent.Invoke();
        else
            visualEffects.TransformBounce(buttonImage.transform, new Vector3(-10f, 0f, 0f), 0.2f, DG.Tweening.Ease.InBounce, 6);
    }
}
