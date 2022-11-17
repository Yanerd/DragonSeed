using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackHouse : MonoBehaviourPun
{
    //house Hp
    [SerializeField] float houseAttackCount = 5;
    //BurnEffect
    GameObject burnEffect;
    //House origin Position
    Vector3 houseOriginPos;
    //House Renderer
    MeshRenderer mRr;
    //House color Change Value
    Color fixedColor = new Color(1f, 1f, 1f);
    bool destroyCheck;

    private void Awake()
    {
        houseOriginPos = this.gameObject.transform.position;
        mRr = GetComponent<MeshRenderer>();
        destroyCheck = false;
    }

    public void CallHouseTransferDamage()
    {
        if (photonView.IsMine) return;

        photonView.RPC("HouseTransferDamage", RpcTarget.All);
    }

    [PunRPC]
    public void HouseTransferDamage()
    {
        if (houseAttackCount == 0&& destroyCheck==false)
        {
            GameManager.INSTANCE.HouseBurn = true;


            destroyCheck = true;
            GameManager.INSTANCE.HOUSEDESTROYCOUNT++;

            PhotonNetwork.Instantiate("O_Burn", this.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            return;
        }

        houseAttackCount--;
        //house shake
        StartCoroutine(ObjectShake(0.5f, 0.01f));
        //color change
        StartCoroutine(ColorChange());
    }

    IEnumerator ObjectShake(float shakeTime, float shakeRange)
    {
        float timer = 0;

        while (timer <= shakeTime)
        {
            this.gameObject.transform.localPosition = Random.insideUnitSphere * shakeRange + houseOriginPos;

            timer += Time.deltaTime;
            
            yield return null;
        }
        this.gameObject.transform.localPosition = houseOriginPos;
    }

    void ChangeColor()
    {
        mRr.material.color -= new Color(0.2f, 0.2f, 0.2f);
    }

    IEnumerator ColorChange()
    {
        fixedColor.r -= 0.2f;
        fixedColor.g -= 0.2f;
        fixedColor.b -= 0.2f;

        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            mRr.material.color = Color.Lerp(mRr.material.color, fixedColor, Time.deltaTime);

            if (timer > 1f) yield break;
            yield return null;
        }
    }



}
