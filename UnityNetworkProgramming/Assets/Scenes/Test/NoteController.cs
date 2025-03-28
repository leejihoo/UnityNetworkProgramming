using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using DG.Tweening;

public class NoteController: MonoBehaviour
{
    public string direction;
    public int actorNumber;
    public int scaleNum;
    public int NoteID;

    public Transform front;
    public Transform end;
    
    // 0은 단타, 1이 롱노트
    public int NoteType;
    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
