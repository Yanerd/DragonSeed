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
        // 오펜스씬이 되었을 때
        // other 가 가지고 있는 userInfo를 master 에게 넘긴다.
        // master가 가지고 있는 info를 other에게 넘긴다.
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.INVASIONALLOW) // 침입당할때
        {
            Debug.Log("전달시작");
            // 전달함수실행
            photonView.RPC("TransferMasterName", RpcTarget.All, Name);
            photonView.RPC("TransferMasterWalletAdress", RpcTarget.All, WalletAdress);
            photonView.RPC("TransferMasterBettingId", RpcTarget.All, BettingId);

        }
        if (GameManager.INSTANCE.SCENENUM == 2 && GameManager.INSTANCE.WANTINVASION) //침입할때
        {
            photonView.RPC("TransferOtherName", RpcTarget.All, Name);
            photonView.RPC("TransferOtherWalletAdress", RpcTarget.All, WalletAdress);
            photonView.RPC("TransferOtherBettingId", RpcTarget.All, BettingId);

        }

        ////게임이 정상적으로 끝났을 때 
        ////조건에 맞게 승리자 api 호출
        //if (GameManager.INSTANCE.INVASIONALLOW) // 내가 마스터일때
        //{
        //    if (GameManager.INSTANCE.ISGAMEIN && BettingToken) // 배틀씬들어옴
        //    {
        //        BettingToken = false;
        //
        //      MetaTrendAPIHandler.INSTANCE.Betting_Zera(MasterBettingId);
        //      MetaTrendAPIHandler.INSTANCE.Betting_Zera(OtherBettingId);
        //    }
        //}
        //
        ////승자판정
        //if (GameManager.INSTANCE.ISDEAD && WinToken) // 침입한 플레이어가 죽었을 때
        //{
        //    WinToken = false;
        //    //master win
        //    //other의 zera를 master에게 준다.
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(MasterBettingId);
        //}
        //else if (GameManager.INSTANCE.ISTIMEOVER && WinToken) // 타임오버가 호출됐을때
        //{
        //    WinToken = false;
        //    //invader/otherclient win
        //    // master의 zera를 other에게 준다.
        //    MetaTrendAPIHandler.INSTANCE.Betting_Zera_DeclareWinner(OtherBettingId);
        //    // 받아온 id를 가진 유저에게 베팅했던 zera를 넘긴다.
        //    photonView.RPC("TransferOtherName", RpcTarget.All, Name);
        //}
        //
        //
        //// 게임이 비정상적으로 끝났을 때
        //if (!GameManager.INSTANCE.GameEndCorrect && DisConnectToken)
        //{
        //    DisConnectToken = false;
        //    // 베팅금액반환 - 베팅했던 Zera를 각자에게 되돌려줌                     
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