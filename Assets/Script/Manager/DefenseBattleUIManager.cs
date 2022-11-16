using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class DefenseBattleUIManager : MonoBehaviourPun
{
    [SerializeField] Image sliderImage = null;
    //DefenseEndUI
    [SerializeField] GameObject defenseEndUI = null;

    // Defense User
    #region Defense User
    [Header("[Defense User Stats]")]
    [SerializeField] private float defenseUserAttackPower;
    [SerializeField] private float defenseUserMaxAttackPower = 10f;
    [SerializeField] GameObject attackSkillPrefab;
        

    #endregion

    // Components
    #region components
    // Play Time
    [SerializeField] TextMeshProUGUI playTimeText;
    [SerializeField] float sliderRechargeTime = 0;

    // Stamina Slider
    [SerializeField] Slider stamina_Slider;
    #endregion

    Transform camTransfrom = null;
    bool recoveryCheck;
    Vector3 camOriginPos;
    private void Awake()
    {
        camTransfrom = GameObject.Find("O_DEFMainCamera(Clone)").GetComponent<Transform>();
        camTransfrom.position = new Vector3(-2.72f, 4.25f, -2.72f);
        camTransfrom.rotation = Quaternion.Euler(30, 45, 0);

        playTimeText = GameObject.Find("PlayTime_number").GetComponent<TextMeshProUGUI>();
        stamina_Slider = GameObject.Find("Stamina_Slider").GetComponent<Slider>();

        // 슬라이더 초기값 설정
        stamina_Slider.value = stamina_Slider.maxValue;

        defenseUserAttackPower = defenseUserMaxAttackPower;

        sliderRechargeTime = Time.deltaTime/10;

        camOriginPos = camTransfrom.position;
    }

    private void Start()
    {
        defenseEndUI.SetActive(false);

        GameManager.INSTANCE.TimerStart();
       
    }

    private void Update()
    {
        if (GameManager.INSTANCE.ISDEAD || GameManager.INSTANCE.ISTIMEOVER)
        {
            Debug.Log("게임 끝남");
            GameManager.INSTANCE.TimeOut();
            defenseEndUI.SetActive(true);
        }

        // 플레이 타임 텍스트로 받아오기
        playTimeText.text = ((int)GameManager.INSTANCE.GAMETIME).ToString();

        if (Input.GetMouseButtonDown(0))
        {
            SkillCheck();
        }
       
    }

    #region SkillCheck
    void SkillCheck()
    {
        //슬라이더 값이 적을경우 리턴
        if (stamina_Slider.value <= 0.2f)
        {
            //흩날려라 카본앵
            StartCoroutine(CameraShake(0.5f,0.01f));
            return;
        }
      
        // 마우스 버튼 입력이 있을 경우
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //맵 밖 클릭 할 경우
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            if (hit.transform.name == "NoBuildingZone" || hit.transform.name == "Boundary")
            {
                //흩날려라 카본앵
                StartCoroutine(CameraShake(0.5f, 0.01f));
                return;
            }
        }
        //스테미너 사용
        stamina_Slider.value -= 0.2f;
        
        //스태미나 회복 코루틴// bool 값으로 코루틴 중복실행 막음
        if(recoveryCheck == false) 
            StartCoroutine(RecoveryStamina());

        //Ray 디버그
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        //Layer ground 만 Raycast가 판단
        if (Physics.Raycast(ray, out hit, 1000f, 1 << 9))
        {
            StartCoroutine(CallSkill(hit.transform.position));
        }
    }

    //포톤인스턴시로 게임 오브젝트 생성 및 삭제
    IEnumerator CallSkill(Vector3 pos)
    {
        GameObject skill = PhotonNetwork.Instantiate("O_AttackSkillObj", pos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(skill);
    }

    //슬라이더 스테미나 회복 코루틴
    IEnumerator RecoveryStamina()
    {
        recoveryCheck = true;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            stamina_Slider.value += sliderRechargeTime;

            if(stamina_Slider.value>= stamina_Slider.maxValue)
            {
                recoveryCheck = false;
                yield break;
            }

            //스킬을 못 쓸때 빨간색으로 변경 
            if(stamina_Slider.value<=0.2f)
            {
                sliderImage.color = Color.red;
            }
            else
            {
                sliderImage.color = Color.white;
            }
        }
    }

    IEnumerator CameraShake(float shakeTime, float shakeRange)
    {
        float timer = 0;

        while (timer<= shakeTime)
        {
            camTransfrom.localPosition = Random.insideUnitSphere * shakeRange + camOriginPos;

            timer += Time.deltaTime;
            yield return null;
        }
        camTransfrom.localPosition = camOriginPos;
    }



    #endregion

    Coroutine instiateCoroutine;

    public void OnBackButton()
    {
        Time.timeScale = 1f;

        GameManager.INSTANCE.SCENENUM = 1;
        SendGameEnd();

        PhotonNetwork.Disconnect();

        SceneManager.LoadScene("2_GardenningScene");

        GameManager.INSTANCE.Initializing();

        instiateCoroutine = StartCoroutine(GoBackSceneInstantiate());
    }


    [PunRPC]
    public void SendGameEnd()
    {
        photonView.RPC("GetGameEnd", RpcTarget.All, true);
    }
    public void GetGameEnd(bool flag)
    {
        GameManager.INSTANCE.GameEndCorrect = flag;
    }

    IEnumerator GoBackSceneInstantiate()
    {
        Debug.Log("씬 전환중");

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "2_GardenningScene");
        StopCoroutine(instiateCoroutine);
        instiateCoroutine = null;
        yield break;
    }



}


