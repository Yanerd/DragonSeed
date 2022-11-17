using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackHouse : MonoBehaviourPun
{
    Vector3 houseOriginPos;
    bool shakeCheck;
    Renderer Rr;
    int houseAttackCount;

    private void Awake()
    {
        if(!photonView.IsMine)
        {
            houseOriginPos = this.gameObject.transform.position;
            shakeCheck = false;
            Rr = GetComponent<Renderer>();
            houseAttackCount = 5;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (houseAttackCount == 0) return;

        if(collision.collider.tag=="Weapon"&& !photonView.IsMine)
        {
            if(shakeCheck==false)
            {
                houseAttackCount--;
                StartCoroutine(ObjectShake(0.5f,0.01f));
                
            }
        }


    }

    IEnumerator ObjectShake(float shakeTime, float shakeRange)
    {
        float timer = 0;
        shakeCheck = true;

        while (timer <= shakeTime)
        {
            this.gameObject.transform.localPosition = Random.insideUnitSphere * shakeRange + houseOriginPos;

            timer += Time.deltaTime;
            
            yield return null;
        }
        this.gameObject.transform.localPosition = houseOriginPos;
    }


}
