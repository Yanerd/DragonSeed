using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vegetable : MonoBehaviour
{

    [Header("VegetableObject")]
    [SerializeField] MeshRenderer Seed;
    [SerializeField] GameObject Stem;
    [SerializeField] GameObject Effect;

    [Header("DropFruit")]
    [SerializeField] DropFruit[] fruit;

    [Header("DragonPrefab")]
    [SerializeField] GameObject Dragon;

    [Header("DragonState")]
    [SerializeField] public bool onWater=false;
    [SerializeField] bool GrownDragon;

    [SerializeField] VegetableData vegetableData;
    [SerializeField] Slider growBar;
    [SerializeField] Slider CountBar;
    int  WaterCount;
    float GrowthTime;
    float OnSearchTime;
    float LooseTime;
    int PurchasePrice;

    [SerializeField] public float GrowthValue;
    [SerializeField] public int CountValue;

    private void Awake()
    {
        
    }


    private void Start()
    {
        Initializing();

        CountBar.maxValue = WaterCount;
        CountBar.minValue = 0;
        CountBar.value = 0;

        if (GameManager.INSTANCE.SCENENUM == 1)
        {
            //슬라이더를 찾아서 게임인이 될 경우 끈다.
            if (GameManager.INSTANCE.ISGAMEIN)
            {
                growBar.gameObject.SetActive(false);
                CountBar.gameObject.SetActive(false);
            }


            DefenseUIManager.INSTANCE.SliderBarList.Add(growBar);
            DefenseUIManager.INSTANCE.SliderBarList.Add(CountBar);
        }
    }

    void Initializing()
    {
        if (vegetableData != null)
        {
            WaterCount = vegetableData.WaterCount;
            GrowthTime = vegetableData.GrowthTime;
            OnSearchTime = vegetableData.OnSearchTime;
            LooseTime = vegetableData.LooseTime;
            PurchasePrice = vegetableData.PurchasePrice;
        }
        else
        {
            Debug.Log("ref error");
        }
       
    }


    public void StartGrowth()
    {
        if (CountBar.value == WaterCount) return;
        
        Debug.Log("물 들어옴");
        Debug.Log(CountBar.maxValue);

        CountValue += 1;
        CountBar.value = CountValue;

        if (CountBar.value == WaterCount)
        {
            CountBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
            if (GrownDragon == true) return;
            Debug.Log("자라나라");
            StartCoroutine(GrowthSeed());
        }
    }

    public void PhotonInstDefenseVegetable(float growthValue, int countValue)
    {
        if (CountBar.value == WaterCount) return;

        this.GrowthValue = growthValue;
        this.CountValue = countValue;

        CountBar.value = CountValue;

        if (CountBar.value == WaterCount)
        {
            CountBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
            if (GrownDragon == true) return;
            Debug.Log("자라나라");
            StartCoroutine(GrowthSeed());
        }
    }

    public void PhotonInstOffenseVegetable(float growthValue, int countValue)
    {
        this.GrowthValue = growthValue;
        this.CountValue = countValue;

        growBar.gameObject.SetActive(false);
        CountBar.gameObject.SetActive(false);

        if (growthValue>=0.5f&& growthValue<1)
        {
            Seed.enabled = false;
            Stem.gameObject.SetActive(true);
        }
        if(growthValue>=1)
        {
            Seed.enabled = false;
            Stem.gameObject.SetActive(false);
        }

    }



    void StemStat()
    {
        Seed.enabled = false;
        Stem.gameObject.SetActive(true);

        StartCoroutine(onEffect());
        for (int i = 0; i < fruit.Length; i++)
        {
            fruit[i].SendMessage("dropObject", SendMessageOptions.DontRequireReceiver);
        }
    }
    void GrownState()
    {
        StartCoroutine(onEffect());

        Stem.gameObject.SetActive(false);

        if (GrownDragon == false)
        {
            growBar.gameObject.SetActive(false);
            CountBar.gameObject.SetActive(false);   
            InstantiateDragon();
        }
    }

    IEnumerator GrowthSeed()
    {
        StartCoroutine(ValueChange());
        bool SateStem = false;
        bool SateGrown = false;
        while (true)
        {
            if (growBar.value >= 0.5f&& SateStem==false)
            {
                StemStat();
                SateStem = true;
            }
            if (growBar.value >= 1f&& SateGrown==false)
            {
                GrownState();
                SateGrown = true;
                yield break;
            }

            yield return null;
        }
       
    }
    IEnumerator ValueChange()
    {
        
        while(true)
        {
            yield return new WaitForSeconds(1f);
            GrowthValue += 0.1f;
            growBar.value = GrowthValue;
            if (DefenseUIManager.INSTANCE.invadePermit==true)
            {
                GrowthValue += 0.1f;
                growBar.value = GrowthValue;
            }
            if(growBar.value>=1f)
            {
                growBar.gameObject.SetActive(false);
                CountBar.gameObject.SetActive(false);
                yield break;
            }
        }
    }


    void InstantiateDragon()
    {
        GrownDragon = true;
        GameObject instDragon = ObjectPoolingManager.inst.Instantiate(Dragon, transform.position, Quaternion.Euler(0, -180f, 0), ObjectPoolingManager.inst.PoolingZone);

        AllDragonCount(instDragon);
    }
    void AllDragonCount(GameObject instDragon)
    {
        string prefabId = instDragon.name.Replace("(Clone)", "");

        if (prefabId == "D_Potatagon")
        {
            GameManager.INSTANCE.potatoDragonList.Add(instDragon);
        }
        else if(prefabId == "D_Appleagon")
        {
            GameManager.INSTANCE.appleDragonList.Add(instDragon);
        }
        else if (prefabId == "D_Carrotagon")
        {
            GameManager.INSTANCE.carrotDragonList.Add(instDragon);
        }
        else if (prefabId == "D_Cabbagon")
        {
            GameManager.INSTANCE.cabbageDragonList.Add(instDragon);
        }
        else if (prefabId == "D_Eggplagon")
        {
            GameManager.INSTANCE.eggplantDragonList.Add(instDragon);
        }

    }

    IEnumerator onEffect()
    {
        Effect.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        Effect.gameObject.SetActive(false);
    }
}
