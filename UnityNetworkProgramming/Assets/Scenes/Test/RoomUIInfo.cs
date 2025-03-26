using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomUIInfo : MonoBehaviour
{
    public TMP_Text roomName;

    public TMP_Text playerCountInfo;

    public void OnClickEnterRoomButton()
    {
        PhotonNetwork.JoinRoom(roomName.text);
        PhotonNetwork.LoadLevel("testRoom");
    }
    
}
