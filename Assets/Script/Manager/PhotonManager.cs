using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;


public class PhotonManager : MonoSingleTon<PhotonManager>
{
    public bool INABLE { get; set; }

    UserInfo userInfo = null;

    [Header("TestNameField")]
    [SerializeField] TMP_InputField testNameInput = null;

    [Header("UICanvas")]
    [SerializeField] GameObject photonUI;

    [Header("Buttons")]
    [SerializeField] Button invasionPermitButton;
    [SerializeField] Button searchRoomButton;

    [Header("[ConnectViewPage]")]
    [SerializeField] GameObject connectViewPage = null;
    [SerializeField] TextMeshProUGUI connectInfo = null;

    [Header("RoomListPage")]
    [SerializeField] GameObject searchRoomPage = null;
    [SerializeField] GameObject roomListSpot = null;

    [Header("RoomPrefab")]
    [SerializeField] GameObject roomPrefab = null;
    [SerializeField] GameObject emptyRoomPrefab = null;

    List<RoomInfo> testRoomList = null;

    public void OnIPButton()
    {
        invasionPermitButton.interactable = true;
    }

    public void OnSRButton()
    {
        searchRoomButton.interactable = true;
    }

    //property
    public bool ISMASTER { get; set; }
    
    //room list
    List<GameObject> roomObjList = new List<GameObject>();
    GameObject emptyRoomObj = null;

    Vector3 originCamearPos;

    bool onMenu = false;

    public string testName { get; set; }

    Coroutine connectingCoroutine;
    Coroutine instiateCoroutine;

    private void Awake()
    {
        INABLE = false;

        DontDestroyOnLoad(this.gameObject);

        //Bring Camera Component
        originCamearPos = Camera.main.transform.position;

        //screen setting
        Screen.SetResolution(1920, 1080, true);

        //button interaction false
        invasionPermitButton.interactable = false;
        searchRoomButton.interactable = false;

    }
    private void Start()
    {
        GameManager.INSTANCE.INVASIONALLOW = (GameManager.INSTANCE.INVASIONALLOW == false);
    }
    private void Update()//view real time connect
    {
        ISMASTER = PhotonNetwork.IsMasterClient;

        //meta trand connetion enable
        testName = MetaTrendAPIHandler.INSTANCE.USERNAME;
        if (testName == null)
        {
            INABLE = false;
        }
        else
        {
            INABLE = true;
        }

        connectInfo.text = PhotonNetwork.NetworkClientState.ToString();

        //this parts detecting clients alived athor room successfully ->this clients be a invader
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2 && GameManager.INSTANCE.ISGAMEIN == false)
        {
            if (PhotonNetwork.MasterClient.NickName != testName)
            {
                //Offense scene move -> invasion view
                GameManager.INSTANCE.ISGAMEIN = true;
                GameManager.INSTANCE.SCENENUM = 2;

                PhotonNetwork.LoadLevel("3_OffenceScene");

                Debug.Log("나는 공격");

                // photonView.RPC("SendMyData", );

                instiateCoroutine = StartCoroutine(playerInstantiate()); 
            }
        }

        //photon UI check
        if (GameManager.INSTANCE.SCENENUM == 1 && DefenseUIManager.INSTANCE.BUILDINGMODE == false)
        {
            photonUI.SetActive(true);
        }
        else
        {
            photonUI.SetActive(false);
        }


