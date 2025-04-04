using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIInterative : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent<PointerEventData> buttonOnPointerDown;
    public UnityEvent<PointerEventData> buttonOnPointerUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonOnPointerDown.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonOnPointerUp.Invoke(eventData);
    }
}
