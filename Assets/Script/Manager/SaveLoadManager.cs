using AESWithJava.Con;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class Data
{
    public string saveListCount;

    public string waterListCount;
    public string plantBarListCount;
    public string wellBarListCount;

    public List<string> xPos = new List<string>();
    public List<string> yPos = new List<string>();
    public List<string> zPos = new List<string>();
    public List<string> name = new List<string>();
    public List<string> water = new List<string>();
    public List<string> plantBar = new List<string>();
    public List<string> wellBar = new List<string>();
    public string potatoSeedCount;
    public string appleSeedCount;
    public string cabbageSeedCount;
    public string carrotSeedCount;
    public string eggplantSeedCount;
    public string houseCount;
    public string wellCount;



    public string groundState;
}

public class SaveLoadManager : MonoSingleTon<SaveLoadManager>
{
    public int? objectCount;
    public int waterCount;
    public int plantBarCount;
    public int wellBarCount;

    public string str;
    public string xpos;
    public string ypos;
    public string zpos;

    int convertWaterCount;
    float convertPlantBar;
    int convertGroundState;
    float convertWellBar;

    public int potatoSeedCount { get; set; }
    public int appleSeedCount { get; set; }
    public int cabbageSeedCount { get; set; }
    public int carrotSeedCount { get; set; }
    public int eggplantSeedCount { get; set; }
    public int HouseCount { get; set; }
    public int WellCount { get; set; }

    Transform[] fence = new Transform[5];
    Transform[] tree = new Transform[4];

    private GameObject PoolingZone;

    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Dragon Seed/" + "/Saves/";
    string fileName = "/Save01.json";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void InitLoad()
    {
        if (Directory.Exists(path) == true) // There's no directory
        {
            Load();
        }
    }

    public void Save()
    {
        //clear instance list
        GameManager.INSTANCE.DragonTableClear();

        //save data to json 
        PoolingZone = GameObject.Find("PoolingZone");

        if (Directory.Exists(path) == false) // There's no directory
        {
            Directory.CreateDirectory(path);
        }

        Debug.Log("저장시작" + Time.time);

        objectCount = PoolingZone.transform.childCount;
        Data data = new Data();

        data.groundState = DefenseUIManager.INSTANCE.MapState.ToString();
        data.potatoSeedCount = DefenseUIManager.INSTANCE.potatoSeedCount.ToString();
        data.appleSeedCount = DefenseUIManager.INSTANCE.appleSeedCount.ToString();
        data.cabbageSeedCount = DefenseUIManager.INSTANCE.cabbageSeedCount.ToString();
        data.carrotSeedCount = DefenseUIManager.INSTANCE.carrotSeedCount.ToString();
        data.eggplantSeedCount = DefenseUIManager.INSTANCE.eggplantSeedCount.ToString();
        data.houseCount = DefenseUIManager.INSTANCE.houseCount.ToString();
        data.wellCount = DefenseUIManager.INSTANCE.wellCount.ToString();

        for (int i = 0; i < PoolingZone.transform.childCount; i++)
        {
            str = PoolingZone.transform.GetChild(i).name;
            xpos = PoolingZone.transform.GetChild(i).position.x.ToString();
            ypos = PoolingZone.transform.GetChild(i).position.y.ToString();
            zpos = PoolingZone.transform.GetChild(i).position.z.ToString();
            string[] words = str.Split('(');
            string[] vwords = str.Split('_');
            data.name.Add(words[0]);
            data.xPos.Add(xpos);
            data.yPos.Add(ypos);
            data.zPos.Add(zpos);
            data.saveListCount = objectCount == null ? "0" : objectCount.ToString();


            if (words[0] == "Well")
            {
                data.wellBar.Add(PoolingZone.transform.GetChild(i).GetComponent<WellBar>().FillValue.ToString());
            }

            if (vwords[0] == "V")
            {
                data.plantBar.Add(PoolingZone.transform.GetChild(i).GetComponent<Vegetable>().GrowthValue.ToString());
                data.water.Add(PoolingZone.transform.GetChild(i).GetComponent<Vegetable>().CountValue.ToString());
            }

            data.waterListCount = data.water.Count.ToString();
        }


        Debug.Log("저장완료" + Time.time);


        File.WriteAllText(path + fileName, Encryption.Encrypt((JsonUtility.ToJson(data)), "key"));
    }

