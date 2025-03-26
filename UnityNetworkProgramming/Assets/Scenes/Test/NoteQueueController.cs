using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NoteQueueController : PhotonView
{
    public Queue<GameObject> NoteQueue = new Queue<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (!NoteQueue.TryPeek(out GameObject top))
        {
            //Debug.Log("queue가 비었습니다.");    
            return;
        }

        Debug.Log("통과함");
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Input.GetKeyDown(KeyCode.LeftArrow)");
            var temp = top.GetComponent<NoteController>();
            
            if (temp.direction == "left"  && temp.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                GetComponent<PhotonView>().RPC("DequeueRPC",RpcTarget.All);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Input.GetKeyDown(KeyCode.RightArrow)");
            var temp = top.GetComponent<NoteController>();
            
            if (temp.direction == "right"  && temp.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                GetComponent<PhotonView>().RPC("DequeueRPC",RpcTarget.All);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Input.GetKeyDown(KeyCode.Space)");
            var temp = top.GetComponent<NoteController>();
            
            if (temp.direction == "jump"  && temp.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                GetComponent<PhotonView>().RPC("DequeueRPC",RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void DequeueRPC()
    {
        var dequeuedObject = NoteQueue.Dequeue();
        Destroy(dequeuedObject);
    }
}
