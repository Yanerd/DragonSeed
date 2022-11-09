using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WellBar : MonoBehaviour
{
    
    [SerializeField] Transform water;

    [SerializeField] public float FillValue;
    [SerializeField] float lerpSensive = 0.3f;


    [SerializeField] public Slider wellBar;
    [SerializeField] Canvas canvas;

    bool instCheck=false;
    bool WaterCheck = false;

    private void Awake()
    {
        wellBar.minValue = 0;
        wellBar.value = 0;

        if (GameManager.INSTANCE.ISGAMEIN)
        {
            InstOffenseWell(SaveLoadManager.INSTANCE.wellBarCount);
            
        }
        else
        {
            Debug.Log(SaveLoadManager.INSTANCE.wellBarCount.ToString());
            InstGardeningWell(SaveLoadManager.INSTANCE.wellBarCount);
        }


    }
    private void Start()
    {
        if (GameManager.INSTANCE.ISGAMEIN)
        {
            wellBar.gameObject.SetActive(false);
        }
        DefenseUIManager.INSTANCE.SliderBarList.Add(wellBar);

        //wellBar.value = FillValue * 0.25f;
    }

    public void DisapearWaterCount()
    {
        if (wellBar.value <= 0) return;
        
        wellBar.value -= 0.25f;
        water.transform.Translate(0, -0.056f, 0);

        if (instCheck) return;

        if (WaterCheck) return;
        StartCoroutine(FillWater());
    }

    void InstGardeningWell(float fillValue)
    {
        this.FillValue = fillValue;
        wellBar.value  = fillValue;
        instCheck = true;

        float pos=0f;
        pos =fillValue*0.056f;

        water.transform.position=new Vector3 (water.transform.position.x, transform.position.y+pos, water.transform.position.z);
        
        StartCoroutine(FillWater());
    }

    void InstOffenseWell(float FillValue)
    {
        float pos = 0f;
        pos = FillValue * 0.056f;

        wellBar.gameObject.SetActive(false);
        water.transform.position = new Vector3(water.transform.position.x, transform.position.y + pos, water.transform.position.z);
      
    }

    IEnumerator FillWater()
    {
        while (true)
        {
            WaterCheck = true;
            if (wellBar.value >= 0.97f)
            {
                WaterCheck = false;
                instCheck = false;
                wellBar.value = 1;
                water.transform.position = new Vector3(water.transform.position.x, transform.position.y+ 0.225f, water.transform.position.z);
                yield break;
            }

            yield return new WaitForSeconds(Time.deltaTime * lerpSensive);
            wellBar.value = Mathf.Lerp(wellBar.value, 1f, Time.deltaTime * lerpSensive);
            water.transform.position = Vector3.Lerp(water.transform.position, new Vector3(water.transform.position.x, transform.position.y + 0.225f, water.transform.position.z), lerpSensive * Time.deltaTime);
        }
    }




}
