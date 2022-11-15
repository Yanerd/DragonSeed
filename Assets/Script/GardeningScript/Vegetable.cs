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
    [SerializeField] Canvas Canvas;


    int  WaterCount;
    float GrowthTime;
    float OnSearchTime;
    float LooseTime;
    int PurchasePrice;

    [SerializeField] public float GrowthValue;
    [SerializeField] public int CountValue;

    private void Awake()
    {
        Initializing();

        DefenseUIManager.INSTANCE.SliderBarList.Add(growBar);
        DefenseUIManager.INSTANCE.SliderBarList.Add(CountBar);

        CountBar.maxValue = WaterCount;
        CountBar.minValue = 0;
        CountBar.value = 0;
        growBar.maxValue = GrowthTime;



        if (DefenseUIManager.INSTANCE.BUILDINGMODE==false)
        {
            if (GameManager.INSTANCE.SCENENUM == 2)
            {
                InstOffenseVegetable(SaveLoadManager.INSTANCE.vegetableGrowBarValue, SaveLoadManager.INSTANCE.vegetableWaterValue);
            }
            else if (GameManager.INSTANCE.SCENENUM == 1 || GameManager.INSTANCE.SCENENUM == 0)
            {
                InstGardeningVegetable(SaveLoadManager.INSTANCE.vegetableGrowBarValue, SaveLoadManager.INSTANCE.vegetableWaterValue);
            }
        }

       
    }


    private void Start()
    {

        if (GameManager.INSTANCE.SCENENUM == 1)
        {
            DefenseUIManager.INSTANCE.SliderBarList.Add(growBar);
            DefenseUIManager.INSTANCE.SliderBarList.Add(CountBar);
        }

        if (GameManager.INSTANCE.ISGAMEIN)
        {
            growBar.gameObject.SetActive(false);
            CountBar.gameObject.SetActive(false);
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

    void InstGardeningVegetable(float growthValue, int countValue)
    {

        if (growthValue>= GrowthTime)
        {
            this.GrowthValue = growthValue;
            this.CountValue = countValue;

            growBar.value = growthValue;
            CountBar.value = countValue;

            growBar.gameObject.SetActive(false);
            CountBar.gameObject.SetActive(false);
            Seed.enabled = false;
            Stem.gameObject.SetActive(false);
            this.gameObject.name = "B_Mud";
            return;
        }
        this.GrowthValue = growthValue;
        this.CountValue = countValue;

        growBar.value = growthValue;
        CountBar.value = countValue;

        if (CountBar.value == WaterCount)
        {
            CountBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
            if (GrownDragon == true) return;
            Debug.Log("자라나라");
            StartCoroutine(GrowthSeed());
        }
    }

    void InstOffenseVegetable(float growthValue, int countValue)
    {
        Debug.Log("ㅎㅇ");
        Debug.Log(growthValue.ToString());

        this.GrowthValue = growthValue;
        this.CountValue = countValue;

        growBar.value = growthValue;
        CountBar.value = countValue;

        growBar.gameObject.SetActive(false);
        CountBar.gameObject.SetActive(false);

        if (growthValue>= GrowthTime /2&& growthValue< GrowthTime)
        {
            Seed.enabled = false;
            Stem.gameObject.SetActive(true);
        }
        else if(growthValue>=1f)
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
            Debug.Log("슬라이더 꺼져");
            growBar.gameObject.SetActive(false);
            CountBar.gameObject.SetActive(false);
            this.gameObject.name = "B_Mud";
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
            if (growBar.value >= GrowthTime /2&& SateStem==false)
            {
                StemStat();
                SateStem = true;
                this.gameObject.name = this.gameObject.name+"_Stemp";
            }
            if (growBar.value >= GrowthTime && SateGrown==false)
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
            GrowthValue += 0.1f;

            if (GameManager.INSTANCE.INVASIONALLOW == true) yield return new WaitForSeconds(0.05f);
            else yield return new WaitForSeconds(0.1f);

            growBar.value = GrowthValue;

            if (growBar.value>= GrowthTime)
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
