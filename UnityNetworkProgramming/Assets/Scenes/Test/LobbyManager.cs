using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject roomUI;
    public GameObject roomListUI;
    public Dictionary<string, GameObject> NewRoomDictionary;
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.AutomaticallySyncScene = true;
        NewRoomDictionary = new Dictionary<string, GameObject>();
        
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.LoadLevel("testRoom");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("로비에서 방이 들어가짐");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                Debug.Log("방삭제");
                for (int i = roomListUI.transform.childCount - 1; i >= 0; i--)
                {
                    if (roomListUI.transform.GetChild(i).GetComponent<RoomUIInfo>().roomName.text == room.Name)
                    {
                        Destroy(roomListUI.transform.GetChild(i).gameObject);
                        break;
                    }
                }
                continue;
            }

            // 방이 이미 존재하는지 확인
            if (NewRoomDictionary.TryGetValue(room.Name,out GameObject target))
            {
                target.GetComponent<RoomUIInfo>().roomName.text = room.Name;
                target.GetComponent<RoomUIInfo>().playerCountInfo.text = room.PlayerCount + "/" + room.MaxPlayers;
                return;
            }
            
            // 방이 없으면 생성해서 넣는다.
            var newRoomUI = Instantiate(roomUI, roomListUI.transform);
            newRoomUI.transform.localScale = Vector3.one;
            NewRoomDictionary[room.Name] = newRoomUI;
            
            newRoomUI.GetComponent<RoomUIInfo>().roomName.text = room.Name;
            newRoomUI.GetComponent<RoomUIInfo>().playerCountInfo.text = room.PlayerCount + "/" + room.MaxPlayers;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방생성 실패");
    }

    public void ClickCreateRoomButton()
    {
        string roomName = "testRoom" + Random.Range(1000,9999);
        int maxPlayer = 4;
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayer };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
}
