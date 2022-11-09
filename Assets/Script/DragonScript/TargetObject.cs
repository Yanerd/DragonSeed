using UnityEngine;

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
        if (timer > 10f)
        {
            timer = 0f;
            CreatNewPosition();
        }
    }

    // ���ο� ��ġ���� �Ҵ�
    private void CreatNewPosition()
    {
        int centerpos = 10 - DefenseUIManager.INSTANCE.MapState;
        if (dragon == null) { Destroy(this.gameObject); }

        // ������ ��ġ���� ��� �ش�
        RandXpos = Random.Range(-(float)centerpos / 2f, (float)centerpos / 2f);
        RandZpos = Random.Range(-(float)centerpos / 2f, (float)centerpos / 2f);

        // �巡���� ������
        if (dragon != null)
        {
            Vector3 dragonPos = dragon.transform.position;

            // ������Ʈ�� ���ο� ���� ���� ��ġ��
            Vector3 RandPos = new Vector3(RandXpos, 0f, RandZpos);
            newPos = RandPos + new Vector3(centerpos / 2, 0.33f, centerpos / 2);
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

        if (other.CompareTag("well") || other.CompareTag("Building"))
        {
            timer = 0f;
            CreatNewPosition();
            Debug.Log("�浹�n");
        }
    }

    // ����� �׷��ֱ�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }



}
