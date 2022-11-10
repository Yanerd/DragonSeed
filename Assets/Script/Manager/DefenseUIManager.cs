using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DefenseUIManager : MonoSingleTon<DefenseUIManager>
{
    Canvas defenseUI;

    [Header("Be A KING")]
    [SerializeField] bool masterMode = false;
    public bool invadePermit;

    #region Click Object Value
    [SerializeField] public bool BUILDINGMODE { get; set; } 
    [SerializeField] public bool onPOTATO     { get; set; }
    [SerializeField] public bool onAPPLE      { get; set; }
    [SerializeField] public bool onCABBAGE    { get; set; }
    [SerializeField] public bool onCARROT     { get; set; }
    [SerializeField] public bool onEEGPLANT   { get; set; }
    [SerializeField] public bool onWATER      { get; set; }
    [SerializeField] public bool onHOUSE      { get; set; }
    [SerializeField] public bool onWELL       { get; set; }
    [SerializeField] public bool WATERRAY     { get; set; }
    #endregion

    #region All Ui

    void UiInitializing()
    {
        //액티브 펄스 된 상태면 find로 못 찾아서 찾은 다음 펄스시킴

        GardeningMenu = GameObject.Find("GardeningMenu");

        BulidingModeMenu = GameObject.Find("BulidingModeMenu");

        VegetableScroll = GameObject.Find("VegetableScroll");
        VegetableScrollOpenButton = GameObject.Find("VegetableScrollOpenButton");
        VegetableScrollCloseButton = GameObject.Find("VegetableScrollCloseButton");
        VegetableScrollCloseButton.SetActive(false);

        BuildingScroll = GameObject.Find("BuildingScroll");
        BuildingScrollOpenButton = GameObject.Find("BuildingScrollOpenButton");
        BuildingScrollCloseButton = GameObject.Find("BuildingScrollCloseButton");

        BuildingScrollCloseButton.SetActive(false);

        for (int i = 0; i < GameObject.Find("VegetableButton").transform.childCount; i++)
        {
            InstButton[i] = GameObject.Find("VegetableButton").transform.GetChild(i).gameObject;
        }
        InstButton[5] = GameObject.Find("HouseButton");
        InstButton[6] = GameObject.Find("WellButton");

        BulidingModeMenu.SetActive(false);

        //StorePage/////////////////////////////////////////////////
        StorePage = GameObject.Find("ShopPage");

        StoreVegetablePage = GameObject.Find("StoreVegetablePage");
        VegetableScrollView = StoreVegetablePage.transform.GetChild(1);

        StoreBuildingPage = GameObject.Find("StoreBuildingPage");
        BuildingScrollView = StoreBuildingPage.transform.GetChild(1);

        StoreGroundPage = GameObject.Find("StoreGroundPage");
        GroundScrollView = StoreGroundPage.transform.GetChild(1);
        GroundScrollView.gameObject.SetActive(false);

        
    }

    GameObject GardeningMenu;
    GameObject BulidingModeMenu;
    GameObject StorePage;

    #region StorPage Func & Sell & purchase
    /////////////////////////////////////////////////
    GameObject StoreVegetablePage;
    Transform VegetableScrollView;

    GameObject StoreBuildingPage;
    Transform BuildingScrollView;

    GameObject StoreGroundPage;
    Transform GroundScrollView;

    [SerializeField] TextMeshProUGUI PotatoCount;
    [SerializeField] TextMeshProUGUI AppleCount;
    [SerializeField] TextMeshProUGUI CabbageCount;
    [SerializeField] TextMeshProUGUI CarrotCount;
    [SerializeField] TextMeshProUGUI EggplantCount;

    [SerializeField] TextMeshProUGUI curHouseCount;
    [SerializeField] TextMeshProUGUI curWellCount;
    [SerializeField] Button GroundBuyButton;
    [SerializeField] GameObject curGroundState;

    [SerializeField] TextMeshProUGUI PotatoSeedCount;
    [SerializeField] TextMeshProUGUI AppleSeedCount;
    [SerializeField] TextMeshProUGUI CabbageSeedCount;
    [SerializeField] TextMeshProUGUI CarrotSeedCount;
    [SerializeField] TextMeshProUGUI EggplantSeedCount;

    Transform[] fence = new Transform[10];
    Transform[] tree  = new Transform[20];
    [SerializeField] public int MapState;
    /////////////////////////////////////////////////
    #endregion

    #region BuildingMode Scroll & Button

    GameObject VegetableScroll;
    GameObject VegetableScrollOpenButton;
    GameObject VegetableScrollCloseButton;

    GameObject BuildingScroll;
    GameObject BuildingScrollOpenButton;
    GameObject BuildingScrollCloseButton;

    GameObject[] InstButton=new GameObject[7];

    #endregion

    #region Object Amount
    /////////////////////////////////////////////////
    [Header("[GameMoney(Zera)]")]
    [SerializeField] TextMeshProUGUI GoldText;
    [SerializeField] TextMeshProUGUI ZeraText;

    [Header("Cur DragonCount")]
    [SerializeField] TextMeshProUGUI curPotato;
    [SerializeField] TextMeshProUGUI curApple;
    [SerializeField] TextMeshProUGUI curCabbage;
    [SerializeField] TextMeshProUGUI curCarrot;
    [SerializeField] TextMeshProUGUI curEggplant;
    [SerializeField] TextMeshProUGUI curHouse;
    [SerializeField] TextMeshProUGUI curWell;
    /////////////////////////////////////////////////
    #endregion

    #region Slider

    public List<Slider> SliderBarList = new List<Slider>();

    #endregion

    //클릭 방지
    bool onMenu;

    #endregion

    #region Dragon Data

    

    [SerializeField] DragonData[] dragonData = new DragonData[5];
    [SerializeField] VegetableData[] vegetableData = new VegetableData[5];

    #endregion


    //test
    private GameObject testCount = null;

    //GameManager value
    /////////////////////////////////////////////////////
    public int Gold;
    int groundPrice = 1000;
    int housePrice = 500;
    int wellPrice = 500;

    public int potatoSeedCount = 0;
    public int appleSeedCount = 0;
    public int cabbageSeedCount = 0;
    public int carrotSeedCount = 0;
    public int eggplantSeedCount = 0;
    public int houseCount = 0;
    public int wellCount = 0;
    /////////////////////////////////////////////////////

    CursorChange myCursor;
    Vector3 VegetableMenuOriginPos;
    Vector3 BuildingMenuOriginPos;
    Vector3 originMenuPos;
    Vector3 levelPos;
    Vector3 levelDisPos;

    GameObject level = null;

   
    private void Awake()
    {
        defenseUI = GameObject.Find("DefenseUI").GetComponent<Canvas>();

        if (FindObjectsOfType<DefenseUIManager>().Length >1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        //MapState = 5;
        //Gold = 5000;

        //button is clicked Permit
        onMenu = false;
        WATERRAY = false;

        //Cursor Change Image 
        myCursor = FindObjectOfType<CursorChange>();

        //Reset Value
        ValueInitializing();
        UiInitializing();

        //Back to the Defense Scene transform initializing Scroll
        VegetableMenuOriginPos = VegetableScroll.gameObject.transform.position;
        BuildingMenuOriginPos = BuildingScroll.transform.position;

        if (GameManager.INSTANCE.SCENENUM != 0)
        {
            Debug.Log(this.gameObject.name);
            PhotonManager.INSTANCE.EndGame();

            SaveLoadManager.INSTANCE.Load();
        }
        levelPos = new Vector3(0.06f, 4.1f, -0.16f);
        levelDisPos = new Vector3(10f, 10f, 10f);
    }

    public override void OnEnable()
    {
        base.OnEnable();

        //----------------------------------------------------------
        //StartCoroutine(FindLevel());


        if (SliderBarList != null && GameManager.INSTANCE.SCENENUM == 1)
        {
            SliderBarList.Clear();
        }
        
    }




    private void Start()
    {
        defenseUI.targetDisplay = 1;

    }

    private void Update()
    {
        GoldText.text = Gold.ToString();
        ZeraText.text = MetaTrendAPIHandler.INSTANCE.ZERA.ToString();   

        if (GameManager.INSTANCE.SCENENUM == 1)
        {
            defenseUI.targetDisplay = 0;
        }
        else if (GameManager.INSTANCE.SCENENUM == 2)
        {
            defenseUI.targetDisplay = 1;
        }

        if (masterMode)
        {
            potatoSeedCount = 10;
            appleSeedCount = 10;
            cabbageSeedCount = 10;
            carrotSeedCount = 10;
            eggplantSeedCount = 10;
            houseCount = 10;
            wellCount = 10;
        }

    }

    //Value Reset 
    /////////////////////////////////////////////////////
    private void ValueInitializing()
    {
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;

    }



    private void BringObjectCount()
    {
        curGroundState.GetComponent<TextMeshProUGUI>().text = MapState.ToString();

        PotatoCount.GetComponent<TextMeshProUGUI>().text    = GameManager.INSTANCE.potatoDragonList.Count.ToString();
        AppleCount.GetComponent<TextMeshProUGUI>().text     = GameManager.INSTANCE.appleDragonList.Count.ToString();
        CabbageCount.GetComponent<TextMeshProUGUI>().text   = GameManager.INSTANCE.cabbageDragonList.Count.ToString();
        CarrotCount.GetComponent<TextMeshProUGUI>().text    = GameManager.INSTANCE.carrotDragonList.Count.ToString();
        EggplantCount.GetComponent<TextMeshProUGUI>().text  = GameManager.INSTANCE.eggplantDragonList.Count.ToString();

        curHouseCount.GetComponent<TextMeshProUGUI>().text = houseCount.ToString();
        curWellCount.GetComponent<TextMeshProUGUI>().text = wellCount.ToString();

        curPotato.text   = "Poato : " + potatoSeedCount.ToString();
        curApple.text    = "Apple : " + appleSeedCount.ToString();
        curCarrot.text   = "Carrot : " + carrotSeedCount.ToString();
        curCabbage.text  = "Cabbage : " + cabbageSeedCount.ToString();
        curEggplant.text = "Eggplant : " + eggplantSeedCount.ToString();
        curWell.text     = "Well : " + wellCount.ToString();
        curHouse.text    = "House : " + houseCount.ToString();
        GoldText.text   = Gold.ToString();

        PotatoSeedCount.text = "Seed : " + potatoSeedCount;
        AppleSeedCount.text = "Seed : " + appleSeedCount;
        CabbageSeedCount.text = "Seed : " + cabbageSeedCount;
        CarrotSeedCount.text = "Seed : " + carrotSeedCount;
        CarrotSeedCount.text = "Seed : " + carrotSeedCount;
        EggplantSeedCount.text = "Seed : " + eggplantSeedCount;
    }
    /////////////////////////////////////////////////////


    #region BuildingMode Scroll Func

    //Vegetable Scroll Open&Close
    /////////////////////////////////////////////////////////////////
    public void OpenScrollVegetable()
    {
        BuildingScrollOpenButton.GetComponent<Button>().interactable = false;

        StartCoroutine(OpenScroll(VegetableScroll));

        SwitchBackButton(VegetableScrollOpenButton);
    }
    public void CloseScrollVegetable()
    {
        BuildingScrollOpenButton.GetComponent<Button>().interactable = true;
        StartCoroutine(CloseScroll(VegetableScroll));
        SwitchOpenButton(VegetableScrollCloseButton);
    }
    /////////////////////////////////////////////////////////////////

    //Building Scroll Open&Close
    /////////////////////////////////////////////////////////////////
    public void OpenScrollBuilding()
    {
        VegetableScrollOpenButton.GetComponent<Button>().interactable = false;

        StartCoroutine(OpenScroll(BuildingScroll));

        SwitchBackButton(BuildingScrollOpenButton);
    }
    public void CloseScrollBuilding()
    {
        VegetableScrollOpenButton.GetComponent<Button>().interactable = true;
        StartCoroutine(CloseScroll(BuildingScroll));
        SwitchOpenButton(BuildingScrollCloseButton);
    }
    /////////////////////////////////////////////////////////////////

    //Scroll Button Chane (Switch Go & Back)
    /////////////////////////////////////////////////////////////////
    void SwitchBackButton(GameObject button)
    {
        if(button == VegetableScrollOpenButton)
        {
            VegetableScrollOpenButton.SetActive(false);
            VegetableScrollCloseButton.SetActive(true);
        }
        else if (button == BuildingScrollOpenButton)
        {
            BuildingScrollOpenButton.SetActive(false);
            BuildingScrollCloseButton.SetActive(true);
        }
    }
    void SwitchOpenButton(GameObject button)
    {
        if (button == VegetableScrollCloseButton)
        {
            VegetableScrollOpenButton.SetActive(true);
            VegetableScrollCloseButton.SetActive(false);
        }
        else if (button == BuildingScrollCloseButton)
        {
            BuildingScrollOpenButton.SetActive(true);
            BuildingScrollCloseButton.SetActive(false);
        }
    }
    /////////////////////////////////////////////////////////////////

    #endregion
    #region BuildingMenu Open&Close Corutine

    IEnumerator OpenScroll(GameObject scroll)
    {
        originMenuPos = scroll.transform.position;
        while(true)
        {
            scroll.transform.position = 
                Vector3.Lerp(scroll.transform.position, scroll.transform.position + new Vector3(100f, 0, 0), Time.deltaTime * 15f);
            yield return null;

            if(scroll.transform.position.x>=450f)
            {
                yield break;
            }
        }
        
    }
    IEnumerator CloseScroll(GameObject scroll)
    {
        while (true)
        {
            scroll.transform.position = 
                Vector3.Lerp(scroll.transform.position, scroll.transform.position + new Vector3(-100f, 0, 0), Time.deltaTime * 25f);
            yield return null;

            if(scroll.transform.position.x<=-350f)
            {
                scroll.transform.position = originMenuPos;
                yield break;
            }
        }
        
    }

    #endregion

    //Building Mode & home
    /////////////////////////////////////////////////////
    public void Go_buildingMode()
    {
        if (onMenu) return;
        onMenu = true;

        BringObjectCount();
        InstButtonTurnOff();
        BUILDINGMODE = true;
        GardeningMenu.gameObject.SetActive(false);
        BulidingModeMenu.SetActive(true);
        for (int i = 0; i < SliderBarList.Count; i++)
        {
            SliderBarList[i].gameObject.SetActive(false);
        }

    }
    public void Back_buildingMode()
    {
        if (!onMenu) return;
        if (onMenu)
            onMenu = false;

        VegetableScroll.transform.position = VegetableMenuOriginPos;
        BuildingScroll.transform.position = BuildingMenuOriginPos;
        SwitchOpenButton(VegetableScrollCloseButton);
        SwitchOpenButton(BuildingScrollCloseButton);

        BuildingScrollOpenButton.GetComponent<Button>().interactable = true;
        VegetableScrollOpenButton.GetComponent<Button>().interactable = true;

        BUILDINGMODE = false;
        GardeningMenu.gameObject.SetActive(true);
        BulidingModeMenu.SetActive(false);
        ValueInitializing();
        ObjectPoolingManager.inst.ObjectDisappear();

        myCursor.BasicCursor();

        for (int i = 0; i < SliderBarList.Count; i++)
        {
            SliderBarList[i].gameObject.SetActive(true);
        }


    }
    void InstButtonTurnOff()
    {
        if (potatoSeedCount <= 0)
        {
            InstButton[0].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[0].GetComponent<Button>().interactable = true;
        }

        if (appleSeedCount <= 0)
        {
            InstButton[1].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[1].GetComponent<Button>().interactable = true;
        }

        if (cabbageSeedCount <= 0)
        {
            InstButton[2].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[2].GetComponent<Button>().interactable = true;
        }

        if (carrotSeedCount <= 0)
        {
            InstButton[3].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[3].GetComponent<Button>().interactable = true;
        }

        if (eggplantSeedCount <= 0)
        {
            InstButton[4].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[4].GetComponent<Button>().interactable = true;
        }

        if (houseCount <= 0)
        {
            InstButton[5].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[5].GetComponent<Button>().interactable = true;
        }

        if (wellCount <= 0)
        {
            InstButton[6].GetComponent<Button>().interactable = false;
        }
        else
        {
            InstButton[6].GetComponent<Button>().interactable = true;
        }

    }
    /////////////////////////////////////////////////////
    #region UI Transform Up&Down Corutine
    IEnumerator Uptrans(GameObject page)
    {
        while (true)
        {
            page.transform.position = Vector3.Lerp(page.transform.position, page.transform.position + new Vector3(0, 100f, 0), Time.deltaTime * 20f);
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
                for (int i = 0; i < SliderBarList.Count; i++)
                {
                    SliderBarList[i].gameObject.SetActive(true);
                }

                yield break;
            }
        }

    }
    #endregion

    
    //StorePage
    /////////////////////////////////////////////////////
    #region ClickStorePage
    public void ClickStoreVegetablePage()
    {
        VegetableScrollView.gameObject.SetActive(true);
        BuildingScrollView.gameObject.SetActive(false);
        GroundScrollView.gameObject.SetActive(false);

        StoreVegetablePage.GetComponent<Button>().interactable = false;
        StoreBuildingPage.GetComponent<Button>().interactable = true;
        StoreGroundPage.GetComponent<Button>().interactable = true;
    }
    public void ClickStoreBildingPage()
    {
        BuildingScrollView.gameObject.SetActive(true);
        VegetableScrollView.gameObject.SetActive(false);
        GroundScrollView.gameObject.SetActive(false);

        StoreVegetablePage.GetComponent<Button>().interactable = true;
        StoreBuildingPage.GetComponent<Button>().interactable = false;
        StoreGroundPage.GetComponent<Button>().interactable = true;
    }
    public void ClickStoreGroundPage()
    {
        curGroundState.GetComponent<TextMeshProUGUI>().text = MapState.ToString();

        GroundScrollView.gameObject.SetActive(true);
        VegetableScrollView.gameObject.SetActive(false);
        BuildingScrollView.gameObject.SetActive(false);

        StoreVegetablePage.GetComponent<Button>().interactable = true;
        StoreBuildingPage.GetComponent<Button>().interactable = true;
        StoreGroundPage.GetComponent<Button>().interactable = false;
    }

    #endregion
    #region SellObject
    public void SellPotatoDragon()
    {
        if (GameManager.INSTANCE.DragonCount("D_Potatagon") == 0) return;

        //gold calculation
        Gold += dragonData[0].SalePrice;
        GoldText.text = Gold.ToString();

        //bring instance list 
        List<GameObject> instList = new List<GameObject>();
        GameManager.INSTANCE.dragonTable.TryGetValue("D_Potatagon", out instList);

        //remove obj and list
        Destroy(instList[0]);
        GameManager.INSTANCE.RemoveDragonCount("D_Potatagon");


        //생성할때 text에 연결해줘야함
        //리스트는 정상동작하고있음
        //text update 확인해주기
        PotatoCount.GetComponent<TextMeshProUGUI>().text = GameManager.INSTANCE.DragonCount("D_Potatagon").ToString();
    }

    public void SellAppleDragon()
    {
        if (GameManager.INSTANCE.appleDragonList.Count == 0) return;

        Gold += dragonData[1].SalePrice;
        GoldText.text = Gold.ToString();

        ObjectPoolingManager.inst.Destroy(GameManager.INSTANCE.appleDragonList[0]);
        GameManager.INSTANCE.appleDragonList.RemoveAt(0);
        BringObjectCount();
    }
    public void SellCabbageDragon()
    {
        if (GameManager.INSTANCE.cabbageDragonList.Count == 0) return;

        Gold += dragonData[2].SalePrice;
        GoldText.text = Gold.ToString();

        ObjectPoolingManager.inst.Destroy(GameManager.INSTANCE.cabbageDragonList[0]);
        GameManager.INSTANCE.cabbageDragonList.RemoveAt(0);
        BringObjectCount();
    }
    public void SellCarrotDragon()
    {
        if (GameManager.INSTANCE.carrotDragonList.Count == 0) return;

        Gold += dragonData[3].SalePrice;
        GoldText.text = Gold.ToString();

        ObjectPoolingManager.inst.Destroy(GameManager.INSTANCE.carrotDragonList[0]);
        GameManager.INSTANCE.carrotDragonList.RemoveAt(0);
        BringObjectCount();
    }
    public void SellEggplantDragon()
    {
        if (GameManager.INSTANCE.eggplantDragonList.Count == 0) return;

        Gold += dragonData[4].SalePrice;
        GoldText.text = Gold.ToString();

        ObjectPoolingManager.inst.Destroy(GameManager.INSTANCE.eggplantDragonList[0]);
        GameManager.INSTANCE.eggplantDragonList.RemoveAt(0);
        BringObjectCount();
    }
    #endregion
    #region BuyObject
    public void BuyPotatoSeed()
    {
        potatoSeedCount++;
        Gold -= vegetableData[0].PurchasePrice;
        GoldText.text = Gold.ToString();
        PotatoSeedCount.text = "Seed : " + potatoSeedCount;

    }
    public void BuyAppleSeed()
    {
        appleSeedCount++;
        Gold -= vegetableData[1].PurchasePrice;
        GoldText.text = Gold.ToString();
        AppleSeedCount.text = "Seed : " + appleSeedCount;
    }
    public void BuyCabbageSeed()
    {
        cabbageSeedCount++;
        Gold -= vegetableData[2].PurchasePrice;
        GoldText.text = Gold.ToString();
        CabbageSeedCount.text = "Seed : " + cabbageSeedCount;
    }
    public void BuyCarrotSeed()
    {
        carrotSeedCount++;
        Gold -= vegetableData[3].PurchasePrice;
        GoldText.text = Gold.ToString();
        CarrotSeedCount.text = "Seed : " + carrotSeedCount;
    }
    public void BuyEggplantSeed()
    {
        eggplantSeedCount++;
        Gold -= vegetableData[4].PurchasePrice;
        GoldText.text = Gold.ToString();
        EggplantSeedCount.text = "Seed : " + eggplantSeedCount;
    }
    public void BuyHouse()
    {
        houseCount++;
        Gold -= housePrice;
        GoldText.text = Gold.ToString();
        BringObjectCount();
    }
    public void BuyWell()
    {
        wellCount++;
        Gold -= wellPrice;
        GoldText.text = Gold.ToString();
        BringObjectCount();
    }

    public void BuyGround()
    {
        if (MapState == 0) return;

        if (SaveLoadManager.INSTANCE.instMaplevel != null)
        {
            SaveLoadManager.INSTANCE.instMaplevel.transform.position = levelDisPos;
        }

        // if (level != null) Destroy(level);
        if (level != null) level.transform.position = levelDisPos;

        MapState--;
        curGroundState.GetComponent<TextMeshProUGUI>().text = MapState.ToString();
        GameObject levelPrefab = Resources.Load<GameObject>("L_MapState_" + MapState.ToString());

        if (MapState == 0)
        {
            GroundBuyButton.interactable = false;
            return;
        }
            
        level = Instantiate(levelPrefab, levelPos, Quaternion.identity);

    }

    #endregion
    public void UpStorePage()
    {
        if (onMenu)
        {
            OnShopExitButton();
            return;
        }
        onMenu = true;

        StoreVegetablePage.GetComponent<Button>().interactable = false;
        BuildingScrollView.gameObject.SetActive(false);
        GroundScrollView.gameObject.SetActive(false);

        StartCoroutine(Uptrans(StorePage));

        for (int i = 0; i < SliderBarList.Count; i++)
        {
            SliderBarList[i].gameObject.SetActive(false);
        }

    }
    public void OnShopExitButton()
    {
        if (!onMenu) return;
        if (onMenu)
            onMenu = false;
        ClickStoreVegetablePage();
        StartCoroutine(Downtrans(StorePage));
        for (int i = 0; i < SliderBarList.Count; i++)
        {
            SliderBarList[i].gameObject.SetActive(true);
        }
    }
    /////////////////////////////////////////////////////
    #region Select Building&Vegetable Button
    public void SelectWell()
    {
        if (wellCount <=0) return;
        wellCount--;
        InstButtonTurnOff();
        curWell.text = "Well : " + wellCount.ToString();

        CloseScrollBuilding();
        //myCursor.BuildingCursor();
        DefenseUIManager.INSTANCE.onWELL = true;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    public void SelectHouse()
    {
        if (houseCount <= 0) return;
        //houseCount--;
        InstButtonTurnOff();
        curHouse.text = "House : " + houseCount.ToString();

        CloseScrollBuilding();
        //myCursor.BuildingCursor();
        DefenseUIManager.INSTANCE.onHOUSE = true;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    public void SelectPotato()
    {
        if (potatoSeedCount <= 0) return;
        //potatoSeedCount--;
        InstButtonTurnOff();
        curPotato.text = "Poato : "+potatoSeedCount.ToString();

        CloseScrollVegetable();
        //myCursor.VegetableCursor();
        DefenseUIManager.INSTANCE.onPOTATO = true;
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    public void SelectApple()
    {
        if (appleSeedCount <= 0) return;
        appleSeedCount--;
        InstButtonTurnOff();
        curApple.text = "Apple : " + appleSeedCount.ToString();

        CloseScrollVegetable();
        //myCursor.VegetableCursor();
        DefenseUIManager.INSTANCE.onAPPLE = true;
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    public void SelectCabbage()
    {
        if (cabbageSeedCount <= 0) return;
        cabbageSeedCount--;
        InstButtonTurnOff();

        curCabbage.text = "Cabbage : " + cabbageSeedCount.ToString();

        CloseScrollVegetable();
        //myCursor.VegetableCursor();
        DefenseUIManager.INSTANCE.onCABBAGE = true;
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    public void SelectCarrot()
    {
        if (carrotSeedCount <= 0) return;
        carrotSeedCount--;
        InstButtonTurnOff();

        curCarrot.text = "Carrot : " + carrotSeedCount.ToString();

        CloseScrollVegetable();
        //myCursor.VegetableCursor();
        DefenseUIManager.INSTANCE.onCARROT = true;
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onEEGPLANT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    public void SelectEggplant()
    {
        if (eggplantSeedCount <= 0) return;
        eggplantSeedCount--;
        InstButtonTurnOff();

        curEggplant.text = "Eggplant : " + eggplantSeedCount.ToString();

        CloseScrollVegetable();
        //myCursor.VegetableCursor();
        DefenseUIManager.INSTANCE.onEEGPLANT = true;
        DefenseUIManager.INSTANCE.onWELL = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onHOUSE = false;
        DefenseUIManager.INSTANCE.onPOTATO = false;
        DefenseUIManager.INSTANCE.onAPPLE = false;
        DefenseUIManager.INSTANCE.onCABBAGE = false;
        DefenseUIManager.INSTANCE.onCARROT = false;
        ObjectPoolingManager.inst.ObjectDisappear();
    }
    #endregion



}
