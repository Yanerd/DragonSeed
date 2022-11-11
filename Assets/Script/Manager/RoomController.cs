using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RoomController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomName = null;
    [SerializeField] TextMeshProUGUI roomInfomation = null;

    [SerializeField] Button joinButton = null;

    public string ID { get; set; }
    public int PLAYERCOUNT { get; set; }

    //setting function
    public void RoomNameSetting(string roomName)
    {
        ID = roomName;
        this.roomName.text = "[Owner of Island]\n" + roomName;
    }

    public void RoomInfoSetting(int nowPlayer)
    {
        PLAYERCOUNT = nowPlayer;
        this.roomInfomation.text = PLAYERCOUNT.ToString() + " / 2";
        if (nowPlayer == 2)
        {
            joinButton.interactable = false;
        }
        else if (nowPlayer == 0)
        {
            joinButton.interactable = false;
        }
        else
        {
            joinButton.interactable = true;
        }
    }
    

    public void OnButtonJoin()
    {
        PhotonNetwork.JoinRoom(ID);
    }

}
