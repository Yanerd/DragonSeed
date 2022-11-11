using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class Ground : MonoBehaviour
{
    MeshRenderer mr;
    public bool OnBuilding;
    public Material Floor;
    [SerializeField] bool OnAlpha=false;
    [SerializeField] public bool OnWater = false;

    int mask;

    DefenseUIManager B;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        B = FindObjectOfType<DefenseUIManager>();
        mask = 1 << 9 | 1 << 10|1<<6|1<<8|1<<7;
    }
    void Update()
    {

        if (DefenseUIManager.INSTANCE.BUILDINGMODE == true)
        {
            Camera.main.cullingMask = mask;
            if (OnBuilding == true)
            {
                mr.materials[0].color = Color.red;
            }
            else if (OnAlpha == true)
            {
                mr.materials[0].color = Color.blue; 
            }
            else if (OnAlpha == false)
            {
                mr.materials[0].color = Color.white;
            }
            
        }
        if (DefenseUIManager.INSTANCE.BUILDINGMODE == false)
        {
            if (OnBuilding == true || OnAlpha == true)
            {
                 Camera.main.cullingMask = -1;
                mr.materials[0].color = Color.white;
            }
            if(OnWater==true)
            {
                if (OnWater == false)
                {
                    mr.materials[0].color = Color.white;
                    return;
                }
                    
                mr.materials[0].color = Color.cyan;
            }
            else if (OnWater == false)
            {
                mr.materials[0].color = Color.white;
            }

        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Building")
        {
            this.gameObject.tag = "OnGround";
            OnBuilding = true;
        }
        if (collision.collider.tag == "plant")
        {
            this.gameObject.tag = "OnGround";
            OnBuilding = true;
        }
        if (collision.collider.tag == "well")
        {
            this.gameObject.tag = "OnGround";
            OnBuilding = true;
        }
       
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Building")
        {
            this.gameObject.tag = "GroundEmpty";
            OnBuilding = false;
        }
        if (collision.collider.tag == "plant")
        {
            this.gameObject.tag = "GroundEmpty";
            OnBuilding = false;
        }
        if (collision.collider.tag == "well")
        {
            this.gameObject.tag = "GroundEmpty";
            OnBuilding = false;
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.tag == "Alpha")
        {
            this.gameObject.tag = "Alpha";
            OnAlpha = true;
        }
        if (other.tag == "WaterRay")
        {
            OnWater = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
       
        if (other.tag == "Alpha")
        {
            this.gameObject.tag = "GroundEmpty";
            OnAlpha = false;
        }
        if (other.tag == "WaterRay")
        {
            OnWater = false;
        }

    }


}
