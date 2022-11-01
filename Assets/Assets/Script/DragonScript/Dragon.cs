using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Dragon : MonoBehaviourPun
{
    public bool testMode;
    // delegate
    public EventReciever dragonEvent = null;

    [Header("[���� ����]")]

    // �巡���� ������ Ÿ�� ������Ʈ�� ������ �¾��.
    [SerializeField] GameObject targetObjectPrefab;
    // �÷��̾� �߰� �� ����� ����ǥ ������
    [SerializeField] GameObject Eff_ExclamationMark;
    // �ǰݽ� ����Ʈ
    [SerializeField] GameObject Eff_Hit;
    // �巡�� ���� ��
    [SerializeField] GameObject Face;
    // �巡�� �ǰݽ� ��
    [SerializeField] GameObject Hit_Face;
    // �� ����
    [SerializeField] GameObject myFruitObj;
    // ���� ����Ʈ1
    [SerializeField] GameObject dieEffectobj;
    //���� ����Ʈ2
    [SerializeField] GameObject dieEffectobj2;
    //body
    [SerializeField] GameObject body;

    // ����ǥ ������Ʈ
    GameObject markObj;
    // �ǰ� ����Ʈ ������Ʈ
    GameObject hitObj;
    // Ÿ�� ������Ʈ
    GameObject newObj;

    // �巡���� �ٶ� �÷��̾�
    PlayerController targetPlayer;
    // �ִϸ��̼�
    Animator myAnimation;

    //target object parent
    GameObject Level = null;

    [Header("[�巡�� ����]")]

    [SerializeField] DragonData dragonData;
    float AttackInterval;// ���� ���ð�
    float AttackRange;// ���� ����
    float TrackingRange = 1.5f; // ���� ����
    float AttackPower;// ���ݷ�
    float curHP; // ���� ü��
    float maxHP;// �ִ� ü��
    float speed; // �ӵ�
    float RandXpos;
    float RandZpos;

    float TargetPlayerToDragon; // �÷��̾�� �巡����� �Ÿ�
    Vector3 PlayerToMove; // �÷��̾�� �巡����� ����

    float TargetObjectToDragon; // ������Ʈ�� �巡����� �Ÿ�
    Vector3 ObjectToMove; // ������Ʈ�� �巡����� ����

    Vector3 newPos = Vector3.zero;



    bool IsDeath { get { return (curHP <= 0); } } // �׾����� üũ


    void initialing()
    {
        AttackInterval = dragonData.AttackInterval;
        AttackRange = dragonData.AttackRange;
        AttackPower = dragonData.AttackPower;
        maxHP = dragonData.HP;
        curHP = maxHP;
        speed = dragonData.Speed;
    }



    private void Awake()
    {
        Level = GameObject.Find("Level");

        initialing();
        //data.AttackPower

        //delegate
        dragonEvent = GetComponent<EventReciever>();


        
            // ü�� �ʱ�ȭ
            curHP = maxHP;

            myAnimation = GetComponent<Animator>();

            //player find
            targetPlayer = FindObjectOfType<PlayerController>();

            // ����Ʈ ã�ƿ���
            markObj = Instantiate(Eff_ExclamationMark, this.transform);
            hitObj = Instantiate(Eff_Hit, this.transform);///////////////////////////////
            Eff_ExclamationMark.SetActive(false);
            Eff_Hit.SetActive(false);
            Face.SetActive(true);
            Hit_Face.SetActive(false);

        if (testMode == false)
        {
            newObj = Instantiate(targetObjectPrefab, new Vector3(this.transform.position.x + RandXpos, 0f, this.transform.position.z + RandZpos), Quaternion.identity, Level.transform);
        }

        GameManager.INSTANCE.AllDragonCount(this.gameObject);


    }








    private void Start()
    {
        
        // �巡���� �����Ǹ� IDLE ���¸� 2������ ���� �� MOVE ���·� �̵�
        StartCoroutine(IDLE_ST());
        
    }



    private void Update()
    {
    }






    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetObject"))
        {
            nextState(STATE.MOVERATE);
        }
    }








    private void OnTriggerExit(Collider other)
    {
        // �ǰ� ����Ʈ ���
        Eff_Hit.GetComponent<ParticleSystem>().Stop();
        Eff_Hit.SetActive(false);

    }



    //--------------------------------------------------------------------------------------------------------
    #region

    // HIT -> ���� ����ȭ
    public void CallDragonTransferDamage(float attackPower)
    {
        if (testMode)
        {
            DragonTransferDamage(attackPower);
        }
        else
        {
            photonView.RPC("DragonTransferDamage", RpcTarget.All, attackPower);
        }
    }

    [PunRPC]
    public void DragonTransferDamage(float attackPower) // �÷��̾��� ������ ������ ü�� ����
    {

        // �̹� �׾����� ����
        if (IsDeath) return;

        curHP -= attackPower;

        if (dragonEvent.callBackDragonHPChangeEvent != null)
            dragonEvent.callBackDragonHPChangeEvent(curHP, maxHP);

        // �ִϸ��̼� ȣ��
        myAnimation.SetTrigger("hit");

        // �ǰ� ����Ʈ ���
        hitObj.SetActive(true);
        hitObj.GetComponent<ParticleSystem>().Play();

        // ü���� �� ������ ��
        if (curHP <= maxHP / 2)
        {
            HitAction_Face();
        }
        // ü���� 0 �� ��
        if (curHP <= 0)
        {
            Debug.Log("�巡�� ����");
            nextState(STATE.DIE);
        }
    }

    #endregion
    //--------------------------------------------------------------------------------------------------------







    void FindObject()
    {
        if (testMode) return;
        // �巡��� ������Ʈ�� �Ÿ� ��� - ���� ����� ���ֱ� ���� ��
        TargetObjectToDragon = Vector3.Distance(transform.position, newObj.transform.position);
        // ������Ʈ�� �� ���� - ������� ȸ������ �־��ֱ� ���� ��
        ObjectToMove = (newObj.transform.position - this.transform.position).normalized;

        if (GameManager.INSTANCE.ISGAMEIN)
        {
            if (photonView.IsMine)
            {
                //Debug.Log("�� ���̰� ������");
                this.GetComponent<Rigidbody>().position += this.transform.forward * speed * Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(ObjectToMove), Time.deltaTime);
            }
            else
            {
                //Debug.Log("�� ���̰� ���� �ƴ�");
            }
            
        }
        else
        {
            //Debug.Log("�� ����");
            this.GetComponent<Rigidbody>().position += this.transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(ObjectToMove), Time.deltaTime);
        }
        
    }


    void FindPlayer()
    {
        if (targetPlayer == null)
        {
            TargetPlayerToDragon = float.MaxValue;
        }
        else
        {
            // �巡��� �÷��̾��� �Ÿ� ��� - ���� ����� ���ֱ� ���� ��
            TargetPlayerToDragon = Vector3.Distance(transform.position, targetPlayer.transform.position);
            // �÷��̾�� �� ���� - ������� ȸ������ �־��ֱ� ���� ��
            PlayerToMove = (targetPlayer.transform.position - this.transform.position).normalized;
        }
    }








    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  Dragon STATE Coroutine
    #region  Dragon STATE Coroutine





    public enum STATE
    {
        NONE, IDLE, MOVE, MOVERATE, FIND, TRKING, ATTACK, DIE
    }





    Coroutine curCoroutine = null;
    // STATE�� ������ �Ҵ��Ͽ� ���
    STATE curState = STATE.NONE;





    // ���ο� STATE�� �Ű������� �޾Ƽ� ���

    void nextState(STATE newState)
    {
        //�޾ƿ� STATE�� ���� STATE�� 
        if (newState == curState)
            return;

        //���� �ڷ�ƾ�� ������ ���� �ڷ�ƾ ����
        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }

        // ���ο� STATE�� ���� STATE�� �Ҵ�ޱ� 
        curState = newState;
        curCoroutine = StartCoroutine(newState.ToString() + "_ST");
    }








    // ������

    IEnumerator IDLE_ST()
    {
        // Ÿ���� ���� ��� ������ �ִٰ�, �̵��ϴٰ� �ݺ��ϵ���
        yield return new WaitForSeconds(2f);

        //�ִϸ��̼� ȣ��
        myAnimation.SetBool("move", false);

        // �����ǰ��� 2�� �Ŀ� �����̵���

        nextState(STATE.MOVE);

    }









    // ����

    IEnumerator MOVERATE_ST()
    {

        //�ִϸ��̼� ȣ��
        myAnimation.SetBool("move", false);

        yield return new WaitForSeconds(1.5f);
        nextState(STATE.MOVE);

    }









    // �̵�

    IEnumerator MOVE_ST()
    {
        this.speed = 0.5f;
        while (true)
        {
            //�ִϸ��̼� ȣ��
            myAnimation.SetBool("move", true);


            // �巡���� �÷��̾ ã�´�.
            FindPlayer();

            // �������� �ܿ� ������ �̵�
            if (TargetPlayerToDragon > TrackingRange)
            {
                // �巡���� ���� ���� �ϳ��� ������ �ִ�.
                FindObject();

            }
            // �������� ���� ������ �߰�
            else if (TargetPlayerToDragon < TrackingRange)
            {
                // �߰߻��·� �Ѿ
                nextState(STATE.FIND);
                break;
            }

            yield return null;

        }
       

    }











    // FIND

    IEnumerator FIND_ST()
    {

        // �߰� �� ���� ��ġ�� �״�� 
        Vector3 myPos = transform.position;

        yield return StartCoroutine(EffectControl());

        // �߰� �� �÷��̾ ���� ���� ���� ��� ����
        if (TargetPlayerToDragon < TrackingRange)
        {
            nextState(STATE.TRKING);
        }
        // �߰� �� �÷��̾ ���� ���� ���� ��� �ٽ� �̵� -> �߰� -> ����
        else
        {
            nextState(STATE.MOVE);
        }

        yield return null;
    }







    // " ! " ����Ʈ 


    Coroutine markMove = null;

    void UnActive_Eff()
    {
        StopCoroutine(markMove);
        markObj.SetActive(false);
    }


    IEnumerator EffectControl()
    {
        // �߰� ����Ʈ Ȱ��ȭ
        markObj.SetActive(true);

        markMove = StartCoroutine(MarkMovement());

        //�ִϸ��̼� ȣ��
        myAnimation.SetBool("move", false);
        myAnimation.SetTrigger("find");

        // 1�� �� ����Ʈ ��Ȱ��ȭ
        yield return new WaitForSeconds(0.8f);
        UnActive_Eff();
    }

    IEnumerator MarkMovement()
    {
        while (true)
        {
            markObj.transform.position = this.transform.position + new Vector3(0, 0.45f, 0);
            yield return null;
        }
    }







    // Dragon Face

    void HitAction_Face()
    {
        Hit_Face.gameObject.SetActive(true);
        Face.gameObject.SetActive(false);
    }












    // TRKING

    IEnumerator TRKING_ST()
    {
        speed = 0.7f;

        while (true)
        {
            myAnimation.SetBool("tracking", true);


            // �����ϴٰ� �÷��̾ ���� ���� ������ �巡���� �ӵ� ����
            FindPlayer();
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (PlayerToMove != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(PlayerToMove), Time.deltaTime * 3f);
            }


            // �÷��̾ ���ݹ��� �ȿ� ���� ���

            if (targetPlayer == null || (TargetPlayerToDragon > TrackingRange))
            {
                /////////////////////////////////////////////////////////
                myAnimation.SetBool("tracking", false);
                nextState(STATE.MOVE);
                break;
            }
            else if (TargetPlayerToDragon <= AttackRange)
            {
                nextState(STATE.ATTACK);
                break;
            }

            yield return null;
        }

    }










    // ATTACK

    IEnumerator ATTACK_ST()
    {

        while (true)
        {
            myAnimation.SetBool("tracking", false);
            myAnimation.SetTrigger("attack");

            FindPlayer();
            if (PlayerToMove != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(PlayerToMove); // ���߿� �ε巴�� ������ �� �ִ��� Ȯ�� 
            }
            // ���� �� �÷��̾�� ������ ����
            targetPlayer.SendMessage("CallPlayerTransferDamage", AttackPower, SendMessageOptions.DontRequireReceiver);

            //targetPlayer.GetComponent<Player>().PlayerTransferDamage(AttackPower);
            // �÷��̾ ���� ���� ��, ���ݹ��� ���� ��
            if ((targetPlayer == null) || (TargetPlayerToDragon > TrackingRange))
            {
                nextState(STATE.MOVE);
                break;
            }
            if (TargetPlayerToDragon > AttackRange && TargetPlayerToDragon < TrackingRange)
            {
                nextState(STATE.TRKING);
                break;
            }
            // �÷��̾ ���� ���϶�


            yield return new WaitForSeconds(AttackInterval);
        }
        //yield return null;


    }



    // DIE

    IEnumerator DIE_ST()
    {
        if (curHP <= 0)
        {
            curHP = 0;

            body.SetActive(false);
            // ���� �ִϸ��̼�
            myAnimation.SetTrigger("die");
            // �װ��� ����Ʈ ����
            //��������Ʈ1
            GameObject dieEff = Instantiate(dieEffectobj, this.transform.position, this.transform.rotation);
            //1�ʵڿ� ������ ����Ʈ����
            yield return new WaitForSeconds(1f);
            //��������Ʈ2
            GameObject dieEff2 = Instantiate(dieEffectobj2, this.transform.position, this.transform.rotation);
            //���ڹݰ���
            GameObject fruitEff = Instantiate(myFruitObj, this.transform.position, this.transform.rotation);
            // �巡�� real ����
            yield return new WaitForSeconds(1f);
            Debug.Log("�ֱ�");
            Destroy(this.gameObject);

        }
        yield return null;
    }







    #endregion Dragon STATE Coroutine
    //
    //---------------------------------------------------------------------------------------------------------------------------------








    // Draw Gizmos

    private void OnDrawGizmos()
    {
        // ���� ���� ����� �׷��ֱ�
        Gizmos.color = new Color32(255, 0, 0, 40);
        Gizmos.DrawSphere(this.transform.position, AttackRange);

        // ���� ���� ����� �׷��ֱ�
        Gizmos.color = new Color32(255, 0, 0, 40);
        Gizmos.DrawSphere(this.transform.position, TrackingRange);
    }





}
