using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Dragon : MonoBehaviourPun
{
    public bool testMode;
    // delegate
    public EventReciever dragonEvent = null;

    [Header("[참조 정보]")]

    // 드래곤은 각자의 타겟 오브젝트를 가지고 태어난다.
    [SerializeField] GameObject targetObjectPrefab;
    // 플레이어 발견 시 사용할 느낌표 아이콘
    [SerializeField] GameObject Eff_ExclamationMark;
    // 피격시 이펙트
    [SerializeField] GameObject Eff_Hit;
    // 드래곤 원래 얼굴
    [SerializeField] GameObject Face;
    // 드래곤 피격시 얼굴
    [SerializeField] GameObject Hit_Face;
    // 내 과일
    [SerializeField] GameObject myFruitObj;
    // 죽음 이펙트1
    [SerializeField] GameObject dieEffectobj;
    //죽음 이펙트2
    [SerializeField] GameObject dieEffectobj2;
    //body
    [SerializeField] GameObject body;

    // 느낌표 오브젝트
    GameObject markObj;
    // 피격 이펙트 오브젝트
    GameObject hitObj;
    // 타겟 오브젝트
    GameObject newObj;

    // 드래곤이 바라볼 플레이어
    PlayerController targetPlayer;
    // 애니메이션
    Animator myAnimation;

    //target object parent
    GameObject Level = null;

    [Header("[드래곤 정보]")]

    [SerializeField] DragonData dragonData;
    float AttackInterval;// 공격 대기시간
    float AttackRange;// 공격 범위
    float TrackingRange = 1.5f; // 추적 범위
    float AttackPower;// 공격력
    float curHP; // 현재 체력
    float maxHP;// 최대 체력
    float speed; // 속도
    float RandXpos;
    float RandZpos;

    float TargetPlayerToDragon; // 플레이어와 드래곤과의 거리
    Vector3 PlayerToMove; // 플레이어와 드래곤과의 방향

    float TargetObjectToDragon; // 오브젝트와 드래곤과의 거리
    Vector3 ObjectToMove; // 오브젝트와 드래곤과의 방향

    Vector3 newPos = Vector3.zero;



    bool IsDeath { get { return (curHP <= 0); } } // 죽었는지 체크

    IEnumerator enumerator;

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



        // 체력 초기화
        curHP = maxHP;

        myAnimation = GetComponent<Animator>();

        enumerator = FindPlayerObject();

        //player find
        StartCoroutine(enumerator);

        // 이펙트 찾아오기
        markObj = Instantiate(Eff_ExclamationMark, this.transform);
        hitObj = Instantiate(Eff_Hit, this.transform);///////////////////////////////
        Eff_ExclamationMark.SetActive(false);
        Eff_Hit.SetActive(false);
        Face.SetActive(true);
        Hit_Face.SetActive(false);

        if (testMode == false)
        {
            newObj = Instantiate(targetObjectPrefab, new Vector3(this.transform.position.x + RandXpos, 0.33f, this.transform.position.z + RandZpos), Quaternion.identity, Level.transform);
        }

        GameManager.INSTANCE.AllDragonCount(this.gameObject);
        
        if (!photonView.IsMine&&GameManager.INSTANCE.SCENENUM==2)
        {
            GameManager.INSTANCE.dragons.Add(this.gameObject);
            Debug.Log(this.gameObject.name);
        }
    }

    IEnumerator FindPlayerObject()
    {
        while (true)
        {
            targetPlayer = FindObjectOfType<PlayerController>();
            //Debug.Log("졸라찾기");
            if (targetPlayer != null)
            {
                if (targetPlayer.name == "O_Farmer(Clone)")
                {
                    //Debug.Log($"졸라 찾았음{targetPlayer.name}");

                    StopCoroutine(enumerator);
                    yield break;
                }
            }
            yield return null;
        }
    }


    private void Start()
    {
        // 드래곤이 생성되면 IDLE 상태를 2초정도 지속 후 MOVE 상태로 이동
        StartCoroutine(IDLE_ST());

    }



    private void Update()
    {
    }
    private void OnDestroy()
    {
        GameManager.INSTANCE.dragons.Clear();

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
        // 피격 이펙트 출력
        Eff_Hit.GetComponent<ParticleSystem>().Stop();
        Eff_Hit.SetActive(false);

    }



    //--------------------------------------------------------------------------------------------------------
    #region

    // HIT -> 포톤 동기화
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
    public void DragonTransferDamage(float attackPower) // 플레이어의 공격이 들어오면 체력 감소
    {
        Debug.Log(attackPower.ToString());
        // 이미 죽었으면 리턴
        if (IsDeath) return;

        curHP -= attackPower;

        if (dragonEvent.callBackDragonHPChangeEvent != null)
            dragonEvent.callBackDragonHPChangeEvent(curHP, maxHP);

        // 애니메이션 호출
        myAnimation.SetTrigger("hit");

        // 피격 이펙트 출력
        hitObj.SetActive(true);
        hitObj.GetComponent<ParticleSystem>().Play();

        // 체력이 반 이하일 때
        if (curHP <= maxHP / 2)
        {
            HitAction_Face();
        }
        // 체력이 0 일 때
        if (curHP <= 0)
        {
            Debug.Log("드래곤 죽음");
            nextState(STATE.DIE);
        }
    }

    #endregion
    //--------------------------------------------------------------------------------------------------------







    void FindObject()
    {
        if (testMode) return;
        // 드래곤과 오브젝트의 거리 재기 - 범위 계산을 해주기 위한 값
        TargetObjectToDragon = Vector3.Distance(transform.position, newObj.transform.position);
        // 오브젝트로 갈 방향 - 룩앳으로 회전값을 넣어주기 위한 값
        ObjectToMove = (newObj.transform.position - this.transform.position).normalized;

        if (GameManager.INSTANCE.ISGAMEIN)
        {
            if (photonView.IsMine)
            {
                //Debug.Log("방 안이고 내꺼임");
                this.GetComponent<Rigidbody>().position += this.transform.forward * speed * Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(ObjectToMove), Time.deltaTime * 4);
            }
            else
            {
                //Debug.Log("방 안이고 내꺼 아님");
            }

        }
        else
        {
            //Debug.Log("방 밖임");
            this.GetComponent<Rigidbody>().position += this.transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(ObjectToMove), Time.deltaTime * 4);
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
            // 드래곤과 플레이어의 거리 재기 - 범위 계산을 해주기 위한 값
            TargetPlayerToDragon = Vector3.Distance(transform.position, targetPlayer.transform.position);
            // 플레이어로 갈 방향 - 룩앳으로 회전값을 넣어주기 위한 값
            PlayerToMove = (targetPlayer.transform.position - this.transform.position).normalized;
        }
    }








    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  Dragon STATE Coroutine
    #region  Dragon STATE Coroutine





    public enum STATE
    {
        NONE, IDLE, MOVE, MOVERATE, FIND, TRKING, ATTACK, DIE, RANDOMMOTION
    }





    Coroutine curCoroutine = null;
    // STATE를 변수에 할당하여 사용
    STATE curState = STATE.NONE;





    // 새로운 STATE를 매개변수로 받아서 계산

    void nextState(STATE newState)
    {
        //받아온 STATE를 현재 STATE로 
        if (newState == curState)
            return;

        //현재 코루틴이 있을때 현재 코루틴 중지
        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }

        // 새로운 STATE를 현재 STATE에 할당받기 
        curState = newState;
        curCoroutine = StartCoroutine(newState.ToString() + "_ST");
    }








    // 숨쉬기

    IEnumerator IDLE_ST()
    {
        // 타겟이 없을 경우 가만히 있다가, 이동하다가 반복하도록
        yield return new WaitForSeconds(2f);

        //애니메이션 호출
        myAnimation.SetBool("move", false);

        // 생성되고나서 2초 후에 움직이도록

        nextState(STATE.MOVE);

    }









    //멈춤 - 이거 그거임 타겟 먹으면 멈추는거

    IEnumerator MOVERATE_ST()
    {

        //애니메이션 호출
        myAnimation.SetBool("move", false);

        yield return new WaitForSeconds(0f);
        nextState(STATE.RANDOMMOTION);
    }

    IEnumerator RANDOMMOTION_ST()
    {
        int randMotion = Random.Range(0, 3);
        switch (randMotion)
        {
            case 0:
                myAnimation.SetTrigger("Eat");
                Debug.Log("먹어!");
                break;
            case 1:
                myAnimation.SetTrigger("Dead");
                Debug.Log("죽어!");
                break;
            case 2:
                myAnimation.SetTrigger("Roar");
                Debug.Log("짖어!");
                break;
        }
        yield return new WaitForSeconds(4f);
        nextState(STATE.MOVE);
    }







    // 이동

    IEnumerator MOVE_ST()
    {
        this.speed = 0.5f;
        while (true)
        {
            //애니메이션 호출
            myAnimation.SetBool("move", true);


            // 드래곤은 플레이어를 찾는다.
            FindPlayer();

            // 추적범위 외에 있으면 이동
            if (TargetPlayerToDragon > TrackingRange)
            {
                // 드래곤은 각자 공을 하나씩 가지고 있다.
                FindObject();

            }
            // 추적범위 내에 있으면 발견
            else if (TargetPlayerToDragon < TrackingRange)
            {
                // 발견상태로 넘어감
                nextState(STATE.FIND);
                break;
            }

            yield return null;

        }


    }











    // FIND

    IEnumerator FIND_ST()
    {

        // 발견 시 나의 위치를 그대로 
        Vector3 myPos = transform.position;

        yield return StartCoroutine(EffectControl());

        // 발견 후 플레이어가 범위 내에 있을 경우 추적
        if (TargetPlayerToDragon < TrackingRange)
        {
            nextState(STATE.TRKING);
        }
        // 발견 후 플레이어가 범위 내에 없을 경우 다시 이동 -> 발견 -> 추적
        else
        {
            nextState(STATE.MOVE);
        }

        yield return null;
    }







    // " ! " 이펙트 


    Coroutine markMove = null;

    void UnActive_Eff()
    {
        StopCoroutine(markMove);
        markObj.SetActive(false);
    }


    IEnumerator EffectControl()
    {
        // 발견 이펙트 활성화
        markObj.SetActive(true);

        markMove = StartCoroutine(MarkMovement());

        //애니메이션 호출
        myAnimation.SetBool("move", false);
        myAnimation.SetTrigger("find");

        // 1초 뒤 이펙트 비활성화
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


            // 추적하다가 플레이어가 범위 내에 있을때 드래곤의 속도 증가
            FindPlayer();
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (PlayerToMove != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(PlayerToMove), Time.deltaTime * 3f);
            }


            // 플레이어가 공격범위 안에 있을 경우

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
                transform.rotation = Quaternion.LookRotation(PlayerToMove); // 나중에 부드럽게 움직일 수 있는지 확인 
            }
            // 공격 시 플레이어에게 데미지 전달
            targetPlayer.SendMessage("CallPlayerTransferDamage", AttackPower, SendMessageOptions.DontRequireReceiver);

            //targetPlayer.GetComponent<Player>().PlayerTransferDamage(AttackPower);
            // 플레이어가 추적 범위 내, 공격범위 밖일 때
            if (GameManager.INSTANCE.ISDEAD || (TargetPlayerToDragon > TrackingRange))
            {
                nextState(STATE.MOVE);
                break;
            }
            if (TargetPlayerToDragon > AttackRange && TargetPlayerToDragon < TrackingRange)
            {
                nextState(STATE.TRKING);
                break;
            }
            // 플레이어가 범위 밖일때


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
            // 죽음 애니메이션
            myAnimation.SetTrigger("die");
            // 죽고나서 이펙트 생성
            //죽음이펙트1
            GameObject dieEff = Instantiate(dieEffectobj, this.transform.position, this.transform.rotation);
            //1초뒤에 나머지 이펙트실행
            yield return new WaitForSeconds(1f);
            //죽음이펙트2
            GameObject dieEff2 = Instantiate(dieEffectobj2, this.transform.position, this.transform.rotation);
            //감자반갈죽
            GameObject fruitEff = Instantiate(myFruitObj, this.transform.position, this.transform.rotation);
            // 드래곤 real 삭제
            yield return new WaitForSeconds(1f);
            Debug.Log("주금");
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
        // 공격 범위 기즈모 그려주기
        Gizmos.color = new Color32(255, 0, 0, 40);
        Gizmos.DrawSphere(this.transform.position, AttackRange);

        // 추적 범위 기즈모 그려주기
        Gizmos.color = new Color32(255, 0, 0, 40);
        Gizmos.DrawSphere(this.transform.position, TrackingRange);
    }





}
