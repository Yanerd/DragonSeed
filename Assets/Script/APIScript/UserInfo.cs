using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class UserInfo : MonoBehaviourPunCallbacks
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string WalletAdress { get; set; }
    [field: SerializeField] public int Zera { get; set; }
    [field: SerializeField] public string SessionId { get; set; }
    [field: SerializeField] public string _Id { get; set; }
    [field: SerializeField] public string Player_ID { get; set; }
    [field: SerializeField] public string BettingId { get; set; }
    //====================================================================
    [field: SerializeField] public string MasterName { get; set; }
    [field: SerializeField] public string MasterPlayer_ID { get; set; }
    [field: SerializeField] public string MasterSessionId { get; set; }
    [field: SerializeField] public string MasterBettingId { get; set; }
    //====================================================================
    [field: SerializeField] public string OtherName { get; set; }
    [field: SerializeField] public string OtherPlayer_ID { get; set; }
    [field: SerializeField] public string OtherSessionId { get; set; }
    [field: SerializeField] public string OtherBettingId { get; set; }

    bool BettingToken = true;
    bool WinToken = true;
    bool DisConnectToken = true;
    bool isMaster = false;

    private void Awake()
    {
        SettingClear();
    }

    private void Start()
    {
        Invoke("UserInfoUpdate", 2f);
    }
    void Update()
    {
        UserInfoUpdate();

        // transfer data
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.INVASIONALLOW) // master
        {
            isMaster = true;
            photonView.RPC("SetMasterName", RpcTarget.All, Name);
            photonView.RPC("SetMasterPlayer_Id", RpcTarget.All, Player_ID);
            photonView.RPC("SetMasterSessionId", RpcTarget.All, SessionId);
        }
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.WANTINVASION) // other
        {
            photonView.RPC("SetOtherName", RpcTarget.All, Name);
            photonView.RPC("SetOtherPlayer_Id", RpcTarget.All, Player_ID);
            photonView.RPC("SetOtherSessionId", RpcTarget.All, SessionId);
        }

        // betting start
        if (OtherSessionId != null && MasterSessionId != null && GameManager.INSTANCE.SCENENUM == 2 && BettingToken) // �����͹�Ʋ������
        {
            StartCoroutine(betting());
        }

        if (!isMaster) return;

        if (GameManager.INSTANCE.ISDEAD && WinToken) // ħ���� �÷��̾ �׾��� ��
        {
            WinToken = false;
            //master win
            //MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(MasterBettingId);
            MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(MasterBettingId, MasterPlayer_ID);
            SettingClear();
        }
        else if (GameManager.INSTANCE.ISTIMEOVER && WinToken) // Ÿ�ӿ����� ȣ�������
        {
            WinToken = false;
            //otherclient win
            //MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(MasterBettingId);
            MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(OtherBettingId, OtherPlayer_ID);
            SettingClear();
        }
        
        ////// ������ ������������ ������ ��
        //if (GameManager.INSTANCE.SCENENUM == 2 &&!GameManager.INSTANCE.GameEndCorrect && DisConnectToken)
        //{
        //    DisConnectToken = false;
        //    // ���ñݾ׹�ȯ - �����ߴ� Zera�� ���ڿ��� �ǵ�����                     
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(MasterBettingId);
        //}
    }

    IEnumerator betting()
    {
        BettingToken = false;

        string[] array = { MasterSessionId, OtherSessionId };

        yield return MetaTrendAPIHandler.INSTANCE.processRequestBetting_Zera(_Id,array);

        BettingId = MetaTrendAPIHandler.INSTANCE.BETTINGID;

        if(GameManager.INSTANCE.INVASIONALLOW) photonView.RPC("SetMasterBettingId", RpcTarget.All, BettingId);
        if(GameManager.INSTANCE.WANTINVASION) photonView.RPC("SetOtherBettingId", RpcTarget.All, BettingId);
    }

    // Name
    [PunRPC]
    public void SetMasterName(string name)
    {
        MasterName = name;
    }
    [PunRPC]
    public void SetOtherName(string name)
    {
        OtherName = name;
    }
    [PunRPC]
    public void SetMasterPlayer_Id(string player_Id)
    {
        MasterPlayer_ID = player_Id;
    }
    [PunRPC]
    public void SetOtherPlayer_Id(string player_Id)
    {
        OtherPlayer_ID = player_Id;
    }
    // SessionId
    [PunRPC]
    public void SetMasterSessionId(string sessionId)
    {
        MasterSessionId = sessionId;
    }
    [PunRPC]
    public void SetOtherSessionId(string sessionId)
    {
        OtherSessionId = sessionId;
    }
    // BettingId
    [PunRPC]
    public void SetMasterBettingId(string bettingId)
    {
        MasterBettingId = bettingId;
    }
    [PunRPC]
    public void SetOtherBettingId(string bettingId)
    {
        OtherBettingId = bettingId;
    }

    void UserInfoUpdate()
    {
        Name = MetaTrendAPIHandler.INSTANCE.USERNAME;
        Player_ID = MetaTrendAPIHandler.INSTANCE.Player_ID;
        SessionId = MetaTrendAPIHandler.INSTANCE.SESSIONID;
        Zera = MetaTrendAPIHandler.INSTANCE.ZERA;
        WalletAdress = MetaTrendAPIHandler.INSTANCE.WALLETADRESS;
        _Id = MetaTrendAPIHandler.INSTANCE._ID;
    }

    void SettingClear()
    {
        this.MasterName = null;
        this.MasterSessionId = null;
        this.MasterBettingId = null;

        this.OtherName = null;
        this.OtherSessionId = null;
        this.OtherBettingId = null;

        this.WinToken = true;
        this.BettingToken = true;
        this.DisConnectToken = true;
        this.isMaster = false;
    }
}