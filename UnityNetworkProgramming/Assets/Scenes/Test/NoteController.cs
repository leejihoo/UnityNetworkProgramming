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
    
    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
