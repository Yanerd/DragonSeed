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

        // �����̴� �ʱⰪ ����
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
            Debug.Log("���� ����");
            GameManager.INSTANCE.TimeOut();
            defenseEndUI.SetActive(true);
        }

        // �÷��� Ÿ�� �ؽ�Ʈ�� �޾ƿ���
        playTimeText.text = ((int)GameManager.INSTANCE.GAMETIME).ToString();

        if (Input.GetMouseButtonDown(0))
        {
            SkillCheck();
        }
       
    }

    #region SkillCheck
    void SkillCheck()
    {
        //�����̴� ���� ������� ����
        if (stamina_Slider.value <= 0.2f)
        {
            //�𳯷��� ī����
            StartCoroutine(CameraShake(0.5f,0.01f));
            return;
        }
      
        // ���콺 ��ư �Է��� ���� ���
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //�� �� Ŭ�� �� ���
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            if (hit.transform.name == "NoBuildingZone" || hit.transform.name == "Boundary")
            {
                //�𳯷��� ī����
                StartCoroutine(CameraShake(0.5f, 0.01f));
                return;
            }
        }
        //���׹̳� ���
        stamina_Slider.value -= 0.2f;
        
        //���¹̳� ȸ�� �ڷ�ƾ// bool ������ �ڷ�ƾ �ߺ����� ����
        if(recoveryCheck == false) 
            StartCoroutine(RecoveryStamina());

        //Ray �����
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        //Layer ground �� Raycast�� �Ǵ�
        if (Physics.Raycast(ray, out hit, 1000f, 1 << 9))
        {
            StartCoroutine(CallSkill(hit.transform.position));
        }
    }

    //�����ν��Ͻ÷� ���� ������Ʈ ���� �� ����
    IEnumerator CallSkill(Vector3 pos)
    {
        GameObject skill = PhotonNetwork.Instantiate("O_AttackSkillObj", pos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(skill);
    }

    //�����̴� ���׹̳� ȸ�� �ڷ�ƾ
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

            //��ų�� �� ���� ���������� ���� 
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
        Debug.Log("�� ��ȯ��");

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "2_GardenningScene");
        StopCoroutine(instiateCoroutine);
        instiateCoroutine = null;
        yield break;
    }



}


