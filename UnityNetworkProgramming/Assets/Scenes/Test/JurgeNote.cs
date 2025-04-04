using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public bool isHoding;
    public bool isTailEnter;
    
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
        if (target.GetComponent<NoteController>().NoteType == 0 || target.GetComponent<NoteController>().NoteType == 1)
        {
            Destroy(target);
        }
        else // 롱노트일 경우 부모 파괴
        {
            Destroy(target.transform.parent.gameObject);
        }
        
        target = null;
        //Debug.Log("perfect");
        // foreach (var temp in destroyedNoteID)
        // {
        //     Debug.Log(temp);
        // }
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
        Debug.Log("PressMiss RPC 함수 스레드: " + Thread.CurrentThread.ManagedThreadId);
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
        if (target.GetComponent<NoteController>().NoteType == 0 || target.GetComponent<NoteController>().NoteType == 1)
        {
            Destroy(target);
        }
        else
        {
            Destroy(target.transform.parent.gameObject);
        }
        
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
        var targetNoteType = other.GetComponent<NoteController>().NoteType;
        if ( targetNoteType == 0 || targetNoteType == 1)
        {
            targets.Enqueue(other.gameObject);
        }
        else // 롱노트 끝에 닿았다면
        {
            isTailEnter = true;
        }
        
    }

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     float distance = Mathf.Abs(transform.position.x - target.transform.position.x);
    //     //Debug.Log("distance: " + distance);
    //     
    // }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("OnTriggerExit2D 함수 스레드: " + Thread.CurrentThread.ManagedThreadId);
        //isCanPress = false;
        //target = null;
        //Debug.Log("탈출");
        var targetNoteType = other.GetComponent<NoteController>().NoteType;
        
        if(targetNoteType == 2 && isTailEnter) // 롱노트 끝에서 나갔다면
        {
            isTailEnter = false;
            GetComponent<PhotonView>().RPC("PressPerfect",RpcTarget.All,target.GetComponent<NoteController>().scaleNum);
        }
        else if (targetNoteType == 1)
        {
            if (isHoding || targets.Count == 0)
            {
                return;
            }
            
            isTailEnter = false;
            GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
        }
        Debug.Log("OnTriggerExit2D");
        targets.Dequeue();
        Debug.Log("OnTriggerExit2D, " + "targets.count: " + targets.Count);
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
                if (temp.NoteType == 0)
                {
                    GetComponent<PhotonView>().RPC("PressPerfect",RpcTarget.All,temp.scaleNum);
                }
                else if(temp.NoteType == 1)
                {
                    isHoding = true;
                }
                
            }
            else if (distance < 0.8f)
            {
                if (temp.NoteType == 0)
                {
                    GetComponent<PhotonView>().RPC("PressGood",RpcTarget.All,temp.scaleNum);
                }
                else if (temp.NoteType == 1)
                {
                    isHoding = true;
                }
            }
            else
            {
                GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
            }
            
        }
    }

    public void OnTargetButtonUp(PointerEventData eventData)
    {
        isHoding = false;
        Debug.Log("OnTargetButtonUp: " + Thread.CurrentThread.ManagedThreadId);
        Debug.Log("OnTargetButtonUp1111, " + "targets.Count: " + targets.Count);
        if (target == null || targets.Count == 0)
        {
            return;
        }
        
        if (isTailEnter)
        {
            GetComponent<PhotonView>().RPC("PressPerfect",RpcTarget.All,target.GetComponent<NoteController>().scaleNum);
            isTailEnter = false;
        }
        else
        {
            targets.Dequeue();
            GetComponent<PhotonView>().RPC("PressMiss",RpcTarget.All);
            Debug.Log("OnTargetButtonUp, " + "targets.Count: " + targets.Count);
        }
    }
}
