using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class UserInfo : MonoBehaviourPunCallbacks
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string SessionId { get; set; }
    [field: SerializeField] public int Zera { get; set; }
    [field: SerializeField] public string WalletAdress { get; set; }
    [field: SerializeField] public string BettingId { get; set; }
    //====================================================================
    [field: SerializeField] public string MasterName { get; set; }
    [field: SerializeField] public string MasterSessionId { get; set; }
    [field: SerializeField] public string MasterWalletAdress { get; set; }
    [field: SerializeField] public string MasterBettingId { get; set; }
    //====================================================================
    [field: SerializeField] public string OtherName { get; set; }
    [field: SerializeField] public string OtherSessionId { get; set; }
    [field: SerializeField] public string OtherWalletAdress { get; set; }
    [field: SerializeField] public string OtherBettingId { get; set; }

    bool BettingToken = true;
    bool WinToken = true;
    bool DisConnectToken = true;

    private void Start()
    {
        Invoke("UserInfoUpdate", 2f);
    }
    void Update()
    {
        // ���潺���� �Ǿ��� ��
        // other �� ������ �ִ� userInfo�� master ���� �ѱ��.
        // master�� ������ �ִ� info�� other���� �ѱ��.
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.INVASIONALLOW) // ħ�Դ��Ҷ�
        {
            Debug.Log("���޽���");
            // �����Լ�����
            photonView.RPC("TransferMasterName", RpcTarget.All, Name);
            photonView.RPC("TransferMasterWalletAdress", RpcTarget.All, WalletAdress);
            photonView.RPC("TransferMasterBettingId", RpcTarget.All, BettingId);

        }
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.WANTINVASION) //ħ���Ҷ�
        {
            photonView.RPC("TransferOtherName", RpcTarget.All, Name);
            photonView.RPC("TransferOtherWalletAdress", RpcTarget.All, WalletAdress);
            photonView.RPC("TransferOtherBettingId", RpcTarget.All, BettingId);

        }

        ////������ ���������� ������ �� 
        ////���ǿ� �°� �¸��� api ȣ��
        //if (GameManager.INSTANCE.INVASIONALLOW) // ���� �������϶�
        //{
        //    if (GameManager.INSTANCE.ISGAMEIN && BettingToken) // ��Ʋ������
        //    {
        //        BettingToken = false;
        //
        //      MetaTrendAPIHandler.INSTANCE.Betting_Zera(MasterBettingId);
        //      MetaTrendAPIHandler.INSTANCE.Betting_Zera(OtherBettingId);
        //    }
        //}
        //
        ////��������
        //if (GameManager.INSTANCE.ISDEAD && WinToken) // ħ���� �÷��̾ �׾��� ��
        //{
        //    WinToken = false;
        //    //master win
        //    //other�� zera�� master���� �ش�.
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(MasterBettingId);
        //}
        //else if (GameManager.INSTANCE.ISTIMEOVER && WinToken) // Ÿ�ӿ����� ȣ�������
        //{
        //    WinToken = false;
        //    //invader/otherclient win
        //    // master�� zera�� other���� �ش�.
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(OtherBettingId);
        //    // �޾ƿ� id�� ���� �������� �����ߴ� zera�� �ѱ��.
        //    photonView.RPC("TransferOtherName", RpcTarget.All, Name);
        //}
        //
        //
        //// ������ ������������ ������ ��
        //if (!GameManager.INSTANCE.GameEndCorrect && DisConnectToken)
        //{
        //    DisConnectToken = false;
        //    // ���ñݾ׹�ȯ - �����ߴ� Zera�� ���ڿ��� �ǵ�����                     
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(MasterBettingId);
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(OtherBettingId);
        //}

    }
    // Name
    [PunRPC]
    public void TransferMasterName(string name)
    {
        MasterName = name;
    }
    [PunRPC]
    public void TransferOtherName(string name)
    {
        OtherName = name;
    }
    // WalletAdress
    [PunRPC]
    public void TransferMasterWalletAdress(string adress)
    {
        MasterWalletAdress = adress;
    }
    [PunRPC]
    public void TransferOtherWalletAdress(string adress)
    {
        OtherWalletAdress = adress;
    }
    // BettingId
    [PunRPC]
    public void TransferMasterBettingId(string bettingId)
    {
        MasterBettingId = bettingId;
    }
    [PunRPC]
    public void TransferOtherBettingId(string bettingId)
    {
        OtherBettingId = bettingId;
    }

    void UserInfoUpdate()
    {
        Name = MetaTrendAPIHandler.INSTANCE.USERNAME;
        SessionId = MetaTrendAPIHandler.INSTANCE.SESSIONID;
        Zera = MetaTrendAPIHandler.INSTANCE.ZERA;
        WalletAdress = MetaTrendAPIHandler.INSTANCE.WALLETADRESS;
    }
}