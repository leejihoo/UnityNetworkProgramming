using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteTest : MonoBehaviour
{
    public GameObject Head;

    public GameObject Tail;

    public LineRenderer LineRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        LineRenderer.SetPosition(0,Head.transform.position);
        LineRenderer.SetPosition(1,Tail.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer.SetPosition(0,Head.transform.position);
        LineRenderer.SetPosition(1,Tail.transform.position);
    }
}
