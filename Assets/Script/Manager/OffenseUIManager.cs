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

    // ���ӸŴ����� �ִ� ���� ���, ����ؼ� �ؽ�Ʈ�� ����
    int DragonKilledCountNum = 0;
    int PlantKilledCountNum = 0;
    int BuildingDestroyCountNum = 0;

    // �ı��� ������Ʈ �ؽ�Ʈ�� �޾ƿ���
    TextMeshProUGUI DragonKilledCount;
    TextMeshProUGUI GetZeraReward;
    TextMeshProUGUI BuildingDestroyCount;
    // �� ���� ���
    TextMeshProUGUI GetCoin;
    // ���
    TextMeshProUGUI Result;

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
        GetZeraReward = GameObject.Find("GetZera_number").GetComponent<TextMeshProUGUI>();
        BuildingDestroyCount = GameObject.Find("DestroyBuilding_number").GetComponent<TextMeshProUGUI>();
        GetCoin = GameObject.Find("GetCoins_number").GetComponent<TextMeshProUGUI>();
        Result = GameObject.Find("Result").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.INSTANCE.TimerStart();
        // Offense End UI
        offenseEndUI.SetActive(false);

        //player hp delegate
        if (player != null)
            //player.playerEvent.callBackPlayerHPChangeEvent += OnChangedHp;

        Debug.Log("�巡�� ã��");
        Debug.Log("�巡�� ã��");
        Debug.Log("�巡�� ã��");
        Debug.Log("�巡�� ã��");

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
        Debug.Log("ü�¹� ����");
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
            // ���콺 Ŀ�� Ȱ��ȭ
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //calculation
            GameManager.INSTANCE.CoinRavish();

            // ���� ���� â ����
            offenseEndUI.SetActive(true);


            // �巡�� ���� �� üũ -> �巡���� ���������� count �� üũ
            DragonKilledCount.text = "" + GameManager.INSTANCE.KILLCOUNT;//"KilledDragons_number" + 
            // �� -> ������������ üũ
            BuildingDestroyCount.text = "" + GameManager.INSTANCE.HOUSEDESTROYCOUNT;//"DestroyBuilding_number" +
            GetZeraReward.text = "" + GameManager.INSTANCE.ZERAREWARD;
            GetCoin.text = "" + GameManager.INSTANCE.STEALCOIN;

            if(GameManager.INSTANCE.ISDEAD)
            {
                Result.text = "Lose";
            }
            else
            {
                Result.text = "Win";
            }

           

        }


    }

    Coroutine instiateCoroutine;

    // ��ư �������� ���콺 Ŀ�� Ȱ��ȭ �ʿ�
    public void OnBackButton()
    {
        Time.timeScale = 1f;

        GameManager.INSTANCE.SCENENUM = 1;
        UserInfo.INSTANCE.SettingClear();

        SendGameEnd();

        MetaTrendAPIHandler.INSTANCE.GetAllData();

        PhotonNetwork.Disconnect();

        SceneManager.LoadScene("2_GardenningScene");

        GameManager.INSTANCE.Initializing();

        instiateCoroutine = StartCoroutine(GoBackSceneInstantiate());
    }

    
    public void SendGameEnd()
    {
        photonView.RPC("GetGameEnd", RpcTarget.All, true);
    }
    [PunRPC]
    public void GetGameEnd(bool flag)
    {
        GameManager.INSTANCE.GameEndCorrect = flag;
    }


    IEnumerator GoBackSceneInstantiate()
    {
        Debug.Log("�� ��ȯ��");

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "2_GardenningScene");
        StopCoroutine(instiateCoroutine);
        instiateCoroutine = null;
        yield break;
    }



}


