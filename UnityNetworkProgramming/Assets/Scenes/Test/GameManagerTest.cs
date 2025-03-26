using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManagerTest : MonoBehaviourPunCallbacks
{
    public GameObject testPrefab;

    public TMP_Text ping;
    public TMP_Text region;

    public GameObject noteQueue;

    public GameObject leftNote;

    public GameObject rightNote;

    public GameObject jumpNote;
    
    public TMP_Text fpsText;
    private float deltaTime = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogFormat("시작했는데 연결이 안됐네");
            SceneManager.LoadScene("Lobby");
            
            return;
        }

        Application.targetFrameRate = 120;
        
        StartCoroutine(UpdateFPS());
        region.text = "Region: " + PhotonNetwork.CloudRegion;
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     int randomNumber = Random.Range(0, 3);
        //     GetComponent<PhotonView>().RPC("CreateNewNote",RpcTarget.All,randomNumber);
        // }
        
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     // 마스터 클라이언트만 자신의 객체를 생성
        //     PhotonNetwork.Instantiate(testPrefab.name, new Vector3(0, 1, 0), Quaternion.identity, 0);
        // }
    }
    
    [PunRPC]
    public void CreateNewNote(int randomNumber)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            
            GameObject target = null;
            if (randomNumber == 0)
            {
                target = leftNote;
            }
            else if (randomNumber == 1)
            {
                target = rightNote;
            }
            else if (randomNumber == 2)
            {
                target = jumpNote;
            }
      
            var newNote = Instantiate(target, noteQueue.transform.position, quaternion.identity);
            newNote.GetComponent<NoteController>().actorNumber = player.ActorNumber;
            
            if (player.ActorNumber == 1)
            {
                newNote.GetComponent<Image>().color = Color.red;
            }
            else if (player.ActorNumber == 2)
            {
                newNote.GetComponent<Image>().color = Color.yellow;
            }
            else if (player.ActorNumber == 3)
            {
                newNote.GetComponent<Image>().color = Color.blue;
            }
            else if (player.ActorNumber == 4)
            {
                newNote.GetComponent<Image>().color = Color.green;
            }
            
            newNote.transform.SetParent(noteQueue.transform);
            noteQueue.GetComponent<NoteQueueController>().NoteQueue.Enqueue(newNote);
            newNote.transform.localScale = Vector3.one;
        }
    }
    


    public override void OnEnable()
    {
        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            if (arg0.name == "testMap")
            {
                var temp = PhotonNetwork.Instantiate(testPrefab.name,new Vector3(PhotonNetwork.LocalPlayer.ActorNumber
                    , PhotonNetwork.LocalPlayer.ActorNumber, 0), Quaternion.identity,0);
                
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    temp.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                {
                    temp.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
                else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
                {
                    temp.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                else if (PhotonNetwork.LocalPlayer.ActorNumber == 4)
                {
                    temp.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }
        };
    }
    
    IEnumerator UpdateFPS()
    {
        while (true)
        {
            float fps = 1.0f / deltaTime;
            fpsText.text = "FPS: " + Mathf.Ceil(fps);
            yield return new WaitForSeconds(0.5f); // 0.5초마다 업데이트
        }
    }

    // Update is called once per frame
    void Update()
    {
        ping.text = "Ping: " + PhotonNetwork.GetPing();
        
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //PhotonNetwork.Instantiate(testPrefab.name,new Vector3(0, 1, 0), Quaternion.identity,0);
        Debug.Log("이건 된다");
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        //     PhotonNetwork.Instantiate(testPrefab.name,new Vector3(0, 1, 0), Quaternion.identity,0);
        //     //PhotonNetwork.LoadLevel("testMap");
        // }
    }

    public override void OnJoinedRoom()
    {
        
    }
}