        if(roomObjList != null)
            Debug.Log($"방 개수 : {roomObjList.Count}");
    }

    public void SendMyData()
    { 
    }

    public void OnInputChanged()//input feild name test
    {
        Debug.Log(testNameInput.text.Length);
        if (testNameInput.text.Length < 10 && testNameInput.text.Length > 2)
        {
            testName = testNameInput.text;
            invasionPermitButton.interactable = true;
            searchRoomButton.interactable = true;

            Debug.Log("Api 확인 (Id 확인됨)");
            Debug.Log("===============================================================");

        }
        else
        {
            Debug.Log("(외부 API 연결이 확인되지 않습니다. (Id 길이가 틀립니다))");
            Debug.Log("InGame : 북풍이 불어오지 않습니다.");
            Debug.Log("===============================================================");
        }
    }
    public void OnInvasionPermitButton()//invasion allow, make a room and ready(takes benefit)
    {
        //invasion allow toggle
        GameManager.INSTANCE.INVASIONALLOW = (GameManager.INSTANCE.INVASIONALLOW == false);

        if (GameManager.INSTANCE.INVASIONALLOW)
        {
            //button interactable
            invasionPermitButton.interactable = false;
            searchRoomButton.interactable = false;

            //make room and ready
            connectingCoroutine = StartCoroutine(RoomMakingProcess());
        }
        else
        {
            //button interactable
            searchRoomButton.interactable = true;

            //network disconnect
            PhotonNetwork.Disconnect();
        }
    }
    IEnumerator RoomMakingProcess()
    {
        while (true)
        {
            if (PhotonNetwork.IsConnected == false)//1. try master server connect
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                if (PhotonNetwork.InLobby == false)//2. try to join lobby
                {
                    PhotonNetwork.JoinLobby();
                }
                else
                {
                    if (PhotonNetwork.InRoom == false)//3. create room
                    {
                        PhotonNetwork.CreateRoom(testName, new RoomOptions { MaxPlayers = 2 }, null);
                    }
                    else 
                    {
                        yield break;
                    }

                }
            }
            yield return null;
        }
    }
    public void OnGoOffenseButton()//go to search room for invasion 
    {
        //roomlist clear
        if (roomObjList != null)
        {
            for (int j = 0; j < roomObjList.Count; j++)
            {
                Destroy(roomObjList[j]);
            }
            roomObjList.Clear();
        }

        GameManager.INSTANCE.WANTINVASION = (GameManager.INSTANCE.WANTINVASION == false);

        if (GameManager.INSTANCE.WANTINVASION)
        {
            //button interactable controll
            invasionPermitButton.interactable = false;
            searchRoomButton.interactable = false;

            //production
            StartCoroutine(ClosUpCamera());
            StartCoroutine(Uptrans(searchRoomPage));

            //function
            connectingCoroutine = StartCoroutine(SearchRoomProcess());
        }
        else
        {
            //button interactable controll
            invasionPermitButton.interactable = true;

            //production
            StartCoroutine(FadeOutCamera());
            StartCoroutine(Downtrans(searchRoomPage));

            //function
            PhotonNetwork.Disconnect();
        }
    }
    IEnumerator SearchRoomProcess()
    {
        Debug.Log("입장중");
        Debug.Log(PhotonNetwork.CountOfRooms);
        Debug.Log(PhotonNetwork.CountOfPlayersInRooms);
        while (true)
        {
            if (PhotonNetwork.IsConnected == false)//1. try master server connect
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                if (PhotonNetwork.InLobby == false)//2. try to join lobby
                {
                    PhotonNetwork.JoinLobby();
                }
                else
                {
                    yield break;
                }
            }
            yield return null;
        }
    }

    #region network call back override
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.LocalPlayer.NickName = testName;

        Debug.Log("UserName = " + PhotonNetwork.NickName);
        Debug.Log("InGame : 북풍이 불어옵니다...!");
        Debug.Log("===============================================================");
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("Join Lobby");
        Debug.Log("InGame : 북풍이 거세게 불어옵니다..!");
        Debug.Log("===============================================================");
        
        if (GameManager.INSTANCE.WANTINVASION)
        {
            searchRoomButton.interactable = true;
        }
    }
    public override void OnCreatedRoom()
    {
        //base function
        base.OnCreatedRoom();

        //debug
        Debug.Log("Room Created Success");
        Debug.Log("InGame : 손님을 맞이할 준비가 끝났습니다..!");
        Debug.Log("===============================================================");

        //function
        StopCoroutine(connectingCoroutine);
    }
    public override void OnJoinedRoom()
    {
        //base function
        base.OnJoinedRoom();

        //debug
        Debug.Log("Joined Room Success");
        Debug.Log("InGame : 손님을 기다립니다..!");
        Debug.Log("===============================================================");

        //function
        invasionPermitButton.interactable = true;
    }
    public override void OnDisconnected(DisconnectCause cause)//master server connecting failed
    {
        //base function
        base.OnDisconnected(cause);

        //debug
        Debug.Log("Server Disconnected");
        Debug.Log("InGame : 북풍이 잦아듭니다...");
        Debug.Log("===============================================================");

        //function
        StopCoroutine(connectingCoroutine);

        //button interactable controll
        invasionPermitButton.interactable = true;
        searchRoomButton.interactable = true;
    }
    public override void OnCreateRoomFailed(short returnCode, string message)//create room failed
    {
        base.OnCreateRoomFailed(returnCode, message);

        Debug.Log("Room Created Failed");
        Debug.Log("InGame : 대문이 대차게 닫힙니다...");
        Debug.Log("===============================================================");

        StopCoroutine(connectingCoroutine);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)//joinning room failded
    {
        base.OnJoinRoomFailed(returnCode, message);

        Debug.Log("Joining Room Failed");
        Debug.Log("InGame : 밥상을 걷어 찹니다...");
        Debug.Log("===============================================================");

        StopCoroutine(connectingCoroutine);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)//update room obj info changed
    {
        Debug.Log("방 업데이트");

        base.OnRoomListUpdate(roomList);

        if (roomList.Count != 0 && roomObjList.Count != 0)//someting changed
        {
            for (int i = 0; i < roomList.Count; i++)//getting changed room list
            {
                for (int j = 0; j < roomObjList.Count; j++)//compare Obj list - changed room list
                {
                    if (roomObjList[j].GetComponent<RoomController>().ID == roomList[i].Name)
                    {
                        Destroy(roomObjList[j]);
                        roomObjList.RemoveAt(j);
                    }
                }
            }
        }

        if (GameManager.INSTANCE.WANTINVASION)
        {
            if(connectingCoroutine != null)
            {
                StopCoroutine(connectingCoroutine);
            }

            if (roomList.Count != 0)//changed room list info write
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].PlayerCount> 0)
                    {
                        GameObject instRoom = Instantiate(roomPrefab, roomListSpot.transform);
                        instRoom.GetComponent<RoomController>().RoomNameSetting(roomList[i].Name);
                        instRoom.GetComponent<RoomController>().RoomInfoSetting(roomList[i].PlayerCount);
                        roomObjList.Add(instRoom);
                    }
                }
            }
        }

        if (roomObjList.Count == 0)
        {
            if (emptyRoomObj == null)
            {
                emptyRoomObj = Instantiate(emptyRoomPrefab, roomListSpot.transform);
            }
            else
            {
                emptyRoomObj.SetActive(true);
            }
        }
        else 
        {
            if (emptyRoomObj != null)
            {
                emptyRoomObj.SetActive(false);
            }
        }
        
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)//this events called when New Player Detectied -> this clients have to be a defense game()  
    {
        base.OnPlayerEnteredRoom(newPlayer);

        //Offense Scene Move -> defense view
        if (GameManager.INSTANCE.ISGAMEIN == false)
        {
            GameManager.INSTANCE.ISGAMEIN = true;
            GameManager.INSTANCE.SCENENUM = 2;


            SaveLoadManager.INSTANCE.Save();

            PhotonNetwork.LoadLevel("3_OffenceScene");

            Debug.Log("나는 방어");

            instiateCoroutine = StartCoroutine(dragonInstantiate());

        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)//this events called when New Player out -> go back to defense scene
    {
        base.OnPlayerLeftRoom(otherPlayer);

        //scene changed
        Debug.Log("나간거 확인!");
        Debug.Log("나간거 확인!");
        Debug.Log("나간거 확인!");
        Debug.Log("나간거 확인!");
        PhotonNetwork.Disconnect();

        GameManager.INSTANCE.SCENENUM = 1;
        PhotonNetwork.LoadLevel("2_GardenningScene");

        GameManager.INSTANCE.Initializing();

        GameManager.INSTANCE.TimerClear();

        instiateCoroutine = StartCoroutine(GoBackSceneInstantiate());

    }
    #endregion

    #region Page Up&Down Coroutine
    IEnumerator Uptrans(GameObject page)
    {
        while (true)
        {
            page.transform.position =
                Vector3.Lerp(page.transform.position, page.transform.position + new Vector3(0, 100f, 0), Time.deltaTime * 20f);
            yield return null;

            if (page.transform.position.y >= 540f)
            {
                yield break;
            }
        }
    }
    IEnumerator Downtrans(GameObject page)
    {
        while (true)
        {
            page.transform.position =
                Vector3.Lerp(page.transform.position, page.transform.position + new Vector3(0, -100f, 0), Time.deltaTime * 30f);
            yield return null;

            if (page.transform.position.y <= -540f)
            {
                yield break;
            }
        }
    }

    public void EndGame()
    {
        StartCoroutine(Downtrans(searchRoomPage));
    }

    #endregion

    #region Change CameraView Coroutine
    IEnumerator ClosUpCamera()
    {
        while (true)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(-2.5f, 2.9f, -4.1f), Time.deltaTime * 2f);

            if (Camera.main.orthographicSize >= 0.3f)
            {
                Camera.main.orthographicSize -= 0.04f;
            }

            if (Camera.main.transform.position.z <= -4.08f)
            {
                searchRoomButton.interactable = true;
                yield break;
            }

            yield return null;
        }
    }
    IEnumerator FadeOutCamera()
    {
        while (true)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(-2.72f, 4.25f, -2.72f), Time.deltaTime * 2f);

            if (Camera.main.orthographicSize <= 2.5f)
            {
                Camera.main.orthographicSize += 0.04f;
            }

            if (Camera.main.transform.position.z >= -2.74f)
            {
                searchRoomButton.interactable = true;
                yield break;
            }
            yield return null;
        }
    }

    #endregion
    IEnumerator GoBackSceneInstantiate()
    {

        Debug.Log("씬 전환중");
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "2_GardenningScene");
        StopCoroutine(instiateCoroutine);
        instiateCoroutine = null;
        yield break;
    }
    IEnumerator playerInstantiate()
    {
        Debug.Log("씬 전환중");

        yield return new WaitUntil(() => (SceneManager.GetActiveScene().name == "3_OffenceScene"));
        
        yield return StartCoroutine(CreateCam("O_CameraArm"));

        yield return StartCoroutine(CreateOffenseUI());
        Debug.Log("플레이어 생성");
        PhotonNetwork.Instantiate("O_Farmer", new Vector3(0f, 1f, 0f), Quaternion.identity);
    }

    IEnumerator dragonInstantiate()
    {
        Debug.Log("씬 전환중");
        
        yield return new WaitUntil(() => (SceneManager.GetActiveScene().name == "3_OffenceScene"));


        yield return StartCoroutine(CreateCam("O_DEFMainCamera"));

        yield return StartCoroutine(CreateDefenseUI());

        Debug.Log("드래곤 생성");
        SaveLoadManager.INSTANCE.Load();
    }

    IEnumerator CreateCam(string camName)
    {
        Debug.Log("카메라 생성");

        GameObject CameraArm;
        
        CameraArm = Resources.Load<GameObject>(camName);
        Instantiate(CameraArm, CameraArm.transform.position, Quaternion.identity);

        Debug.Log("캠 생성 완료");
        yield break;
    }
    IEnumerator CreateOffenseUI()
    {
        Debug.Log("오팬스 ui 생성");

        GameObject OffenseUIManager;
        GameObject instOffenseUIManager;

        OffenseUIManager = Resources.Load<GameObject>("O_OffenseUIManager");
        instOffenseUIManager = Instantiate(OffenseUIManager, OffenseUIManager.transform.position, Quaternion.identity);

        Debug.Log("UI 생성 완료");
        yield break;
    }
    IEnumerator CreateDefenseUI()
    {
        Debug.Log("디팬스 ui 생성");

        GameObject DefenseBattleUIManager;
        GameObject instDefenseBattleUIManager;

        DefenseBattleUIManager = Resources.Load<GameObject>("O_DefenseBattleUIManager");
        instDefenseBattleUIManager = Instantiate(DefenseBattleUIManager, DefenseBattleUIManager.transform.position, Quaternion.identity);

        yield break;
    }













}