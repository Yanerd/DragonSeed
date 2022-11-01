using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetObject : MonoBehaviour
{
    // 드래곤의 위치를 받아오기위해 선언
    Dragon dragon;

    // 벡터 위치 랜덤으로 받을 값
    float RandXpos;
    float RandZpos;

    //timer
    float timer = 0f;

    // 벡터 위치 초기화
    Vector3 newPos = Vector3.zero;



    private void Awake()
    {
        dragon = FindObjectOfType<Dragon>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 드래곤이 일정시간동안 오브젝트를 찾지 못했을 때 재생성
        if (timer > 4f)
        {
            timer = 0f;
            CreatNewPosition();
        }
    }

    // 새로운 위치값을 할당
    private void CreatNewPosition()
    {
        if (dragon==null) { Destroy(this.gameObject); }

        // 랜덤한 위치값을 계속 준다
        RandXpos = Random.Range(-2.5f, 3.5f);
        RandZpos = Random.Range(-2.5f, 3.5f);

        // 드래곤의 포지션
        if (dragon != null)
        {
            Vector3 dragonPos = dragon.transform.position;

            // 오브젝트의 새로운 랜덤 생성 위치값
            Vector3 RandPos = new Vector3(RandXpos, 0f, RandZpos);
            newPos = dragonPos + RandPos;
        }

        // 오브젝트의 위치는 새로운 위치
        transform.position = newPos;

    }

    // 생성자에서 체크하고 재배치
    private void OnTriggerEnter(Collider other)
    {
        // 드래곤과 충돌할 때마다 새로운 위치에 생성
        if (other.CompareTag("Dragon"))
        {
            timer = 0f;
            CreatNewPosition();
        }
    }

    // 기즈모 그려주기
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }



}
