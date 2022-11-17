using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFruit : MonoBehaviour
{
    [SerializeField] float DropSpeed=-2f;

    private void OnEnable()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        
    }


    public  void dropObject()
    {
        StartCoroutine(drop());
    }

    IEnumerator drop()
    {
        yield return new WaitForSeconds(2f);
        while(true)
        {
            if (gameObject.transform.position.y <= -4f)
            {
                yield break;
            }
            transform.Translate(0, DropSpeed * Time.deltaTime, 0,Space.World);
            yield return new WaitForSeconds(0.01f);
        }

    }
   

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="ground"|| collision.gameObject.tag == "OnGround")
        {
            gameObject.SetActive(false);
        }
    }

}
