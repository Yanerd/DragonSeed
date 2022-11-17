    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //[SerializeField] weaponScriptableObj;

    PlayerController playerController;

    private float totalDamage = 0f;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();

        totalDamage = playerController.PlayerAttackPower; //+ weaponScriptableObj.damage;
    }

    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon"))
        {
            Time.timeScale = 0.35f;
            Invoke("TimeBack",0.1f);
            other.SendMessage("CallDragonTransferDamage", totalDamage, SendMessageOptions.DontRequireReceiver);
        }
        if(other.gameObject.name== "B_House(Clone)")
        {
            other.SendMessage("CallHouseTransferDamage", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void TimeBack()
    {
        Time.timeScale = 1f;
    }
}
