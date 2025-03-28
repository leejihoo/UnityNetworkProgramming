using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NoteManager : MonoBehaviour
{
    public GameObject Note;

    public Transform leftStart;
    public Transform rightStart;
    public Transform jumpStart;
    
    public Transform leftEnd;
    public Transform rightEnd;
    public Transform jumpEnd;

    public Sprite arrow;

    public List<NoteInfo> NoteInfos;
    public Queue<NoteInfo> NoteInfosQueue;
    
    private float songStartTime;
    private float currentTime;

    public AudioSource BG;
    public float commonDuration;
    public float commonCreatingTime;
    public float startDelay;
    
    public List<NoteInfo> NoteInfosForDrum;

    public GameObject LongNote;
    
    void Start()
    {
        NoteInfosQueue = new Queue<NoteInfo>();
        
    }

    [PunRPC]
    void LoadNote()
    {
        leftEnd.gameObject.GetComponent<JurgeNote>().destroyedNoteID.Clear();
        rightEnd.gameObject.GetComponent<JurgeNote>().destroyedNoteID.Clear();
        jumpEnd.gameObject.GetComponent<JurgeNote>().destroyedNoteID.Clear();
        leftEnd.gameObject.GetComponent<JurgeNote>().ResetCount();
        rightEnd.gameObject.GetComponent<JurgeNote>().ResetCount();
        jumpEnd.gameObject.GetComponent<JurgeNote>().ResetCount();
        
        currentTime = Time.time;
        songStartTime = Time.time;
        
        int player = PhotonNetwork.CurrentRoom.PlayerCount;
        // for (int i = 0; i < NoteInfos.Count; i++)
        // {
        //     NoteInfos[i].Duration = commonDuration;
        //     int actorNumber = 1+ i % player;
        //     NoteInfos[i].ActorNumber = actorNumber;
        //     NoteInfos[i].NoteCreatingTime = (i + 1) * commonCreatingTime;
        //     // if (i % 3 == 0)
        //     // {
        //     //     for (int j = 0; j < 3; j++)
        //     //     {
        //     //         var temp = new NoteInfo();
        //     //         temp.ActorNumber = NoteInfos[i].ActorNumber;
        //     //         temp.NoteCreatingTime = NoteInfos[i].NoteCreatingTime + 0.25f * j;
        //     //         temp.Duration = NoteInfos[i].Duration;
        //     //         if (NoteInfos[i].DirectionNum != 1)
        //     //         {
        //     //             temp.DirectionNum = 1;
        //     //         }
        //     //         else
        //     //         {
        //     //             temp.DirectionNum = 0;
        //     //         }
        //     //
        //     //         temp.scaleNum = 8;
        //     //         NoteInfosQueue.Enqueue(temp);
        //     //     }
        //     // }
        //     NoteInfos[i].NoteID = i;
        //     NoteInfosQueue.Enqueue(NoteInfos[i]);
        //     
        // }
        
        // 드럼
        float delay = 0.5f;
        for (int k = 0; k < 1; k++)
        {
            for (int i = 0; i < NoteInfosForDrum.Count; i++)
            {
                NoteInfo temp = new NoteInfo();
                temp.Duration= commonDuration;
                int actorNumber = 1+ i % player;
                temp.ActorNumber = actorNumber;
                temp.NoteCreatingTime =(i + 1) * commonCreatingTime + k * commonCreatingTime * NoteInfosForDrum.Count + k * delay;
                temp.scaleNum = NoteInfosForDrum[i].scaleNum;
                temp.DirectionNum = NoteInfosForDrum[i].DirectionNum;
                temp.NoteID = i + k * NoteInfosForDrum.Count;
                temp.NoteType = NoteInfosForDrum[i].NoteType;
                //Debug.Log(NoteInfosForDrum[i].NoteCreatingTime);
                NoteInfosQueue.Enqueue(temp);
            }
            
        }
        

        //Debug.Log("cur:" + currentTime);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime; // 경과 시간 계산
        // if (NoteInfosQueue.Count > 0)
        // {
        //     Debug.Log("NoteInfosQueue.Peek().NoteCreatingTime + songStartTime: " + NoteInfosQueue.Peek().NoteCreatingTime + songStartTime);
        //     Debug.Log("currentTime: " + currentTime);
        // }
        
        while (NoteInfosQueue.Count > 0 && NoteInfosQueue.Peek().NoteCreatingTime + songStartTime <= currentTime)
        {
            //Debug.Log("currentTime: " + currentTime);
            SpawnNote(NoteInfosQueue.Dequeue());
        }
    }
    
    void SpawnNote(NoteInfo note)
    {
        CreateNote2(note.ActorNumber, note.DirectionNum,note.Duration,note.scaleNum,note.NoteID,note.NoteType);
    }

    public void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        //StartCoroutine(StartBG());
        //StartCoroutine(TempCoroutine());
        
        GetComponent<PhotonView>().RPC("LoadNote",RpcTarget.All);
    }

    IEnumerator StartBG()
    {
        yield return new WaitForSeconds(startDelay);
        BG.Play();
    }

    [PunRPC]
    public void CreateNoteRPC(int randomActorNumber,  int randomNum)
    {
        CreateNote(randomActorNumber, randomNum);
    }

    IEnumerator TempCoroutine()
    {
        for (int i = 0; i < 20; i++)
        {
            int randomActorNumber = Random.Range(1, PhotonNetwork.CurrentRoom.PlayerCount+1);
            int randomNum = Random.Range(0, 3);
            GetComponent<PhotonView>().RPC("CreateNoteRPC",RpcTarget.All,randomActorNumber,randomNum);
            yield return new WaitForSeconds(2f);
        }

        yield return null;
    }
    
    public void CreateNote(int randomActorNumber, int randomNum)
    {
        GameObject newNote = Instantiate(Note);
        GameObject newNote2 = Instantiate(Note);
            
        //int randomActorNumber = 1;
        newNote.GetComponent<NoteController>().actorNumber = randomActorNumber;
        newNote2.GetComponent<NoteController>().actorNumber = randomActorNumber;
            
        if (randomActorNumber == 1)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.red;
            newNote2.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (randomActorNumber == 2)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.yellow;
            newNote2.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (randomActorNumber == 3)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.blue;
            newNote2.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (randomActorNumber == 4)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.green;
            newNote2.GetComponent<SpriteRenderer>().color = Color.green;
        }
            
        if (randomNum == 0)
        {
            newNote.GetComponent<SpriteRenderer>().sprite = arrow;
            newNote.transform.position = leftStart.transform.position;
            newNote.transform.DOMove(leftEnd.position - new Vector3(3, 0, 0), 5).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote));
            
            newNote2.transform.localScale = Vector3.one;
            newNote2.transform.position = jumpStart.transform.position;
            newNote2.transform.DOMove(jumpEnd.position - new Vector3(3,0,0), 3).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote2));
        }
        else if (randomNum == 1)
        {
            newNote.transform.localScale = Vector3.one;
            newNote.transform.position = jumpStart.transform.position;
            newNote.transform.DOMove(jumpEnd.position - new Vector3(3,0,0), 7).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote));
            
            Destroy(newNote2);
        }
        else
        {
            newNote.GetComponent<SpriteRenderer>().sprite = arrow;
            newNote.transform.Rotate(Vector3.forward,-180f);
            newNote.transform.position = rightStart.transform.position;
            newNote.transform.DOMove(rightEnd.position - new Vector3(3,0,0), 6).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote));
            
            Destroy(newNote2);
        }
    }
    
    public void CreateNote2(int randomActorNumber, int randomNum, float duration, int scaleNum, int noteID, int noteType)
    {
        GameObject newNote = null;
        if (noteType == 0)
        {
            newNote = Instantiate(Note);
        }
        else
        {
            newNote = Instantiate(LongNote);
        }
            
        //int randomActorNumber = 1;
        newNote.GetComponent<NoteController>().actorNumber = randomActorNumber;
        newNote.GetComponent<NoteController>().scaleNum = scaleNum;
        newNote.GetComponent<NoteController>().NoteID = noteID;
        
        if (randomActorNumber == 1)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (randomActorNumber == 2)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (randomActorNumber == 3)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (randomActorNumber == 4)
        {
            newNote.GetComponent<SpriteRenderer>().color = Color.green;
        }
            
        if (randomNum == 0)
        {
            newNote.GetComponent<SpriteRenderer>().sprite = arrow;
            newNote.transform.position = leftStart.transform.position;
            newNote.transform.DOMove(leftEnd.position - new Vector3(3, 0, 0), duration).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote));
        }
        else if (randomNum == 1)
        {
            newNote.transform.localScale = Vector3.one;
            newNote.transform.position = jumpStart.transform.position;
            newNote.transform.DOMove(jumpEnd.position - new Vector3(3,0,0), duration).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote));
        }
        else
        {
            newNote.GetComponent<SpriteRenderer>().sprite = arrow;
            newNote.transform.Rotate(Vector3.forward,-180f);
            newNote.transform.position = rightStart.transform.position;
            newNote.transform.DOMove(rightEnd.position - new Vector3(3,0,0), duration).SetEase(Ease.Linear).OnComplete(() => Destroy(newNote));
        }
    }
    
}


[Serializable]
public class NoteInfo
{
    public float Duration;
    public float NoteCreatingTime;
    public int ActorNumber;
    // 0,1,2 left, jump, right
    public int DirectionNum;
    //  0~7 도~ 높은 도
    public int scaleNum;
    public int NoteID;
    // 0은 기본 , 1은 롱노트
    public int NoteType;
}
