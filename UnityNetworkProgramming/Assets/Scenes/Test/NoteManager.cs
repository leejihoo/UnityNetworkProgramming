using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;
using Photon.Pun;
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
    
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStartButton()
    {
        StartCoroutine(TempCoroutine());
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
}

[Serializable]
public class NoteInfo
{
    public float Duration;
    public float NoteCreatingTime;
    public int ActorNumber;
    public int DirectionNum;
}
