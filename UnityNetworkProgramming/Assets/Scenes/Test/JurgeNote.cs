using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEditor;
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
    public HashSet<int> destroyedNoteID;

    public TMP_Text perfectText;
    public TMP_Text goodText;
    public TMP_Text missText;
    public static int perfectCount;
    public static int goodCount;
    public static int missCount;

    private bool isHolding;

    public void ResetCount()
    {
        perfectCount = 0;
        goodCount = 0;
        missCount = 0;
        perfectText.text = "perfect: 0";
        goodText.text = "good: 0";
        missText.text = "miss: 0";
    }
    
    // Start is called before the first frame update
    void Start()
    {
        perfectText = GameObject.Find("Perfect").GetComponent<TMP_Text>();
        goodText = GameObject.Find("Good").GetComponent<TMP_Text>();
        missText = GameObject.Find("Miss").GetComponent<TMP_Text>();
        
        isCanPress = false;
        audioSource = GetComponent<AudioSource>();
        targets = new Queue<GameObject>();
        //CreateText("hello");
        destroyedNoteID = new HashSet<int>();
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

        if (isHolding)
        {
            return;
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
                GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
                return;
            }
            
            
            if (temp.NoteType == 1)
            {
                float distance2 = Mathf.Abs(transform.position.x - target.GetComponent<NoteController>().front.position.x);
                
                if (distance2 < 0.8f)
                {
                    isHolding = true;
                    return;
                }
                
                Debug.Log("설마여기?");
                GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
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
        //Debug.Log("pressP");
        if (PhotonNetwork.IsMasterClient && target != null)
        {
            GetComponent<PhotonView>().RPC("PressPerfectInMasterClient",RpcTarget.All,scaleNum);
        }
    }
    
    [PunRPC]
    public void PressPerfectInMasterClient(int scaleNum)
    {
        
        if (target == null || destroyedNoteID.Contains(target.GetComponent<NoteController>().NoteID))
        {
            return;
        }

        perfectCount++;
        perfectText.text = $"perfect: {perfectCount}"; 
        GetComponentInChildren<ParticleSystem>().Play();
        CreateText("Perfect");
        audioSource.clip = scaleList[scaleNum];
        audioSource.Play();
        destroyedNoteID.Add(target.GetComponent<NoteController>().NoteID);
        Destroy(target);
        target = null;
        //Debug.Log("perfect");
        foreach (var temp in destroyedNoteID)
        {
            Debug.Log(temp);
        }
    }

    [PunRPC]
    public void PressGood(int scaleNum)
    {
        if (PhotonNetwork.IsMasterClient && target != null)
        {
            GetComponent<PhotonView>().RPC("PressGoodInMasterClient",RpcTarget.All,scaleNum);
        }
    }

    [PunRPC]
    public void PressGoodInMasterClient(int scaleNum)
    {
        if (target == null || destroyedNoteID.Contains(target.GetComponent<NoteController>().NoteID))
        {
            return;
        }
        
        goodCount++;
        goodText.text = $"good: {goodCount}"; 
        
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
        if (PhotonNetwork.IsMasterClient && target != null)
        {
            GetComponent<PhotonView>().RPC("PressMissInMasterClient",RpcTarget.All);
        }
    }

    [PunRPC]
    public void PressMissInMasterClient()
    {
        if (target == null || destroyedNoteID.Contains(target.GetComponent<NoteController>().NoteID))
        {
            return;
        }
        missCount++;
        missText.text = $"miss: {missCount}"; 
        
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

    private bool isExecuteRPC = false;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            Debug.Log("OnTriggerStay2D 지나감");
            isHolding = false;
        }
        
        if (!isHolding && !isExecuteRPC)
        {
            Debug.Log("!isHolding && !isExecuteRPC");
            isExecuteRPC = true;
            GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
        }
        
        if (isHolding)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
        
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isHolding)
        {
            isHolding = false;
            isExecuteRPC = false;
            GetComponent<PhotonView>().RPC("PressPerfect",RpcTarget.All,7);
        }
        
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
