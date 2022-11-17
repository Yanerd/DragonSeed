using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleTon<GameManager>
{
    public int SCENENUM { get; set; }

    #region Invader client offense value
    //player state value
    public bool ISLOCKON { get; set; }
    public bool ISDEAD { get; set; }
    
    //player invasion value
    public bool WANTINVASION { get; set; }
    public int KILLCOUNT { get; set; }
    public int DESTROYPLANTCOUNT { get; set; }
    public int DESTROYBUILDINGCOUNT { get; set; }
    #endregion

    #region defense client value
    //master clients value
    public bool INVASIONALLOW { get; set; }
    public int TOTALDRAGONCOUNT { get; set; }
    public int TOTALSEEDCOUNT { get; set; }
    public int TOTALBUILDINGCOUNT { get; set; }
    public int TOTALCOIN { get; set;}
    #endregion

    #region Invasion Game Controll Value
    public bool ISGAMEIN { get; set; }
    public bool ISDEFENSE { get; set; }
    public float GAMETIME { get; set; }
    public int STEALCOIN { get; set; }
    public bool HouseBurn { get; set; }
    public bool ISTIMEOVER { get; set; }
    #endregion

    //게임 정상종료 확인 변수
    public bool GameEndCorrect { get; set; }

    #region Dragon List

    public List<GameObject> potatoDragonList = new List<GameObject>();
    public List<GameObject> appleDragonList = new List<GameObject>();
    public List<GameObject> cabbageDragonList = new List<GameObject>();
    public List<GameObject> carrotDragonList = new List<GameObject>();
    public List<GameObject> eggplantDragonList = new List<GameObject>();

    public List<GameObject> dragons = new List<GameObject>();

    #endregion

    public Coroutine timerCoroutine = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);    
    }

    void Start()
    {
        GameManager.INSTANCE.SCENENUM = 0;
        Initializing();
    }
    public void Initializing() //init function
    {
        //player state
        GameManager.INSTANCE.ISDEAD = false;
        GameManager.INSTANCE.ISLOCKON = false;

        //invader client value
        GameManager.INSTANCE.WANTINVASION = false;
        GameManager.INSTANCE.KILLCOUNT = 0;
        GameManager.INSTANCE.DESTROYPLANTCOUNT = 0;
        GameManager.INSTANCE.DESTROYBUILDINGCOUNT = 0;

        //defense client value
        GameManager.INSTANCE.INVASIONALLOW = false;
        GameManager.INSTANCE.TOTALDRAGONCOUNT = 0;
        GameManager.INSTANCE.TOTALSEEDCOUNT = 0;
        GameManager.INSTANCE.TOTALBUILDINGCOUNT = 0;
        GameManager.INSTANCE.TOTALCOIN = 0;

        //Invasion Game Controll Value
        GameManager.INSTANCE.ISGAMEIN = false;

        GameManager.INSTANCE.ISDEFENSE = true;
        GameManager.INSTANCE.GAMETIME = 0;
        GameManager.INSTANCE.STEALCOIN = 0;
        GameManager.INSTANCE.ISTIMEOVER = false;

        GameManager.INSTANCE.TimerClear();

        //
        GameManager.INSTANCE.HouseBurn = false;

        GameEndCorrect = false;
    }

    public void TimerClear()
    {
        if (timerCoroutine == null)
            return;

        StopCoroutine(timerCoroutine);
        timerCoroutine = null;
    }

    public void TimerStart()
    {
        timerCoroutine = StartCoroutine(TimeCount());
    }
    public void TimeOut()
    {
        StopCoroutine(timerCoroutine);
    }

    IEnumerator TimeCount() //invasion timer
    {
        while (true)
        {
            GameManager.INSTANCE.GAMETIME += Time.deltaTime;

            //invader is win
            if (GameManager.INSTANCE.GAMETIME > 60f)//->game time limit
            {
                CoinRavish();
                Time.timeScale = 0f;
                GameManager.INSTANCE.ISTIMEOVER = true;

                yield break;
            }

            if (GameManager.INSTANCE.ISDEAD)
                yield break;

            yield return null;
        }
    }

    public void CoinRavish()//coin ravish calculation
    {
        float stealCoinKillScale = 0f;
        float stealCoinSaboScale = 0f;
        float killCoinPoint = 0f;
        float saboCoinPoint = 0f;

        //KillCoin scale calculation
        {
            float numerator =   ((float)GameManager.INSTANCE.KILLCOUNT + (float)GameManager.INSTANCE.DESTROYPLANTCOUNT);
            float denominator = ((float)GameManager.INSTANCE.TOTALDRAGONCOUNT + (float)GameManager.INSTANCE.TOTALSEEDCOUNT);
            stealCoinKillScale = numerator / denominator;
        }

        //SaboCoin scale calculation
        {
            float numerator =   ((float)GameManager.INSTANCE.DESTROYBUILDINGCOUNT);
            float denominator = ((float)GameManager.INSTANCE.TOTALBUILDINGCOUNT);
            stealCoinSaboScale = numerator / denominator;
        }

        //kill coin calculation
        {
            float numerator =   ((float)GameManager.INSTANCE.TOTALCOIN * stealCoinKillScale);
            float denominator = (20f);
            killCoinPoint =  numerator / denominator;
        }

        //sabo coin calculation
        {
            float numerator = ((float)GameManager.INSTANCE.TOTALCOIN * stealCoinSaboScale);
            float denominator = (20f);
            saboCoinPoint = numerator / denominator;
        }

        //real steal coin calculation
        GameManager.INSTANCE.STEALCOIN = (int)(killCoinPoint + saboCoinPoint);
    }

    #region Dragon Dictionary
    // Dictionary 로 바꾸기
    public Dictionary<string, List<GameObject>> dragonTable = new Dictionary<string, List<GameObject>>();

    // 드래곤이 처음 출현했을 때 -> Awake
    public void AllDragonCount(GameObject obj)
    {
        List<GameObject> list = null;

        string prefabId = obj.name.Replace("(Clone)", "");
        bool listCheck = dragonTable.TryGetValue(prefabId, out list);


        if (listCheck == false) // 이 이름의 리스트가 없을 때
        {
            list = new List<GameObject>(); // 새로 리스트 생성 -> 해당하는 키값의 리스트를 만들어줌            
            GameManager.INSTANCE.dragonTable.Add(prefabId, list); // list를 dictionary 안에 넣기

        }
        list.Add(obj); // dictionary 안에 있는 list에 prefab을 넣음

    }

    // 드래곤이 판매가 되었을 때
    public void RemoveDragonCount(string name)
    {

        List<GameObject> list;

        //string prefabId = name.Replace("(Clone)", "");
        bool listCheck = GameManager.INSTANCE.dragonTable.TryGetValue(name, out list);

        if (listCheck == false)
        {
            Debug.LogError("Not Found " + name);
            return;
        }

        // 리스트가 있을 때 드래곤 지움
        list.RemoveAt(0);
    }

    // 드래곤 카운트 세는 함수
    public int DragonCount(string name)
    {
        List<GameObject> list;

        string prefabId = name.Replace("(Clone)", "");
        bool listCheck = GameManager.INSTANCE.dragonTable.TryGetValue(prefabId, out list);

        int dragonCount = 0;

        if (listCheck == true)
        {
            dragonCount = list.Count;
            return dragonCount;
        }
        return 0;
    }

    public void DragonTableClear()
    {
        GameManager.INSTANCE.dragonTable.Clear();
    }
    #endregion
}
