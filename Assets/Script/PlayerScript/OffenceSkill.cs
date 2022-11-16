using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenceSkill : MonoBehaviour
{
    [Header("[skill damage]")]
    [SerializeField] float skillDamage = 100f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name== "O_Farmer_Upgrade(Clone)")
        {
            Debug.Log("Æ®¸®°Å");
            other.gameObject.GetComponent<PlayerController>().CallPlayerTransferDamage(skillDamage);
        }
        
    }
  
}
