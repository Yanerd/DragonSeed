using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class OffenseUIManager : MonoSingleTon<OffenseUIManager>
{

    //camera components

    //Hp UI components
    PlayerController player = null;
    Canvas playerUI = null;
    Slider hpSlider = null;
    Slider hpFollowSlider = null;

    //Dragon Hp UI components
    GameObject dragonHpGroupObj = null;
    Slider dragonHpSlider = null;
    Slider dragonHpFollowSlider = null;
    public List<Dragon> fruitragons = null;

    //Cine Producton components
    RectTransform cineUp = null;
    RectTransform cineDown = null;

    //variables value
    public float ADDVALUE { get; set; }

    Coroutine curCoroutine = null;

    //constant value
    Vector3 initUp;
    Vector3 initDown;


    //OffenseEndUI
    GameObject offenseEndUI = null;

    // 게임매니저에 있는 변수 사용, 계산해서 텍스트로 띄우기
    int DragonKilledCountNum = 0;
    int PlantKilledCountNum = 0;
    int BuildingDestroyCountNum = 0;

    // 파괴한 오브젝트 텍스트로 받아오기
    TextMeshProUGUI DragonKilledCount;
    TextMeshProUGUI PlantKilledCount;
    TextMeshProUGUI BuildingDestroyCount;
    // 총 점수 계산
    TextMeshProUGUI TotalScore;
    TextMeshProUGUI GetCoin;
    // 랭크
    TextMeshProUGUI RANK;

    //exception coroutine
    Coroutine exceptionCoroutine = null;

    private new void OnEnable()
    {
        //cursor lock
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    //exception coroutine
    IEnumerator FindPlayerUI()
    {
        while(playerUI == null)
        {
            playerUI = GameObject.Find("PlayerUI").GetComponent<Canvas>();
            hpSlider = GameObject.Find("HpSlider").GetComponent<Slider>();
            hpFollowSlider = GameObject.Find("HpFollowSlider").GetComponent<Slider>();
            if (playerUI != null)
            {
                if(exceptionCoroutine != null)
                    StopCoroutine(exceptionCoroutine);

                yield break;
            }
            yield return null;
        }

    }

    private void Awake()
    {
        //hp ui components
        player = FindObjectOfType<PlayerController>();


        if (GameManager.INSTANCE.WANTINVASION)
        {
            //exceptionCoroutine = StartCoroutine(FindPlayerUI());
        }

        //dragon hp ui components
        fruitragons = new List<Dragon>();
        dragonHpGroupObj = GameObject.Find("DragonHp");
        dragonHpSlider = GameObject.Find("DragonHpSlider").GetComponent<Slider>();
        dragonHpFollowSlider = GameObject.Find("DragonHpFollowSlider").GetComponent<Slider>();

        //cine production components
        cineUp = GameObject.Find("CineViewUp").GetComponent<RectTransform>();
        cineDown = GameObject.Find("CineViewDown").GetComponent<RectTransform>();

        // Offense End UI Panel Found in Canvas
        offenseEndUI = GameObject.Find("OffenseEndUI");

        DragonKilledCount = GameObject.Find("DragonsKilled_number").GetComponent<TextMeshProUGUI>();
        PlantKilledCount = GameObject.Find("PlantKilled_number").GetComponent<TextMeshProUGUI>();
        BuildingDestroyCount = GameObject.Find("DestroyBuilding_number").GetComponent<TextMeshProUGUI>();
        TotalScore = GameObject.Find("TotalScore_number").GetComponent<TextMeshProUGUI>();
        GetCoin = GameObject.Find("GetCoins_number").GetComponent<TextMeshProUGUI>();
        RANK = GameObject.Find("Rank").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.INSTANCE.TimerStart();
        // Offense End UI
        offenseEndUI.SetActive(false);

        //player hp delegate
        if (player != null)
            //player.playerEvent.callBackPlayerHPChangeEvent += OnChangedHp;

        Debug.Log("드래곤 찾아");
        Debug.Log("드래곤 찾아");
        Debug.Log("드래곤 찾아");
        Debug.Log("드래곤 찾아");

        Invoke("FindDragon", 2f);

        //dragon hp view init
        dragonHpGroupObj.SetActive(false);

        GameManager.INSTANCE.ISLOCKON = false;
        ADDVALUE = 200f;

        initUp = cineUp.position;
        initDown = cineDown.position;

    }
    public void FindDragon()
    {
        for (int i = 0; i < GameManager.INSTANCE.dragons.Count; i++)
        {
            GameManager.INSTANCE.dragons[i].GetComponent<Dragon>().
                    dragonEvent.callBackDragonHPChangeEvent += OnChangeDragonHP;
        }

    }
    public new void OnDisable()
    {
        for (int i = 0; i < GameManager.INSTANCE.dragons.Count; i++)
        {
            GameManager.INSTANCE.dragons[i].GetComponent<Dragon>().
                    dragonEvent.callBackDragonHPChangeEvent -= OnChangeDragonHP;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        CineProductControll();
        GameEndControll();
    }

    


    private void CineProductControll()
    {
        if (GameManager.INSTANCE.ISDEAD || GameManager.INSTANCE.ISTIMEOVER)
        {
            curCoroutine = StartCoroutine(ProductionOff());
        }


        if (GameManager.INSTANCE.ISLOCKON)
        {
            curCoroutine = StartCoroutine(ProductionOn());
        }
        else if (!GameManager.INSTANCE.ISLOCKON)
        {
            curCoroutine = StartCoroutine(ProductionOff());
        }
    }
    IEnumerator ProductionOn()
    {
        float timer = 0f;

        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }

        while (true)
        {
            timer += Time.deltaTime;

            cineDown.position = Vector3.Lerp(cineDown.position, initDown + Vector3.up * ADDVALUE, 0.05f);
            cineUp.position = Vector3.Lerp(cineUp.position, initUp + Vector3.down * ADDVALUE, 0.05f);

            yield return null;
            if (timer > 0.8f) yield break;
        }
    }
    IEnumerator ProductionOff()
    {
        float timer = 0f;

        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }

        while (true)
        {
            timer += Time.deltaTime;

            cineDown.position = Vector3.Lerp(cineDown.position, initDown, 0.05f);
            cineUp.position = Vector3.Lerp(cineUp.position, initUp, 0.05f);

            yield return null;
            if (timer > 0.8f) yield break;
        }
    }

    //oversee function
    private void OnChangeDragonHP(float curHp, float maxHp)
    {
        Debug.Log("체력바 나와");
        dragonHpGroupObj.SetActive(true);
        dragonHpSlider.value = curHp / maxHp;
        StartCoroutine(DragonFollowSlider());
    }

    IEnumerator DragonFollowSlider()
    {
        while (true)
        {
            dragonHpFollowSlider.value = Mathf.Lerp(dragonHpFollowSlider.value, dragonHpSlider.value, Time.deltaTime / 2f);

            if (dragonHpFollowSlider.value == dragonHpSlider.value)
            {
                yield break;
            }

            yield return null;
        }
    }

    private void OnChangedHp(float curHp, float maxHp)
    {
        hpSlider.value = curHp / maxHp;
        StartCoroutine(FollowSlider());
    }

    IEnumerator FollowSlider()
    {
        while (true)
        {
            hpFollowSlider.value = Mathf.Lerp(hpFollowSlider.value, hpSlider.value, Time.deltaTime / 5f);

            if (hpFollowSlider.value == hpSlider.value) yield break;

            yield return null;
        }
    }












    private void GameEndControll()
    {

        if (GameManager.INSTANCE.ISDEAD|| GameManager.INSTANCE.ISTIMEOVER)
        {
            // 마우스 커서 활성화
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //calculation
            GameManager.INSTANCE.CoinRavish();

            // 게임 오버 창 띄우기
            offenseEndUI.SetActive(true);


            // 드래곤 잡은 수 체크 -> 드래곤이 죽을때마다 count 로 체크
            DragonKilledCount.text = "" + GameManager.INSTANCE.KILLCOUNT;//"KilledDragons_number" + 
            // 식물 잡은 수 체크 -> 식물이 죽을때마다 체크
            PlantKilledCount.text = "" + GameManager.INSTANCE.DESTROYPLANTCOUNT;//"StealSeeds_number" + 
            // 집 -> 없어질때마다 체크
            BuildingDestroyCount.text = "" + GameManager.INSTANCE.DESTROYBUILDINGCOUNT;//"DestroyBuilding_number" +

            TotalScore.text = "" + GameManager.INSTANCE.TOTALCOIN;
            GetCoin.text = "" + GameManager.INSTANCE.TOTALCOIN;



            // 나중에 착착 한 줄씩 점수 뜨는 효과 넣기

        }


    }

    Coroutine instiateCoroutine;

    // 버튼 누를때는 마우스 커서 활성화 필요
    public void OnBackButton()
    {
        Time.timeScale = 1f;

        GameManager.INSTANCE.SCENENUM = 1;
        SendGameEnd();

        SceneManager.LoadScene("2_GardenningScene");

        PhotonNetwork.Disconnect();

        GameManager.INSTANCE.Initializing();

        instiateCoroutine = StartCoroutine(GoBackSceneInstantiate());
    }

    [PunRPC]
    public void SendGameEnd()
    {
        photonView.RPC("GetGameEnd", RpcTarget.All, true);
    }
    public void GetGameEnd(bool flag)
    {
        GameManager.INSTANCE.GameEndCorrect = flag;
    }


    IEnumerator GoBackSceneInstantiate()
    {
        Debug.Log("씬 전환중");

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "2_GardenningScene");
        StopCoroutine(instiateCoroutine);
        instiateCoroutine = null;
        yield break;
    }



}


