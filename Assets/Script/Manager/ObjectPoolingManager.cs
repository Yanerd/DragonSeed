using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPoolingManager : MonoBehaviour
{
    #region SingleTon

    private static ObjectPoolingManager instance = null;
    public static ObjectPoolingManager inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPoolingManager>();
                if (instance == null)
                {
                    instance = new GameObject("ObjectPoolingManager").AddComponent<ObjectPoolingManager>();
                }
            }
            return instance;
        }
    }

    #endregion

    [SerializeField] public List<GameObject> potatoDragonList   = new List<GameObject>();
    [SerializeField] public List<GameObject> appleDragonList    = new List<GameObject>();
    [SerializeField] public List<GameObject> cabbageDragonList  = new List<GameObject>();
    [SerializeField] public List<GameObject> carrotDragonList   = new List<GameObject>();
    [SerializeField] public List<GameObject> eggplantDragonList = new List<GameObject>();
   
    [SerializeField] public Transform PoolingZone;

    private List<GameObject> InstAlphaObjects = new List<GameObject>();//Save Alpha PrefabList

    private Dictionary<string, List<GameObject>> instList = new Dictionary<string, List<GameObject>>();
    //Call instantiate
    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation,Transform parent)
    {
        List<GameObject> list = null;
        GameObject instance = null;

        bool listCheck = instList.TryGetValue(prefab.name, out list);
        if (listCheck==false)
        {
            list = new List<GameObject>();
            instList.Add(prefab.name, list);
        }
        if (list.Count == 0)
        {
            instance = GameObject.Instantiate(prefab, position, rotation, parent);
        }
        else if (list.Count > 0)
        {
            instance = list[0];
            instance.transform.position = position + new Vector3(0, 0.3f, 0);
            instance.transform.rotation = rotation;
            list.RemoveAt(0);
        }

        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            return null;
        }
    }
    //Call bringObject & Pooling
    public void Destroy(GameObject Prefab)
    {
        List<GameObject> list = null;
        string prefabId = Prefab.name.Replace("(Clone)", "");
        bool listCached = instList.TryGetValue(prefabId, out list);
        if (listCached==false)
        {
            Debug.LogError("Not Found " + Prefab.name);
            return;
        }

        Prefab.transform.position = PoolingZone.position;
        Prefab.SetActive(false);
        list.Add(Prefab);

    }

    //BuldingMode Pooling
    public void inst_AlphaPrefab(GameObject[] prefab)
    {
        for (int i = 0; i < prefab.Length; i++)
        {
            GameObject instObject = Instantiate(prefab[i], PoolingZone.position, Quaternion.identity,GameObject.Find("AlphaPooling").transform);
            InstAlphaObjects.Add(instObject);
            //InstAlphaObjects[i].SetActive(false);
        }
    }

    public void Objectapperear(Vector3 tr)
    {
        if(DefenseUIManager.INSTANCE.onHOUSE == true)
        {
            InstAlphaObjects[5].transform.position = tr + new Vector3(0, 0.3f, 0);
            //InstAlphaObjects[5].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.onWELL == true)
        {
            InstAlphaObjects[6].transform.position = tr + new Vector3(0, 0.3f, 0);
            //InstAlphaObjects[0].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.onPOTATO == true)
        {
            InstAlphaObjects[0].transform.position = tr + new Vector3(0, 0.3f, 0);
            //InstAlphaObjects[0].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.onAPPLE ==true)
        {
            InstAlphaObjects[1].transform.position = tr + new Vector3(0, 0.4f, 0);
            //InstAlphaObjects[0].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.onCABBAGE == true)
        {
            InstAlphaObjects[2].transform.position = tr + new Vector3(0, 0.4f, 0);
            //InstAlphaObjects[0].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.onCARROT == true)
        {
            InstAlphaObjects[3].transform.position = tr + new Vector3(0, 0.4f, 0);
            //InstAlphaObjects[0].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.onEEGPLANT == true)
        {
            InstAlphaObjects[4].transform.position = tr + new Vector3(0, 0.5f, 0);
            //InstAlphaObjects[6].SetActive(true);
        }
        else if (DefenseUIManager.INSTANCE.WATERRAY == true)
        {
            InstAlphaObjects[7].transform.position = tr + new Vector3(0, 0.3f, 0);

        }
        else if (DefenseUIManager.INSTANCE.onWATER == true)
        {
            InstAlphaObjects[8].transform.position = tr + new Vector3(0, 0.3f, 0);

        }
    }
    public void ObjectDisappear()
    {
        
        if (DefenseUIManager.INSTANCE.onHOUSE == false)
        {
            InstAlphaObjects[5].transform.position = PoolingZone.position;
            //InstAlphaObjects[5].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.onPOTATO == false)
        {
            InstAlphaObjects[0].transform.position = PoolingZone.position;
            //InstAlphaObjects[0].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.onWELL == false)
        {
            InstAlphaObjects[6].transform.position = PoolingZone.position;
           //InstAlphaObjects[6].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.onAPPLE == false)
        {
            InstAlphaObjects[1].transform.position = PoolingZone.position;
            //InstAlphaObjects[6].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.onCABBAGE == false)
        {
            InstAlphaObjects[2].transform.position = PoolingZone.position;
            //InstAlphaObjects[6].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.onCARROT == false)
        {
            InstAlphaObjects[3].transform.position = PoolingZone.position;
            //InstAlphaObjects[6].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.onEEGPLANT == false)
        {
            InstAlphaObjects[4].transform.position = PoolingZone.position;
            //InstAlphaObjects[6].SetActive(false);
        }
        if (DefenseUIManager.INSTANCE.WATERRAY == false)
        {
            InstAlphaObjects[7].transform.position = PoolingZone.position;

        }

    }
}
