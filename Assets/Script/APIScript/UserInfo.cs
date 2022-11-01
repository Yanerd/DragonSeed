using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class UserInfo : MonoBehaviourPunCallbacks
{
    public string masterSessionId;
    public string masterName;
    public int masterZera;
    public string masterWalletAdress;
    public string masterBettingId;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("UserInfoUpdate", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        PhotonManager.INSTANCE.testName = masterName;
    }

    void UserInfoUpdate()
    {
        masterSessionId = MetaTrendAPIHandler.INSTANCE.SESSIONID;
        masterName = MetaTrendAPIHandler.INSTANCE.USERNAME;
        masterZera = MetaTrendAPIHandler.INSTANCE.ZERA;
        masterWalletAdress = MetaTrendAPIHandler.INSTANCE.WALLETADRESS;
        masterBettingId = MetaTrendAPIHandler.INSTANCE.BETTINGID;
    }
}
