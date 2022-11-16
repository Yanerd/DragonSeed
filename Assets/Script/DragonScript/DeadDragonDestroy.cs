using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadDragonDestroy : MonoBehaviourPun
{
    void Awake()
    {
        if(photonView.IsMine)
            StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(this.gameObject);
    }
  
}
