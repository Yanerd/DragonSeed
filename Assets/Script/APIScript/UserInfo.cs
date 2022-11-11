using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class UserInfo : MonoBehaviourPunCallbacks
{
    [field: SerializeField] public string SessionId { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Zera { get; set; }
    [field: SerializeField] public string WalletAdress { get; set; }
    [field: SerializeField] public string BettingId { get; set; }

    [field: SerializeField] public string masterSessionId { get; set; }
    [field: SerializeField] public string masterName { get; set; }
    [field: SerializeField] public int masterZera { get; set; }
    [field: SerializeField] public string masterWalletAdress { get; set; }
    [field: SerializeField] public string masterBettingId { get; set; }

    [field: SerializeField] public string otherSessionId { get; set; }
    [field: SerializeField] public string otherName { get; set; }
    [field: SerializeField] public int otherZera { get; set; }
    [field: SerializeField] public string otherWalletAdress { get; set; }
    [field: SerializeField] public string otherBettingId { get; set; }

    void Start()
    {
        Invoke("UserInfoUpdate", 2f);
    }

    bool bettingToken = true;
    bool WinToken = true;
    bool DisConnectToken = true;

    void Update()
    {
        PhotonManager.INSTANCE.testName = masterName;
        // ���潺���� �Ǿ��� ��
        // other �� ������ �ִ� userInfo�� master ���� �ѱ��.
        // master�� ������ �ִ� info�� other���� �ѱ��.
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.INVASIONALLOW) // ħ�Դ��Ҷ�
        {
            Debug.Log("���޽���");
            // �����Լ�����
            photonView.RPC("TransferMasterName", RpcTarget.All, Name);
        }
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.WANTINVASION) //ħ���Ҷ�
        {
            photonView.RPC("TransferOtherName", RpcTarget.All, Name);
        }

        //������ ���������� ������ �� 
        //���ǿ� �°� �¸��� api ȣ��
        if (GameManager.INSTANCE.INVASIONALLOW) // ���� �������϶�
        {
            if (GameManager.INSTANCE.ISGAMEIN && bettingToken) // ��Ʋ������
            {
                bettingToken = false;

                MetaTrendAPIHandler.INSTANCE.Betting_Zera(masterBettingId);
                MetaTrendAPIHandler.INSTANCE.Betting_Zera(otherBettingId);
            }
        }

        //��������
        if (GameManager.INSTANCE.ISDEAD && WinToken) // ħ���� �÷��̾ �׾��� ��
        {
            WinToken = false;
            //master win
            // other�� zera�� master���� �ش�.
            MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(masterBettingId);
        }
        else if (GameManager.INSTANCE.ISTIMEOVER && WinToken) // Ÿ�ӿ����� ȣ�������
        {
            WinToken = false;
            //invader/otherclient win
            // master�� zera�� other���� �ش�.
            MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(otherBettingId);
            // �޾ƿ� id�� ���� �������� �����ߴ� zera�� �ѱ��.
            photonView.RPC("TransferOtherName", RpcTarget.All, Name);
        }


        // ������ ������������ ������ ��
        if (!GameManager.INSTANCE.GameEndCorrect && DisConnectToken)
        {
            DisConnectToken = false;
            // ���ñݾ׹�ȯ - �����ߴ� Zera�� ���ڿ��� �ǵ�����                     
            MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(masterBettingId);
            MetaTrendAPIHandler.INSTANCE.Betting_Zera_Disconnect(otherBettingId);
        }

    }
    // Name
    [PunRPC]
    public void TransferMasterName(string name)
    {
        masterName = name;
    }
    [PunRPC]
    public void TransferOtherName(string name)
    {
        otherName = name;
    }
    // ZERA
    [PunRPC]
    public void TransferMasterZera(int zera)
    {
        masterZera = zera;
    }
    [PunRPC]
    public void TransferOtherZera(int zera)
    {
        otherZera = zera;
    }
    // WalletAdress
    [PunRPC]
    public void TransferOtherWalletAdress(string adress)
    {
        otherWalletAdress = adress;
    }
    [PunRPC]
    public void TransferMasterWalletAdress(string adress)
    {
        masterWalletAdress = adress;
    }
    // BettingId
    [PunRPC]
    public void TransferOtherBettingId(string bettingId)
    {
        otherBettingId = bettingId;
    }
    [PunRPC]
    public void TransferMasterBettingId(string bettingId)
    {
        masterBettingId = bettingId;
    }

    void UserInfoUpdate()
    {
        SessionId = MetaTrendAPIHandler.INSTANCE.SESSIONID;
        Name = MetaTrendAPIHandler.INSTANCE.USERNAME;
        Zera = MetaTrendAPIHandler.INSTANCE.ZERA;
        WalletAdress = MetaTrendAPIHandler.INSTANCE.WALLETADRESS;
        BettingId = MetaTrendAPIHandler.INSTANCE.BETTINGID;
    }
}