    public void Load()
    {
        int ObjCount;
        float convertX;
        float convertY;
        float convertZ;

        string parse = File.ReadAllText(path + fileName);
        File.WriteAllText(path + "/Read.json", Encryption.Decrypt(parse, "key"));

        Data data2 = JsonUtility.FromJson<Data>(File.ReadAllText(path + "/Read.json"));

        int waterListCount = 0;
        int wellBarListCount = 0;

        ObjCount           = int.Parse(data2.saveListCount     == "" ? "0" : data2.saveListCount);
        potatoSeedCount    = int.Parse(data2.potatoSeedCount   == "" ? "0" : data2.potatoSeedCount);
        appleSeedCount     = int.Parse(data2.appleSeedCount    == "" ? "0" : data2.appleSeedCount);
        cabbageSeedCount   = int.Parse(data2.cabbageSeedCount  == "" ? "0" : data2.cabbageSeedCount);
        carrotSeedCount    = int.Parse(data2.carrotSeedCount   == "" ? "0" : data2.carrotSeedCount);
        eggplantSeedCount  = int.Parse(data2.eggplantSeedCount == "" ? "0" : data2.eggplantSeedCount);
        HouseCount         = int.Parse(data2.houseCount        == null ? "0" : data2.houseCount);
        WellCount          = int.Parse(data2.wellCount         == null ? "0" : data2.wellCount);
        convertGroundState = int.Parse(data2.groundState       == null ? "0" : data2.groundState);

        for (int i = 0; i < ObjCount; i++)
        {
            convertX = float.Parse(data2.xPos[i]);
            convertY = float.Parse(data2.yPos[i]);
            convertZ = float.Parse(data2.zPos[i]);

            Vector3 vector3 = new Vector3(convertX, convertY, convertZ);

            Init(data2.name[i], vector3);

        }

        GroundInit();
    }

    public void Init(string name, Vector3 pos)
    {
        GameObject obj = Resources.Load<GameObject>(name);
        GameObject instObject = null;

        if (GameManager.INSTANCE.ISGAMEIN == true)
        {
            instObject = PhotonNetwork.Instantiate(name, pos, Quaternion.identity);
        }
        else
        {
            PoolingZone = GameObject.Find("PoolingZone");
            instObject = Instantiate(obj, pos, Quaternion.identity, PoolingZone.transform);
        }
    }

    public void VegetableInit(string name, Vector3 pos, float growthValue, int countValue)
    {
        GameObject obj;
        GameObject instObject;

        obj = Resources.Load<GameObject>($"Prefebs/{name}");

        if (GameManager.INSTANCE.ISGAMEIN == true)
        {
            instObject = PhotonNetwork.Instantiate(name, pos, Quaternion.identity);
            instObject.GetComponent<Vegetable>().PhotonInstOffenseVegetable(growthValue, countValue);

        }
        else
        {
            instObject = Instantiate(obj, pos, Quaternion.identity, PoolingZone.transform);
            instObject.GetComponent<Vegetable>().PhotonInstDefenseVegetable(growthValue, countValue);
        }


    }

    public void WellInit(string name, Vector3 pos, float fillValue)
    {
        GameObject obj;
        GameObject instObject;

        obj = Resources.Load<GameObject>($"Prefebs/{name}");

        if (GameManager.INSTANCE.ISGAMEIN == true)
        {
            instObject = PhotonNetwork.Instantiate(name, pos, Quaternion.identity);
            instObject.GetComponent<WellBar>().PhotonOffenseFillWater(fillValue);
        }
        else
        {
            instObject = Instantiate(obj, pos, Quaternion.identity, PoolingZone.transform);
            instObject.GetComponent<WellBar>().PhotonDefenseFillWater(fillValue);
        }
    }

    public void GroundInit()
    {
        for (int i = 0; i < GameObject.Find("fence").transform.childCount; i++)
        {
            fence[i] = GameObject.Find("fence").transform.GetChild(i);
        }

        for (int i = 0; i < GameObject.Find("tree").transform.childCount; i++)
        {
            tree[i] = GameObject.Find("tree").transform.GetChild(i);
        }

        PhotonInstGround(fence, tree, convertGroundState);
    }

    public void PhotonInstGround(Transform[] fence, Transform[] tree, int mapState)
    {
        if (mapState == 4)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].gameObject.SetActive(true);
            tree[0].gameObject.SetActive(false);
        }
        else if (mapState == 3)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].gameObject.SetActive(true);
            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
        }
        else if (mapState == 2)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].transform.position = new Vector3(10, 10, 10);
            fence[3].gameObject.SetActive(true);
            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
            tree[2].gameObject.SetActive(false);
        }
        else if (mapState == 1)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].transform.position = new Vector3(10, 10, 10);
            fence[3].transform.position = new Vector3(10, 10, 10);
            fence[4].gameObject.SetActive(true);

            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
            tree[2].gameObject.SetActive(false);
            tree[3].gameObject.SetActive(false);
        }
        else if (mapState == 0)
        {
            fence[0].transform.position = new Vector3(10, 10, 10);
            fence[1].transform.position = new Vector3(10, 10, 10);
            fence[2].transform.position = new Vector3(10, 10, 10);
            fence[3].transform.position = new Vector3(10, 10, 10);
            fence[4].transform.position = new Vector3(10, 10, 10);
            tree[0].gameObject.SetActive(false);
            tree[1].gameObject.SetActive(false);
            tree[2].gameObject.SetActive(false);
            tree[3].gameObject.SetActive(false);
        }
    }
}


