using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenceSkill : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name== "O_Farmer_Upgrade(Clone)")
        {
            Debug.Log("Ʈ����");
            other.gameObject.GetComponent<PlayerController>().CallPlayerTransferDamage(10f);
        }
        
    }
  
}
