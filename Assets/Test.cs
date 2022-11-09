using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Test : MonoBehaviourPun
{
    PhotonView pun;

    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GroundSetting()
    {
        pun = PhotonView.Get(this);
        if (pun.IsMine==false)
        {
            return;
        }
        if(GameManager.INSTANCE.SCENENUM != 2)
        {
            return;
        }

        photonView.RPC("GroundInst", RpcTarget.All, SaveLoadManager.INSTANCE.convertGroundState);
    }
    
   
    Transform[] fence;
    Transform[] tree;

    [PunRPC]
    public void GroundInst(int mapState)
    {
        
        for (int i = 0; i < GameObject.Find("fence").transform.childCount; i++)
        {
            fence[i] = GameObject.Find("fence").transform.GetChild(i);
        }

        for (int i = 0; i < GameObject.Find("tree").transform.childCount; i++)
        {
            tree[i] = GameObject.Find("tree").transform.GetChild(i);
        }

        PhotonInstGround(fence, tree, mapState);
    }
    [PunRPC]
    public void PhotonInstGround(Transform[] fence, Transform[] tree, int mapState)
    {
        if (mapState == 4)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].gameObject.SetActive(true);
            tree[0].gameObject.SetActive(false);
        }
        else if (mapState == 3)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].gameObject.SetActive(true);
            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
        }
        else if (mapState == 2)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].transform.position = new Vector3(10, 10, 10);
            fence[3].gameObject.SetActive(true);
            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
            tree[2].gameObject.SetActive(false);
        }
        else if (mapState == 1)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].transform.position = new Vector3(10, 10, 10);
            fence[3].transform.position = new Vector3(10, 10, 10);
            fence[4].gameObject.SetActive(true);

            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
            tree[2].gameObject.SetActive(false);
            tree[3].gameObject.SetActive(false);
        }
        else if (mapState == 0)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].transform.position = new Vector3(10, 10, 10);
            fence[3].transform.position = new Vector3(10, 10, 10);
            fence[4].transform.position = new Vector3(10, 10, 10);
            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
            tree[2].gameObject.SetActive(false);
            tree[3].gameObject.SetActive(false);
        }
    }

}
