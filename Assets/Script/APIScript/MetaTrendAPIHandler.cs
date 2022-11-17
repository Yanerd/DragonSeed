using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using TMPro;
using UnityEngine.Rendering;
using Newtonsoft.Json;

public class MetaTrendAPIHandler : MonoSingleTon<MetaTrendAPIHandler>
{
    public string USERNAME { get; set; }
    public string SESSIONID { get; set; }
    public string WALLETADRESS { get; set; }
    public int ZERA { get; set; }
    public string _ID { get; set; }
    public string BETTINGID { get; set; }

    [Header("[��ϵ� ������Ʈ���� ȹ�氡���� API Ű]")]
    //https://odin-api-sat.browseosiris.com

    [Tooltip("�̰��� http://odin-registration-sat.browseosiris.com/# �� ��ϵ� ������Ʈ�� ���ؼ� ȹ���� �� �ִ� API Key �̴�.\nhttps://odin-registration.browseosiris.com/ �� Production URL")]
    [SerializeField] string API_KEY = "";


    [Header("[Betting Backend Base URL")]
    [SerializeField] string FullAppsProductionURL = "https://odin-api.browseosiris.com";
    [SerializeField] string FullAppsStagingURL = "https://odin-api-sat.browseosiris.com";

    //


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        GetUserProfile();
        GetSessionID();
        Invoke("ZeraBalance", 1f);
        Invoke("Settings", 1f);
    }

    // ���� ���ߴܰ迡 ���� ����ϴ� BaseURL�� �޶�����.
    string getBaseURL()
    {
        return FullAppsStagingURL;
    }

    Res_GetUserProfile resGetUserProfile = null;
    Res_GetSessionID resGetSessionID = null;
    Res_Settings resSettigns = null;





    //-----------------------------------------------------------------------------------------------------
    //
    // ���� ����
    public void GetUserProfile()
    {
        StartCoroutine(processRequestGetUserInfo());
    }

    IEnumerator processRequestGetUserInfo()
    {
        // ���� ����
        yield return requestGetUserInfo((response) =>
        {
            if (response != null)
            {
                Debug.Log("## " + response.ToString());
                resGetUserProfile = response;
            }
        });
    }

    // Session ID
    public void GetSessionID()
    {
        StartCoroutine(processRequestGetSessionID());
    }

    IEnumerator processRequestGetSessionID()
    {
        // ���� ����
        yield return requestGetSessionID((response) =>
        {
            if (response != null)
            {
                Debug.Log("## " + response.ToString());
                resGetSessionID = response;
            }
        });
    }
    //-----------------------------------------------------------------------------------------------------
    // ���ð��� ���� ������ ������
    public void Settings()
    {
        StartCoroutine(processRequestSettings());
    }
    public IEnumerator processRequestSettings()
    {
        yield return requestSettings((response) =>
        {
            if (response != null)
            {
                resSettigns = response;
                
                if (resSettigns.data.bets[0] != null)
                    _ID = resSettigns.data.bets[0]._id;
            }
        });
    }
    //-----------------------------------------------------------------------------------------------------

    //-----------------------------------------------------------------------------------------------------
    // Zera �ܰ� Ȯ��
    public void ZeraBalance()
    {
        if (USERNAME != null)
        {
            StartCoroutine(processRequestZeraBalance());
        }
    }

    IEnumerator processRequestZeraBalance()
    {
        yield return requestZeraBalance(resGetSessionID.sessionId, (response) =>
        {
            if (response != null)
            {
                ZERA = response.data.balance;
            }
        });
    }

    // Ace �ܰ� Ȯ��
    public void AceBalance()
    {
        StartCoroutine(processRequestAceBalance());
    }

    IEnumerator processRequestAceBalance()
    {
        yield return requestAceBalance(resGetSessionID.sessionId, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## Response Ace Balance : " + response.data.balance);
            }

        });
    }

    // Dappx �ܰ� Ȯ��
    public void DappXBalance()
    {
        StartCoroutine(processRequestDappXBalance());
    }
    IEnumerator processRequestDappXBalance()
    {
        yield return requestDappXBalance(resGetSessionID.sessionId, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## Response DappX Balance : " + response.data.balance);
            }

        });
    }



    //-----------------------------------------------------------------------------------------------------
    //
    // ZERA ����
    public void Betting_Zera(string _Id, string[] sessionIdArray)
    {
        StartCoroutine(processRequestBetting_Zera(_Id, sessionIdArray));
    }
    public IEnumerator processRequestBetting_Zera(string _Id, string[] sessionIdArray)
    {
        ResBettingPlaceBet resBettingPlaceBet = null;
        ReqBettingPlaceBet reqBettingPlaceBet = new ReqBettingPlaceBet();
        reqBettingPlaceBet.players_session_id = sessionIdArray;//new string[] { resGetSessionID.sessionId };
        reqBettingPlaceBet.bet_id = _Id;//selectedBettingID;// resSettigns.data.bets[0]._id;
        yield return requestCoinPlaceBet(reqBettingPlaceBet, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinPlaceBet : " + response.message);
                resBettingPlaceBet = response;

                BETTINGID = response.data.betting_id;
            }
        });
    }

    // ZERA ����-����
    public void Betting_Zera_DeclareWinner(string bettingId, string winner_Id)
    {
        StartCoroutine(processRequestBetting_Zera_DeclareWinner(bettingId, winner_Id));
    }
    IEnumerator processRequestBetting_Zera_DeclareWinner(string bettingId, string winner_Id)
    {
        ResBettingDeclareWinner resBettingDeclareWinner = null;
        ReqBettingDeclareWinner reqBettingDeclareWinner = new ReqBettingDeclareWinner();
        reqBettingDeclareWinner.betting_id = bettingId;// resSettigns.data.bets[0]._id;
        reqBettingDeclareWinner.winner_player_id = winner_Id;//resGetUserProfile.userProfile._id;
        yield return requestCoinDeclareWinner(reqBettingDeclareWinner, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinDeclareWinner : " + response.message);
                resBettingDeclareWinner = response;
            }
        });
    }

    // ���ñݾ� ��ȯ
    public void Betting_Zera_Disconnect(string bettingid)
    {
        StartCoroutine(processRequestBetting_Zera_Disconnect(bettingid));
    }
    IEnumerator processRequestBetting_Zera_Disconnect(string bettingid)
    {
        ResBettingDisconnect resBettingDisconnect = null;
        ReqBettingDisconnect reqBettingDisconnect = new ReqBettingDisconnect();
        reqBettingDisconnect.betting_id = bettingid; //selectedBettingID;// resSettigns.data.bets[1]._id;
        yield return requestCoinDisconnect(reqBettingDisconnect, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinDisconnect : " + response.message);
                resBettingDisconnect = response;
            }
        });
    }
    //-----------------------------------------------------------------------------------------------------



    //-----------------------------------------------------------------------------------------------------
    #region LOCALHOST API
    /// <summary>
    /// To get user��s information.This is also used to authenticate if session-id is valid or not.
    /// This can determine if the Odin is currently running or not. 
    ///	If Odin is not running, the API  is not accesible as well.
    ///	Inform the User to run the Osiris and Connect to Odin via Meta wallet.
    ///	
    /// ������ ������ ��� �´�. �̰��� ���� Session ID �� ��ȿ������ ���� ������ ���ȴ�.
    /// �̰��� Odin�� ���� ���� �������� ���� �����ȴ�.(Odin�� ���� ���̾�� �ǹٸ� �����͸� ���� �� �ִٴ� �ǹ�)
    /// Odin�� ���� ������ ������, API�� ������ �� ����.
    /// Osiris �� �����ϱ� ���ؼ� ������ �˷��ְ�, Meta wallet �� ���ؼ� odin �� �����Ѵ�.
    /// </summary>
    //
    delegate void resCallback_GetUserInfo(Res_GetUserProfile response);
    IEnumerator requestGetUserInfo(resCallback_GetUserInfo callback)
    {
        // get user profile
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getuserprofile");
        yield return www.SendWebRequest();


        Res_GetUserProfile res_getUserProfile = JsonUtility.FromJson<Res_GetUserProfile>(www.downloadHandler.text);
        callback(res_getUserProfile);
        if (res_getUserProfile != null)
        {
            USERNAME = res_getUserProfile.userProfile.username;
            WALLETADRESS = res_getUserProfile.userProfile.public_address;
            SESSIONID = resGetSessionID.sessionId;
            Debug.Log("SESSIONID : " + SESSIONID);
        }
    }

    /// <summary>
    /// ������ Session ID �� ��û�Ѵ�.
    /// 
    /// </summary>
    /// <returns></returns>
    delegate void resCallback_GetSessionID(Res_GetSessionID response);
    IEnumerator requestGetSessionID(resCallback_GetSessionID callback)
    {
        // get session id
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getsessionid");
        yield return www.SendWebRequest();
        Res_GetSessionID res_getSessionID = JsonUtility.FromJson<Res_GetSessionID>(www.downloadHandler.text);
        callback(res_getSessionID);
    }
    #endregion // LOCALHOST API
    //-----------------------------------------------------------------------------------------------------


    //-----------------------------------------------------------------------------------------------------
    //
    #region ��ȭ(DAPPX, ZERA, ACE)�� �ܰ��� Ȯ���ϴ� API

    //
    // ��ȭ(ZERA)�� �ܰ��� Ȯ���ϴ� API
    // 
    delegate void resCallback_BalanceInfo(Res_BalanceInfo response);
    IEnumerator requestZeraBalance(string sessionID, resCallback_BalanceInfo callback)
    {
        string url = getBaseURL() + ("/v1/betting/" + "zera" + "/balance/" + sessionID);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        Res_BalanceInfo res = JsonUtility.FromJson<Res_BalanceInfo>(www.downloadHandler.text);
        callback(res);
        //UnityWebRequest www = new UnityWebRequest(URL);
    }

    //
    // ��ȭ(ACE)�� �ܰ��� Ȯ���ϴ� API
    // 

    IEnumerator requestAceBalance(string sessionID, resCallback_BalanceInfo callback)
    {
        string url = getBaseURL() + ("/v1/betting/" + "ace" + "/balance/" + sessionID);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        Res_BalanceInfo res = JsonUtility.FromJson<Res_BalanceInfo>(www.downloadHandler.text);
        callback(res);
        //UnityWebRequest www = new UnityWebRequest(URL);
    }



    //
    // ��ȭ(DAPPX)�� �ܰ��� Ȯ���ϴ� API
    // 

    IEnumerator requestDappXBalance(string sessionID, resCallback_BalanceInfo callback)
    {
        string url = getBaseURL() + ("/v1/betting/" + "dappx" + "/balance/" + sessionID);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        Res_BalanceInfo res = JsonUtility.FromJson<Res_BalanceInfo>(www.downloadHandler.text);
        callback(res);
        //UnityWebRequest www = new UnityWebRequest(URL);
    }
    #endregion // ��ȭ(DAPPX, ZERA, ACE)�� �ܰ��� Ȯ���ϴ� API
    //-----------------------------------------------------------------------------------------------------



    //-----------------------------------------------------------------------------------------------------
    //
    // To get game��s general and bet settings
    //
    delegate void resCallback_Settings(Res_Settings response);
    IEnumerator requestSettings(resCallback_Settings callback)
    {
        string url = getBaseURL() + "/v1/betting/settings";

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);

        Res_Settings res = JsonConvert.DeserializeObject<Res_Settings>(www.downloadHandler.text);
        callback(res);
    }


    //
    // Request Method : POST 
    // Body Type : json
    delegate void resCallback_BettingPlaceBet(ResBettingPlaceBet response);
    IEnumerator requestCoinPlaceBet(ReqBettingPlaceBet req, resCallback_BettingPlaceBet callback)
    {
        string url = getBaseURL() + "/v1/betting/" + "zera" + "/place-bet";

        string reqJsonData = JsonUtility.ToJson(req);
        Debug.Log(reqJsonData);


        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);
        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        ResBettingPlaceBet res = JsonConvert.DeserializeObject<ResBettingPlaceBet>(www.downloadHandler.text);
        callback(res);
    }

    //
    // Request Method : POST 
    // Body Type : json
    delegate void resCallback_BettingDeclareWinner(ResBettingDeclareWinner response);
    IEnumerator requestCoinDeclareWinner(ReqBettingDeclareWinner req, resCallback_BettingDeclareWinner callback)
    {
        string url = getBaseURL() + "/v1/betting/" + "zera" + "/declare-winner";

        string reqJsonData = JsonUtility.ToJson(req);
        Debug.Log(reqJsonData);


        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);
        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        ResBettingDeclareWinner res = JsonUtility.FromJson<ResBettingDeclareWinner>(www.downloadHandler.text);
        callback(res);
    }

    //
    // Request Method : POST 
    // Body Type : json
    delegate void resCallback_BettingDisconnect(ResBettingDisconnect response);
    IEnumerator requestCoinDisconnect(ReqBettingDisconnect req, resCallback_BettingDisconnect callback)
    {
        string url = getBaseURL() + "/v1/betting/" + "zera" + "/disconnect";

        string reqJsonData = JsonUtility.ToJson(req);
        Debug.Log(reqJsonData);


        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);
        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        ResBettingDisconnect res = JsonUtility.FromJson<ResBettingDisconnect>(www.downloadHandler.text);
        callback(res);
    }
    //-----------------------------------------------------------------------------------------------------



}