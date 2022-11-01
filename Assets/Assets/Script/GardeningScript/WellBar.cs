using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WellBar : MonoBehaviour
{
    [SerializeField] Slider wellBar=null;
    [SerializeField] Transform water;

    [SerializeField] public float FillValue;
    [SerializeField] float lerpSensive = 0.3f;


    Camera myCamera;
    public Slider fillIdex;

    bool instCheck=false;
    bool WaterCheck = false;

    private void Awake()
    {
        myCamera = Camera.main;
        Vector3 sliderPos = myCamera.WorldToScreenPoint(this.transform.position + new Vector3(0, 0.7f, 0));
        fillIdex = Instantiate(wellBar, sliderPos, Quaternion.identity, GameObject.Find("DefenseUI").transform);
        DefenseUIManager.INSTANCE.SliderBarList.Add(fillIdex);

        fillIdex.value = FillValue * 0.25f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
           // PhotonFillWater(FillValue);
        }
    }


    public void DisapearWaterCount()
    {
        if (fillIdex.value <= 0) return;
        
        fillIdex.value -= 0.25f;
        water.transform.Translate(0, -0.056f, 0);

        if (instCheck) return;

        if (WaterCheck) return;
        StartCoroutine(FillWater());
    }

    public void PhotonDefenseFillWater(float FillValue)
    {
        fillIdex.value = FillValue * 0.25f;
        instCheck = true;

        fillIdex.value = FillValue*0.25f;
        float pos=0f;
        pos =FillValue*0.056f;

        water.transform.position=new Vector3 (water.transform.position.x, transform.position.y+pos, water.transform.position.z);
        
        StartCoroutine(FillWater());
    }

    public void PhotonOffenseFillWater(float FillValue)
    {
        fillIdex.value = FillValue * 0.25f;
        instCheck = true;

        fillIdex.value = FillValue * 0.25f;
        float pos = 0f;
        pos = FillValue * 0.056f;
        Debug.Log(FillValue);
        fillIdex.gameObject.SetActive(false);
        water.transform.position = new Vector3(water.transform.position.x, transform.position.y + pos, water.transform.position.z);
      
    }

    IEnumerator FillWater()
    {
        while (true)
        {
            WaterCheck = true;
            if (fillIdex.value >= 0.97f)
            {
                WaterCheck = false;
                instCheck = false;
                fillIdex.value = 1;
                water.transform.position = new Vector3(water.transform.position.x, transform.position.y+ 0.225f, water.transform.position.z);
                yield break;
            }

            yield return new WaitForSeconds(Time.deltaTime * lerpSensive);

            fillIdex.value = Mathf.Lerp(fillIdex.value, 1f, Time.deltaTime * lerpSensive);
            water.transform.position = Vector3.Lerp(water.transform.position, new Vector3(water.transform.position.x, transform.position.y + 0.225f, water.transform.position.z), lerpSensive * Time.deltaTime);
        }
    }




}
