using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIInterative : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent<PointerEventData> buttonOnPointerDown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonOnPointerDown.Invoke(eventData);
    }
}
