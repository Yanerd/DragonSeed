using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] bool testMode = false;

    #region components
    Transform playerTransform = null;
    Rigidbody playerRigidbody = null;
    Animator playerAnimator = null;
    [SerializeField] GameObject weaponCollider = null;
    [SerializeField] Transform camTransfrom = null;
    [SerializeField] Transform cam = null;

    //event components 
    public EventReciever playerEvent = null;
    List<GameObject> liveDragon = new List<GameObject>();
    #endregion

    #region constant value
    [Header("[Player Stats]")]
    [SerializeField] private float playerMaxHp = 1000f;
    [SerializeField] private float playerSpeed = 0.4f;
    [SerializeField] private float playerAttackPower = 4000f; 
    [SerializeField] private float playerAttackInterval = 2f;

    [SerializeField] private float RotCamSpeed = 200f;
    Vector3 camToPlayerVec = Vector3.zero;
    #endregion

    #region prefab
    [Header("[Player FX]")]
    [SerializeField] GameObject swingWeaponFX = null;
    [SerializeField] GameObject hitFX = null;
    #endregion

    #region variables value
    private float playerCurHp = 0f;
    private float axisX = 0f;
    private float axisZ = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;

    Vector3 lookForward = Vector3.zero;
    Vector3 lookRight = Vector3.zero;
    Vector3 moveDir = Vector3.zero;

    private float speed = 0f;
    private Vector3 newPosition = Vector3.zero;

    GameObject targetObject = null;
    #endregion

    #region state check value
    private bool isRun = false;
    private bool isAttacking = false;
    private bool isDead = false;
    #endregion

    #region transition value
    Coroutine jumpCoroutine = null;
    Coroutine curCoroutine = null;
    STATE curState;
    #endregion

    #region property
    public float PlayerAttackPower { get { return playerAttackPower; } }
    #endregion

    [SerializeField] Slider hpSlider = null;
    [SerializeField] Slider hpFollowSlider = null;


    private void Awake()
    {
        if (testMode || photonView.IsMine )
        {
            #region ref components
            playerTransform = this.GetComponent<Transform>();
            playerRigidbody = this.GetComponent<Rigidbody>();
            playerAnimator = this.GetComponent<Animator>();
            playerEvent = this.GetComponent<EventReciever>();

            #endregion



            #region deligate chain
            playerEvent.callBackAttackStartEvent += CallOnAttackStart;
            playerEvent.callBackAttackEndEvent += CallOnAttackEnd;
            playerEvent.callBackEnableTransferDamageEvent += CallOnWeaponCollider;
            playerEvent.callBackDisableTransferDamageEvent += CallOffWeaponCollider;
            playerEvent.callBackPlayerHPChangeEvent += OnChangedHp;
            #endregion

            cam.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        if (testMode || photonView.IsMine)
        {
            #region initializing
            //invader client

            StartCoroutine(FindCamera());

            //live dragon list init
            for (int i = 0; i < FindObjectsOfType<Dragon>().Length; i++)
            {
                liveDragon.Add(FindObjectsOfType<Dragon>()[i].gameObject);
            }


            camToPlayerVec = playerTransform.position - camTransfrom.position;

            //visible value
            lookForward = new Vector3(camTransfrom.forward.x, 0f, camTransfrom.forward.z).normalized;
            lookRight = new Vector3(camTransfrom.right.x, 0f, camTransfrom.right.z).normalized;
            moveDir = lookForward * axisZ + lookRight * axisX;

            //move position value
            newPosition = this.transform.position;

            ////player state
            //playerCurHp = playerMaxHp;
            //Debug.Log($"이니셜 체력 : {playerCurHp}");

            //state value
            curState = STATE.NONE;

            //weapon collider init
           

            //start state
            ChangeState(STATE.IDLE);
            #endregion
        }
        playerCurHp = playerMaxHp;
        Debug.Log($"이니셜 체력 : {playerCurHp}");

        CallOffWeaponCollider();

    }

    IEnumerator FindCamera()
    {
        while (true)
        {
            if (testMode || photonView.IsMine)
            {
                Debug.Log("카메라 찾는 중");
                if (camTransfrom==null)
                {
                    if (testMode == true)
                    {
                        camTransfrom = GameObject.Find("O_CameraArm").GetComponent<Transform>();
                    }
                    else
                    {
                        camTransfrom = GameObject.Find("O_CameraArm").GetComponent<Transform>();
                    }
                    if (camTransfrom != null)
                    {
                        Debug.Log("카메라 찾음");
                        yield break;
                    }
                }
            }
            yield return null;
        }
        
    }

    private void Update()
    {
        Debug.Log(playerCurHp);

        if (GameManager.INSTANCE.ISDEAD || GameManager.INSTANCE.ISTIMEOVER) return;

        if (testMode || photonView.IsMine)
        {
            Debug.Log("카메라 위치 바꾸렴");
            CamTransFormControll();
            InputControll();
        }

        

    }

    private void OnDisable()
    {
        if(playerEvent!=null)
        {
            playerEvent.callBackAttackStartEvent -= CallOnAttackStart;
            playerEvent.callBackAttackEndEvent -= CallOnAttackEnd;
            playerEvent.callBackEnableTransferDamageEvent -= CallOnWeaponCollider;
            playerEvent.callBackDisableTransferDamageEvent -= CallOffWeaponCollider;
        }
    }

    #region Position Controll
    private void InputControll()//keyboard and mouse input controll
    {
        if (!isAttacking)
        {
            //keybarod move input
            axisX = Input.GetAxis("Horizontal");
            axisZ = Input.GetAxis("Vertical");

            //mouse view input
            if (!GameManager.INSTANCE.ISLOCKON)
            {
                mouseX += Input.GetAxis("Mouse X");
                mouseY += Input.GetAxis("Mouse Y");
            }
        }

        //shift dash input
        if (Input.GetKey(KeyCode.LeftShift)) isRun = true;
        else isRun = false;

        //mouse left click input
        if (Input.GetMouseButtonDown(0)) playerAnimator.SetTrigger("isattack");

        //mouse right click input
        if (Input.GetMouseButtonDown(1)) playerAnimator.SetTrigger("isspecialattack");

        //LockOn Input
        if (!GameManager.INSTANCE.ISLOCKON && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f눌림");
            GameManager.INSTANCE.ISLOCKON = true;
            StartCoroutine(LockOn());
        }
        else if (GameManager.INSTANCE.ISLOCKON && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.INSTANCE.ISLOCKON = false;
        }
    }

    private void CamTransFormControll()//camera transform controll
    {
        //camTransfrom.position = this.transform.position + new Vector3(0, 0.1f, -0.5f);
        camTransfrom.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

    }
    #endregion

    #region LockOn Loop
    IEnumerator LockOn()//lockon coroutine
    {
        Debug.Log("lock");
        VisibleCheck();
        Vector3 viewDir = (Camera.main.transform.forward).normalized;

        while (GameManager.INSTANCE.ISLOCKON)/////////////////////////////////////////////////////////////////////////////////////target missing exception
        {
            if (targetObject == null)
            {
                GameManager.INSTANCE.ISLOCKON = false;
                yield break;
            }

            Vector3 targetDir = (targetObject.transform.position - playerTransform.position).normalized;
            viewDir = Vector3.Lerp(viewDir, targetDir, 0.08f);

            camTransfrom.rotation = Quaternion.LookRotation(viewDir);
            playerTransform.rotation = Quaternion.LookRotation(new Vector3(viewDir.x, 0f, viewDir.z));

            if (targetObject.activeSelf == false)
            {
                GameManager.INSTANCE.ISLOCKON = false;
                yield break;
            }

            yield return null;
        }
    }

    void VisibleCheck()//find object visible and nearby of cam middle position
    {
        float shortistDis = float.MaxValue;
        float newDis = float.MaxValue;

        targetObject = null;

        for (int i = 0; i < liveDragon.Count; i++)
        {
            Vector3 objectViewPos = Camera.main.WorldToViewportPoint(liveDragon[i].transform.position);

            //visible check
            if (objectViewPos.x >= 0 && objectViewPos.x <= 1 && objectViewPos.y >= 0 && objectViewPos.y <= 1)
            {
                //shortist targeting
                newDis = Mathf.Pow((objectViewPos.x - 0.5f), 2f) + Mathf.Pow((objectViewPos.y - 0.5f), 2f);
                if (newDis <= shortistDis)
                {
                    shortistDis = newDis;
                    targetObject = liveDragon[i].gameObject;
                }
            }
        }

        if (shortistDis == float.MaxValue || targetObject == null)
        {
            GameManager.INSTANCE.ISLOCKON = false;
        }
    }
    #endregion

    #region State Transition
    //###############################################################################//
    //STATE value
    enum STATE
    {
        NONE,
        IDLE,
        MOVE,
        DIE,
        MAX
    }
    void ChangeState(STATE newState)
    {
        if (newState == curState) return;//may be state didnt changed return function

        if (curCoroutine != null)//may be state changed
        {
            StopCoroutine(curCoroutine);
        }

        curState = newState;

        curCoroutine = StartCoroutine("STATE_" + newState.ToString());
    }
    IEnumerator STATE_IDLE()
    {
        if (isDead) yield break;
        //animation
        playerAnimator.SetBool("ismove", false);

        //function
        while (true)
        {
            //Debug.Log("idle");

            if (axisX != 0 || axisZ != 0)
            {
                ChangeState(STATE.MOVE);
            }

            yield return null;
        }
    }
    IEnumerator STATE_MOVE()
    {
        //animation
        playerAnimator.SetBool("ismove", true);

        //function
        while (true)
        {
            //animation
            playerAnimator.SetFloat("axisX", axisX);
            playerAnimator.SetFloat("axisZ", axisZ);

            if (!GameManager.INSTANCE.ISLOCKON)//normal move
            {
                //animation
                playerAnimator.SetBool("islockon", false);

                if (isRun)
                {
                    playerSpeed = 1.1f;
                    speed = Mathf.Lerp(speed, 1f, 0.1f);//blendig value lerp

                    playerAnimator.SetFloat("speed", speed);
                }
                else
                {
                    playerSpeed = 0.4f;
                    speed = Mathf.Lerp(speed, 0f, 0.1f);//blendig value lerp

                    playerAnimator.SetFloat("speed", speed);
                }

                //function
                lookForward = new Vector3(camTransfrom.forward.x, 0f, camTransfrom.forward.z).normalized;
                lookRight = new Vector3(camTransfrom.right.x, 0f, camTransfrom.right.z).normalized;
                
                //move direction
                if (axisX != 0 || axisZ != 0)
                {
                    moveDir = lookForward * axisZ + lookRight * axisX;
                }
                playerTransform.rotation = Quaternion.LookRotation(moveDir);

                //move forward
                newPosition = moveDir.normalized * playerSpeed * Time.deltaTime;
                playerRigidbody.position += newPosition;
            }
            else//lockon move
            {
                //animation
                playerAnimator.SetBool("islockon", true);

                //move direction
                lookForward = new Vector3(camTransfrom.forward.x, 0f, camTransfrom.forward.z).normalized;
                lookRight = new Vector3(camTransfrom.right.x, 0f, camTransfrom.right.z).normalized;
                moveDir = lookForward * axisZ + lookRight * axisX;

                //move forward
                playerSpeed = 0.4f;
                newPosition = (moveDir.normalized * playerSpeed * Time.deltaTime);
                playerRigidbody.position += newPosition;
                playerRigidbody.velocity = new Vector3(moveDir.x, playerRigidbody.velocity.y, moveDir.z);
            }

            if (axisX == 0 && axisZ == 0)
            {
                ChangeState(STATE.IDLE);
                yield break;
            }

            yield return null;
        }
    }
    IEnumerator STATE_DIE()
    {
        
        GameManager.INSTANCE.ISDEAD = true;

        if (playerAnimator!=null)
            playerAnimator.SetTrigger("isdead");

        yield return new WaitForSeconds(1.2f);

        Debug.Log("타임스탑");

        Time.timeScale = 0;

        yield return null;
    }
    //###############################################################################//
    #endregion

    #region Transfer Function
    
    public void CallOnAttackStart()
    {
        photonView.RPC("OnAttackStart", RpcTarget.All);
    }
    public void CallOnAttackEnd()
    {
        photonView.RPC("OnAttackEnd", RpcTarget.All);
    }
    public void CallOnWeaponCollider()
    {
        photonView.RPC("OnWeaponCollider", RpcTarget.All);
    }
    public void CallOffWeaponCollider()
    {
        photonView.RPC("OffWeaponCollider", RpcTarget.All);
    }

    [PunRPC]
    private void OnAttackStart()
    {
        isAttacking = true;
        //exception
        axisX = 0f;
        axisZ = 0f;
    }
    [PunRPC]
    private void OnAttackEnd()
    {
        isAttacking = false;
    }
    [PunRPC]
    private void OnWeaponCollider()
    {
        weaponCollider.SetActive(true);
    }
    [PunRPC]
    private void OffWeaponCollider()
    {
        Debug.Log("콜라이더 꺼짐");
        Debug.Log(weaponCollider.name);
        weaponCollider.SetActive(false);
    }

    public void CallPlayerTransferDamage(float damage)
    {
        if (testMode)
        {
            PlayerTransferDamage(damage);
        }
        else
        {
            Debug.Log("데미지 받음");
            photonView.RPC("PlayerTransferDamage", RpcTarget.Others, damage);
        }
    }

    [PunRPC]
    public void PlayerTransferDamage(float damage)
    {
        if (isDead) return;
        
        //animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("ishit");

        if (playerCurHp <= 0f)
        {
            Debug.Log("플레이어 죽음");
            playerCurHp = 0f;
            isDead = true;
            ChangeState(STATE.DIE);
            return;
        }

        playerCurHp -= damage;

        if (playerEvent.callBackPlayerHPChangeEvent != null)
            playerEvent.callBackPlayerHPChangeEvent(playerCurHp, playerMaxHp);

    }
    private void OnChangedHp(float curHp, float maxHp)
    {
        hpSlider.value = curHp / maxHp;
        StartCoroutine(FollowSlider());
    }

    IEnumerator FollowSlider()
    {
        while (true)
        {
            hpFollowSlider.value = Mathf.Lerp(hpFollowSlider.value, hpSlider.value, Time.deltaTime / 5f);

            if (hpFollowSlider.value == hpSlider.value) yield break;

            yield return null;
        }
    }



    #endregion
}
