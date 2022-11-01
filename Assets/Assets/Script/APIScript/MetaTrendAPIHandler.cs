using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using TMPro;
using UnityEngine.Rendering;

public class MetaTrendAPIHandler : MonoSingleTon<MetaTrendAPIHandler>
{
    public string USERNAME { get; set; }
    public string SESSIONID { get; set; }
    public string WALLETADRESS { get; set; }
    public int ZERA { get; set; }
    public string BETTINGID { get; set; }




    [SerializeField] string selectedBettingID;

    [Header("[등록된 프로젝트에서 획득가능한 API 키]")]
    //https://odin-api-sat.browseosiris.com

    [Tooltip("이것은 http://odin-registration-sat.browseosiris.com/# 에 등록된 프로젝트를 통해서 획득할 수 있는 API Key 이다.\nhttps://odin-registration.browseosiris.com/ 는 Production URL")]
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
        Invoke("Settings", 1f);
        Invoke("ZeraBalance", 1f);
    }

    private void Start()
    {
        

    }

    private void Update()
    {
        if (PhotonManager.INSTANCE.ISMASTER)
        {
            if (GameManager.INSTANCE.ISGAMEIN)
            {
                //승자판정
                if (GameManager.INSTANCE.ISDEAD)
                {
                    //master win
                }
                else if (GameManager.INSTANCE.ISTIMEOVER)
                {
                    //invader/otherclient win
                }
            }
        }
    }

    // 현재 개발단계에 따라서 사용하는 BaseURL이 달라진다.
    string getBaseURL()
    {
        return FullAppsStagingURL;
    }

    Res_GetUserProfile resGetUserProfile = null;
    Res_GetSessionID resGetSessionID = null;
    Res_Settings resSettigns = null;





    //-----------------------------------------------------------------------------------------------------
    //
    // 유저 정보
    public void GetUserProfile()
    {
        StartCoroutine(processRequestGetUserInfo());
    }

    IEnumerator processRequestGetUserInfo()
    {
        // 유저 정보
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
        // 유저 정보
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
    // 베팅관련 셋팅 정보를 얻어오기
    public void Settings()
    {
        StartCoroutine(processRequestSettings());
    }
    IEnumerator processRequestSettings()
    {
        yield return requestSettings((response) =>
        {
            if (response != null)
            {
                resSettigns = response;
                BETTINGID = resSettigns.data.settings._id;
            }
        });
    }
    //-----------------------------------------------------------------------------------------------------

    //-----------------------------------------------------------------------------------------------------
    // Zera 잔고 확인
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

    // Ace 잔고 확인
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

    // Dappx 잔고 확인
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
    // ZERA 베팅
    public void Betting_Zera()
    {
        StartCoroutine(processRequestBetting_Zera());
    }
    IEnumerator processRequestBetting_Zera()
    {
        ResBettingPlaceBet resBettingPlaceBet = null;
        ReqBettingPlaceBet reqBettingPlaceBet = new ReqBettingPlaceBet();
        reqBettingPlaceBet.players_session_id = new string[] { resGetSessionID.sessionId };
        reqBettingPlaceBet.bet_id = selectedBettingID;// resSettigns.data.bets[0]._id;
        yield return requestCoinPlaceBet(reqBettingPlaceBet, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinPlaceBet : " + response.message);
                resBettingPlaceBet = response;
            }
        });
    }

    // ZERA 베팅-승자
    public void Betting_Zera_DeclareWinner()
    {
        StartCoroutine(processRequestBetting_Zera_DeclareWinner());
    }
    IEnumerator processRequestBetting_Zera_DeclareWinner()
    {
        ResBettingDeclareWinner resBettingDeclareWinner = null;
        ReqBettingDeclareWinner reqBettingDeclareWinner = new ReqBettingDeclareWinner();
        reqBettingDeclareWinner.betting_id = selectedBettingID;// resSettigns.data.bets[0]._id;
        reqBettingDeclareWinner.winner_player_id = resGetUserProfile.userProfile._id;
        yield return requestCoinDeclareWinner(reqBettingDeclareWinner, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinDeclareWinner : " + response.message);
                resBettingDeclareWinner = response;
            }
        });
    }

    // 베팅금액 반환
    public void Betting_Zera_Disconnect()
    {
        StartCoroutine(processRequestBetting_Zera_Disconnect());
    }
    IEnumerator processRequestBetting_Zera_Disconnect()
    {
        ResBettingDisconnect resBettingDisconnect = null;
        ReqBettingDisconnect reqBettingDisconnect = new ReqBettingDisconnect();
        reqBettingDisconnect.betting_id = selectedBettingID;// resSettigns.data.bets[1]._id;
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
    /// To get user’s information.This is also used to authenticate if session-id is valid or not.
    /// This can determine if the Odin is currently running or not. 
    ///	If Odin is not running, the API  is not accesible as well.
    ///	Inform the User to run the Osiris and Connect to Odin via Meta wallet.
    ///	
    /// 유저의 정보를 얻어 온다. 이것은 또한 Session ID 가 유효하지에 따라 인증에 사용된다.
    /// 이것은 Odin이 현재 실행 중인지에 따라서 결정된다.(Odin이 실행 중이어야 옳바른 데이터를 얻을 수 있다는 의미)
    /// Odin이 실행 중이지 않으면, API는 접근할 수 없다.
    /// Osiris 를 실행하기 위해서 유저를 알려주고, Meta wallet 을 통해서 odin 에 연결한다.
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
        }
    }

    /// <summary>
    /// 유저의 Session ID 를 요청한다.
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
    #region 재화(DAPPX, ZERA, ACE)의 잔고를 확인하는 API

    //
    // 재화(ZERA)의 잔고를 확인하는 API
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
    // 재화(ACE)의 잔고를 확인하는 API
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
    // 재화(DAPPX)의 잔고를 확인하는 API
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
    #endregion // 재화(DAPPX, ZERA, ACE)의 잔고를 확인하는 API
    //-----------------------------------------------------------------------------------------------------



    //-----------------------------------------------------------------------------------------------------
    //
    // To get game’s general and bet settings
    //
    delegate void resCallback_Settings(Res_Settings response);
    IEnumerator requestSettings(resCallback_Settings callback)
    {
        string url = getBaseURL() + "/v1/betting/settings";


        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Res_Settings res = JsonUtility.FromJson<Res_Settings>(www.downloadHandler.text);
        callback(res);
        //UnityWebRequest www = new UnityWebRequest(URL);
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
        ResBettingPlaceBet res = JsonUtility.FromJson<ResBettingPlaceBet>(www.downloadHandler.text);
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