using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public float jumpPower = 5;
    public float movePower = 2;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public SpriteRenderer SpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        LocalPlayerInstance = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
        float h = Input.GetAxis("Horizontal");
        
        //gameObject.transform.Translate(h*Time.deltaTime*movePower,0,0);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(h * movePower * Time.deltaTime, 0, 0), movePower * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0,jumpPower),ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SpriteRenderer.color.r);
            stream.SendNext(SpriteRenderer.color.g);
            stream.SendNext(SpriteRenderer.color.b);
            stream.SendNext(SpriteRenderer.color.a);
        }
        else if (stream.IsReading)
        {
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            float a = (float)stream.ReceiveNext();

            // 받은 값으로 색상 구성
            Color color = new Color(r, g, b, a);
            SpriteRenderer.color = color;
        }
    }
}
