using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TestConnector : MonoBehaviourPunCallbacks
{
    
    public GameObject playerEntrytPrefab;

    public GameObject playerGroupPrefab;
    //public TMP_Text log;
    public Dictionary<int, GameObject> playerList;
    private void Awake()
    {
        playerList = new Dictionary<int, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Room Start 작동");
        //PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SerializationRate = 30;
        StartCoroutine(CreatePlayerListUI());
    }

    IEnumerator CreatePlayerListUI()
    {
        //방에 접속할 때까지 대기
        while (!PhotonNetwork.InRoom)
        {
            yield return null;
        }
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerEntrytPrefab);
            entry.transform.SetParent(playerGroupPrefab.transform);
            entry.GetComponent<PlayerList>().actorNumber = player.ActorNumber;
            entry.transform.localScale = Vector3.one;
            object outValue;
            if (player.CustomProperties.TryGetValue("isReady", out outValue))
            {
                entry.GetComponent<PlayerList>().SetPlayerReady((bool)outValue);
            }
            
            playerList.Add(player.ActorNumber, entry);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //log.text += "OnJoinRandomFailed\n";
        Debug.Log("랜덤 방에 들어가는 걸 실패했습니다.");
        PhotonNetwork.CreateRoom("testRoom", new RoomOptions{MaxPlayers = 4});
        
    }

    public override void OnJoinedRoom()
    {
        //log.text += "OnJoinedRoom\n";
        Debug.Log("이게 실행이 되어야 되는데?");
        //PhotonNetwork.Instantiate(testPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        // if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        // {
        //      // "GameScene"은 게임 플레이 씬의 이름
        // }
        //PhotonNetwork.LoadLevel("testMap");
        //PhotonNetwork.Instantiate("PlayerList",)

        // foreach (var player in PhotonNetwork.PlayerList)
        // {
        //     GameObject entry = Instantiate(playerEntrytPrefab);
        //     entry.transform.SetParent(playerGroupPrefab.transform);
        //     entry.GetComponent<PlayerList>().actorNumber = player.ActorNumber;
        //     entry.transform.localScale = Vector3.one;
        //     object outValue;
        //     if (player.CustomProperties.TryGetValue("isReady", out outValue))
        //     {
        //         entry.GetComponent<PlayerList>().SetPlayerReady((bool)outValue);
        //     }
        //     
        //     playerList.Add(player.ActorNumber, entry);
        // }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //log.text += "OnPlayerEnteredRoom\n";
        Debug.Log(newPlayer.NickName + "가 들어왔습니다.");
        GameObject entry = Instantiate(playerEntrytPrefab);
        entry.transform.SetParent(playerGroupPrefab.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerList>().actorNumber = newPlayer.ActorNumber;
        
        playerList.Add(newPlayer.ActorNumber, entry);
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        GameObject entry;
        if (playerList.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue("isReady", out isPlayerReady))
            {
                entry.GetComponent<PlayerList>().SetPlayerReady((bool) isPlayerReady);
            }
        }
    }

    public void StartGame()
    {
        //방장이 아니면 시작할 수 없다.
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("testMap");
    }
}
