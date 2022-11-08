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
    public List<string> xPos     = new List<string>();
    public List<string> yPos     = new List<string>();
    public List<string> zPos     = new List<string>();
    public List<string> name     = new List<string>();
    public List<string> water    = new List<string>();
    public List<string> plantBar = new List<string>();
    public List<string> wellBar  = new List<string>();
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
    #region Variable
    //Total Object Count
    public int? totalObjectCount;

    //Slider Value
    public int vegetableWaterValue;
    public float vegetableGrowBarValue;
    public float wellBarCount;

    //Object info
    public string objectName;
    public string xpos;
    public string ypos;
    public string zpos;

    //Ground State
    public int convertGroundState;

    //Seed Count
    public int potatoSeedCount   { get; set; }
    public int appleSeedCount    { get; set; }
    public int cabbageSeedCount  { get; set; }
    public int carrotSeedCount   { get; set; }
    public int eggplantSeedCount { get; set; }
    public int HouseCount        { get; set; }
    public int WellCount         { get; set; }

    //Level Object
    public Transform[] fence = new Transform[10];
    public Transform[] tree  = new Transform[20];

    //Jason File Route
    string path     = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Dragon Seed/" + "/Saves/";
    string fileName = "/Save01.json";

    //Inst Object Route 
    private GameObject PoolingZone;
    private GameObject Level;
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Save()
    {
        //clear instance list
        GameManager.INSTANCE.DragonTableClear();

        //save data to json 
        PoolingZone = GameObject.Find("PoolingZone");

        //There's no directory
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        //Bring total ObjectCount
        totalObjectCount = PoolingZone.transform.childCount;

        //divede name
        string[] saveName  = null;
        string[] objectType  = null;

        Data data = new Data();

        #region data Save
        data.groundState       = DefenseUIManager.INSTANCE.MapState.ToString();
        data.potatoSeedCount   = DefenseUIManager.INSTANCE.potatoSeedCount.ToString();
        data.appleSeedCount    = DefenseUIManager.INSTANCE.appleSeedCount.ToString();
        data.cabbageSeedCount  = DefenseUIManager.INSTANCE.cabbageSeedCount.ToString();
        data.carrotSeedCount   = DefenseUIManager.INSTANCE.carrotSeedCount.ToString();
        data.eggplantSeedCount = DefenseUIManager.INSTANCE.eggplantSeedCount.ToString();
        data.houseCount        = DefenseUIManager.INSTANCE.houseCount.ToString();
        data.wellCount         = DefenseUIManager.INSTANCE.wellCount.ToString();
        data.saveListCount     = totalObjectCount == null ? "0" : totalObjectCount.ToString();
        #endregion

        for (int i = 0; i < totalObjectCount; i++)
        {
            this.objectName = PoolingZone.transform.GetChild(i).name;
            xpos = PoolingZone.transform.GetChild(i).position.x.ToString();
            ypos = PoolingZone.transform.GetChild(i).position.y.ToString();
            zpos = PoolingZone.transform.GetChild(i).position.z.ToString();

            saveName = this.objectName.Split('(');
            objectType = this.objectName.Split('_');

            data.name.Add(saveName[0]);
            data.xPos.Add(xpos);
            data.yPos.Add(ypos);
            data.zPos.Add(zpos);

            if (saveName[0] == "B_Well")
            {
                data.wellBar.Add(PoolingZone.transform.GetChild(i).GetComponent<WellBar>().wellBar.value.ToString());
            }
            if (objectType[0] == "V")
            {
                data.plantBar.Add(PoolingZone.transform.GetChild(i).GetComponent<Vegetable>().GrowthValue.ToString());
                data.water.Add(PoolingZone.transform.GetChild(i).GetComponent<Vegetable>().CountValue.ToString());
            }
        }
        File.WriteAllText(path + fileName, Encryption.Encrypt((JsonUtility.ToJson(data)), "key"));
    }

    public void Load()
    {
        //총 오브젝트의 숫자
        int ObjCount;
        //오브젝트의 위치를 저장할 변수
        float convertX;
        float convertY;
        float convertZ;
        //data2(식물 슬라이더 밸류 리스트)의 인덱스 변수
        int vegetableCount = 0;
        int wellCount = 0;
        //data2의 담긴 name을 구분 할 변수
        string[] whatYourName = null;
        //설치 될 오브젝트의 위치를 담을 변수
        Vector3 vector3 = Vector3.zero;

        string parse = File.ReadAllText(path + fileName);
        File.WriteAllText(path + "/Read.json", Encryption.Decrypt(parse, "key"));

        Data data2 = JsonUtility.FromJson<Data>(File.ReadAllText(path + "/Read.json"));

        ObjCount           = int.Parse(data2.saveListCount     == "" ? "0" : data2.saveListCount);
        potatoSeedCount    = int.Parse(data2.potatoSeedCount   == "" ? "0" : data2.potatoSeedCount);
        appleSeedCount     = int.Parse(data2.appleSeedCount    == "" ? "0" : data2.appleSeedCount);
        cabbageSeedCount   = int.Parse(data2.cabbageSeedCount  == "" ? "0" : data2.cabbageSeedCount);
        carrotSeedCount    = int.Parse(data2.carrotSeedCount   == "" ? "0" : data2.carrotSeedCount);
        eggplantSeedCount  = int.Parse(data2.eggplantSeedCount == "" ? "0" : data2.eggplantSeedCount);
        HouseCount         = int.Parse(data2.houseCount        == "" ? "0" : data2.houseCount);
        WellCount          = int.Parse(data2.wellCount         == "" ? "0" : data2.wellCount);
        convertGroundState = int.Parse(data2.groundState       == "" ? "0" : data2.groundState);

        for (int i = 0; i < ObjCount; i++)
        {
            convertX = float.Parse(data2.xPos[i]);
            convertY = float.Parse(data2.yPos[i]);
            convertZ = float.Parse(data2.zPos[i]);

            vector3 = new Vector3(convertX, convertY, convertZ);

            whatYourName = data2.name[i].Split('_');

            if (whatYourName[0]=="V")
            {
                vegetableGrowBarValue = float.Parse(data2.plantBar[vegetableCount] == "" ? "0" : data2.plantBar[vegetableCount]);
                vegetableWaterValue = int.Parse(data2.water[vegetableCount] == "" ? "0" : data2.water[vegetableCount]);
                VegetableInst(data2.name[i], vector3);
                vegetableCount++;
            }
            else if (whatYourName[0] == "B")
            {
                if (data2.name[i] == "B_Well")
                {
                    wellBarCount = float.Parse(data2.wellBar[wellCount] == "" ? "0" : data2.wellBar[wellCount]);
                    WellInst(data2.name[i], vector3);
                    wellCount++;
                }
                else
                    Inst(data2.name[i], vector3);
            }
            else
                Inst(data2.name[i], vector3);
        }
        GroundInst();
        
    }

    PhotonView pun;

    public void InitLoad()
    {
        // There's no directory
        if (Directory.Exists(path) == true)
        {
            Load();
        }
    }

    public void Inst(string name, Vector3 pos)
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

    public void VegetableInst(string name, Vector3 pos)
    {
        GameObject obj= Resources.Load<GameObject>(name);
        GameObject instObject=null;

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

    public void WellInst(string name, Vector3 pos)
    {
        GameObject obj= Resources.Load<GameObject>(name); ;
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
    
    public void GroundInst()
    {
        Vector3 fencePos = new Vector3(0.06f, 4.1f, -0.16f);
        Vector3 treePos  = new Vector3(0.06f, 4.3f, -0.16f);
        GameObject fence = Resources.Load<GameObject>("L_Fence");
        GameObject tree  = Resources.Load<GameObject>("L_Tree");
        GameObject instFence = null;
        GameObject instTree  = null;

        DefenseUIManager.INSTANCE.MapState = convertGroundState;

        if (GameManager.INSTANCE.ISGAMEIN == true)
        {
            instFence = PhotonNetwork.Instantiate("L_Fence", fencePos, Quaternion.identity);
            instTree = PhotonNetwork.Instantiate("L_Tree", fencePos, Quaternion.identity);
        }
        else
        {
            Level = GameObject.Find("Level");
            instFence = Instantiate(fence, fencePos, Quaternion.identity, Level.transform);
            instTree  = Instantiate(tree, treePos, Quaternion.identity, Level.transform);
        }

        for (int i = 0; i < instFence.transform.childCount; i++)
        {
            this.fence[i] = instFence.transform.GetChild(i);
        }

        for (int i = 0; i < instTree.transform.childCount; i++)
        {
            this.tree[i] = instTree.transform.GetChild(i);
        }


        InstGround(this.fence, this.tree, convertGroundState);
    }

    public void InstGround(Transform[] fence, Transform[] tree, int mapState)
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


