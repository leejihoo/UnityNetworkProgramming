using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour, IPunObservable
{
    public bool isReady;
    public int actorNumber;
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            isReady = false;
            Hashtable initialProps = new Hashtable() {{"isReady", isReady}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        }
    }

    public void OnReadyButtonClicked()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            SetPlayerReady(!isReady);
            Hashtable initialProps = new Hashtable() {{"isReady", isReady}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isReady);    
        }
        else
        {
            isReady = (bool)stream.ReceiveNext();
        }
    }

    public void SetPlayerReady(bool isPlayerReady)
    {
        isReady = isPlayerReady;
        
        if (isPlayerReady)
        {
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}
