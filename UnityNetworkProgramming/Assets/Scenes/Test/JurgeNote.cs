using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class JurgeNote : MonoBehaviour
{
    public bool isCanPress;
    public GameObject target;
    public KeyCode firstKeyCode;
    public KeyCode secondKeyCode;

    public AudioClip perfect;

    public AudioClip good;

    public AudioClip miss;
    public AudioSource audioSource;

    public Transform canvas;

    public GameObject judgeText;
    public List<AudioClip> scaleList;
    public Queue<GameObject> targets;
    
    // Start is called before the first frame update
    void Start()
    {
        isCanPress = false;
        audioSource = GetComponent<AudioSource>();
        targets = new Queue<GameObject>();
        //CreateText("hello");
    }

    // Update is called once per frame
    void Update()
    {
        if (targets.Count > 0)
        {
            isCanPress = true;
            // float min = 0;
            // foreach(var cur in targets)
            // {
            //     float curDistance = CalculateDifferenceXPos(transform, cur.transform);
            //     
            // }

            target = targets.Peek();
        }
        else
        {
            isCanPress = false;
        }
        
        if (isCanPress && (Input.GetKeyDown(firstKeyCode) || Input.GetKeyDown(secondKeyCode)))
        {
            if (target == null)
            {
                return;
            }
            
            var temp = target.GetComponent<NoteController>();
            //var tempGameObject = target;
            
            if (temp.actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }
            
            float distance = Mathf.Abs(transform.position.x - target.transform.position.x);
            if (distance < 0.6f)
            {
                GetComponent<PhotonView>().RPC("PressPerfect",RpcTarget.All,temp.scaleNum);
            }
            else if (distance < 0.8f)
            {
                GetComponent<PhotonView>().RPC("PressGood",RpcTarget.All,temp.scaleNum);
            }
            else
            {
                GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
            }
            
        }
        
    }

    public float CalculateDifferenceXPos(Transform first, Transform second)
    {
        return Mathf.Abs(first.position.x - second.position.x);
    }

    [PunRPC]
    public void PressPerfect(int scaleNum)
    {
        GetComponentInChildren<ParticleSystem>().Play();
        CreateText("Perfect");
        audioSource.clip = scaleList[scaleNum];
        audioSource.Play();
        Destroy(target);
        target = null;
        Debug.Log("perfect");
    }

    [PunRPC]
    public void PressGood(int scaleNum)
    {
        GetComponentInChildren<ParticleSystem>().Play();
        CreateText("Good");
        audioSource.clip = scaleList[scaleNum];
        audioSource.Play();
        Destroy(target);
        target = null;
        Debug.Log("good");
    }
    
    [PunRPC]
    public void PressMiss()
    {
        CreateText("Miss");
        audioSource.clip = miss;
        audioSource.Play();
        Destroy(target);
        target = null;
        Debug.Log("miss");
    }

    public void CreateText(string text)
    {
        var temp = Instantiate(judgeText, canvas);
        temp.transform.position = transform.position;
        temp.GetComponent<JudgmentText>().Show(text,Color.yellow);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //isCanPress = true;
        //target = other.gameObject;
        //Debug.Log("진입");
        
        targets.Enqueue(other.gameObject);
    }

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     float distance = Mathf.Abs(transform.position.x - target.transform.position.x);
    //     //Debug.Log("distance: " + distance);
    //     
    // }

    private void OnTriggerExit2D(Collider2D other)
    {
        //isCanPress = false;
        //target = null;
        //Debug.Log("탈출");
        targets.Dequeue();
    }

    public void OnTargetButtonDown(PointerEventData eventData)
    {
        if (isCanPress)
        {
            if (target == null)
            {
                return;
            }
            
            var temp = target.GetComponent<NoteController>();
            //var tempGameObject = target;
            
            if (temp.actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }
            
            float distance = Mathf.Abs(transform.position.x - target.transform.position.x);
            if (distance < 0.6f)
            {
                GetComponent<PhotonView>().RPC("PressPerfect",RpcTarget.All,temp.scaleNum);
            }
            else if (distance < 0.8f)
            {
                GetComponent<PhotonView>().RPC("PressGood",RpcTarget.All,temp.scaleNum);
            }
            else
            {
                GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
            }
            
        }
    }
}
