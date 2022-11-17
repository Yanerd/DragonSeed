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
    [field: SerializeField] public string BettingId { get; set; }
    //====================================================================
    [field: SerializeField] public string MasterName { get; set; }
    [field: SerializeField] public string MasterSessionId { get; set; }
    [field: SerializeField] public string Master_Id { get; set; }
    [field: SerializeField] public string MasterBettingId { get; set; }
    //====================================================================
    [field: SerializeField] public string OtherName { get; set; }
    [field: SerializeField] public string OtherSessionId { get; set; }
    [field: SerializeField] public string Other_Id { get; set; }
    [field: SerializeField] public string OtherBettingId { get; set; }

    bool BettingToken = true;
    bool WinToken = true;
    bool DisConnectToken = true;

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
        // transfer data
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.INVASIONALLOW) // master
        {
            photonView.RPC("SetMasterName", RpcTarget.All, Name);
            photonView.RPC("SetMasterSessionId", RpcTarget.All, SessionId);
            photonView.RPC("SetMaster_Id",RpcTarget.All,_Id);
        }
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.WANTINVASION) // other
        {
            photonView.RPC("SetOtherName", RpcTarget.All, Name);
            photonView.RPC("SetOtherSessionId", RpcTarget.All, SessionId);
            photonView.RPC("SetOther_Id", RpcTarget.All, _Id);
        }

        // betting start
        if (OtherSessionId != null && MasterSessionId != null && GameManager.INSTANCE.SCENENUM == 2 && BettingToken) // �����͹�Ʋ������
        {
            StartCoroutine(betting());
        }

        ////��������
        //if (GameManager.INSTANCE.ISDEAD && WinToken) // ħ���� �÷��̾ �׾��� ��
        //{
        //    WinToken = false;
        //    //master win
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(MasterBettingId, Master_Id);
        //}
        //else if (GameManager.INSTANCE.ISTIMEOVER && WinToken) // Ÿ�ӿ����� ȣ�������
        //{
        //    WinToken = false;
        //    //otherclient win
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(MasterBettingId, Other_Id);
        //}
        
        
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

        yield return MetaTrendAPIHandler.INSTANCE.processRequestBetting_Zera(this._Id,array);

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
    [PunRPC]
    public void SetMaster_Id(string _Id)
    {
        Master_Id = _Id;
    }
    [PunRPC]
    public void SetOther_Id(string _Id)
    {
        Other_Id = _Id;
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
        SessionId = MetaTrendAPIHandler.INSTANCE.SESSIONID;
        Zera = MetaTrendAPIHandler.INSTANCE.ZERA;
        WalletAdress = MetaTrendAPIHandler.INSTANCE.WALLETADRESS;
        _Id = MetaTrendAPIHandler.INSTANCE._ID;
    }

    void SettingClear()
    {
        this.MasterName = null;
        this.MasterSessionId = null;
        this.Master_Id = null;
        this.MasterBettingId = null;

        this.OtherName = null;
        this.OtherSessionId = null;
        this.Other_Id = null;
        this.OtherBettingId = null;

        this.WinToken = true;
        this.BettingToken = true;
        this.DisConnectToken = true;
    }
}