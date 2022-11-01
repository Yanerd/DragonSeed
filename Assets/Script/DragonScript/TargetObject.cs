using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetObject : MonoBehaviour
{
    // �巡���� ��ġ�� �޾ƿ������� ����
    Dragon dragon;

    // ���� ��ġ �������� ���� ��
    float RandXpos;
    float RandZpos;

    //timer
    float timer = 0f;

    // ���� ��ġ �ʱ�ȭ
    Vector3 newPos = Vector3.zero;



    private void Awake()
    {
        dragon = FindObjectOfType<Dragon>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // �巡���� �����ð����� ������Ʈ�� ã�� ������ �� �����
        if (timer > 4f)
        {
            timer = 0f;
            CreatNewPosition();
        }
    }

    // ���ο� ��ġ���� �Ҵ�
    private void CreatNewPosition()
    {
        if (dragon==null) { Destroy(this.gameObject); }

        // ������ ��ġ���� ��� �ش�
        RandXpos = Random.Range(-2.5f, 3.5f);
        RandZpos = Random.Range(-2.5f, 3.5f);

        // �巡���� ������
        if (dragon != null)
        {
            Vector3 dragonPos = dragon.transform.position;

            // ������Ʈ�� ���ο� ���� ���� ��ġ��
            Vector3 RandPos = new Vector3(RandXpos, 0f, RandZpos);
            newPos = dragonPos + RandPos;
        }

        // ������Ʈ�� ��ġ�� ���ο� ��ġ
        transform.position = newPos;

    }

    // �����ڿ��� üũ�ϰ� ���ġ
    private void OnTriggerEnter(Collider other)
    {
        // �巡��� �浹�� ������ ���ο� ��ġ�� ����
        if (other.CompareTag("Dragon"))
        {
            timer = 0f;
            CreatNewPosition();
        }
    }

    // ����� �׷��ֱ�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }



}
