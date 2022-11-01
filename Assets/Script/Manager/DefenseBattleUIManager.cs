using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using System.Threading;


public class DefenseBattleUIManager : MonoBehaviour
{

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

    // RaycastHit
    RaycastHit hit;




    Transform camTransfrom = null;


    private void Awake()
    {
        camTransfrom = GameObject.Find("DEFMainCamera(Clone)").GetComponent<Transform>();
        camTransfrom.position = new Vector3(-2.72f, 4.25f, -2.72f);
        camTransfrom.rotation = Quaternion.Euler(30, 45, 0);

        playTimeText = GameObject.Find("PlayTime_number").GetComponent<TextMeshProUGUI>();
        stamina_Slider = GameObject.Find("Stamina_Slider").GetComponent<Slider>();

        // �����̴� �ʱⰪ ����
        stamina_Slider.value = stamina_Slider.maxValue;

        defenseUserAttackPower = defenseUserMaxAttackPower;

        sliderRechargeTime += Time.deltaTime/10;

    }





    private void Start()
    {
        defenseEndUI.SetActive(false);

        GameManager.INSTANCE.TimerStart();

        // ���� ���� ���� ������ ��� �ð� ����
        if (GameManager.INSTANCE.ISDEAD == true)
        {
            GameManager.INSTANCE.TimeOut();
        }

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit))
        {
            Debug.Log("����ĳ��Ʈ �߻�!");
        }

    }





    private void Update()
    {
        if (GameManager.INSTANCE.ISDEAD || GameManager.INSTANCE.ISTIMEOVER)
        {
            defenseEndUI.SetActive(true);
        }
        


        // �÷��� Ÿ�� �ؽ�Ʈ�� �޾ƿ���
        playTimeText.text = ((int)GameManager.INSTANCE.GAMETIME).ToString();

        if (Input.GetMouseButtonDown(0))
        {
            SkillCheck();
            StaminaSliderCheck();
        }
        
        stamina_Slider.value += sliderRechargeTime;
    }




    #region SkillCheck
    void SkillCheck()
    {
        // ���콺 ��ư �Է��� ���� ���
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            StartCoroutine(CheckSkill(hit.transform.position, Quaternion.identity, this.transform));
        }
    }

    // ����ĳ��Ʈ ����ؼ� ������Ʈ ����
    IEnumerator CheckSkill(Vector3 pos, Quaternion rotation, Transform tr)
    {
        GameObject skill = ObjectPoolingManager.inst.Instantiate(attackSkillPrefab, pos + new Vector3(0, 0.5f, 0), Quaternion.identity, tr);
        yield return new WaitForSeconds(1f);
        ObjectPoolingManager.inst.Destroy(skill);
    }
    #endregion




    #region StaminaSliderCheck
    void StaminaSliderCheck()
    {
        // ���¹̳� �����̴�
        Debug.Log($"{defenseUserAttackPower} ������ ��ŭ ����");

        // ���¹̳� ���
        stamina_Slider.value -= 0.2f;

        // ���¹̳ʰ� 0���� �϶��� ���� �Ұ�
        if (stamina_Slider.value <= 0.2f)
        {
            stamina_Slider.value = 0f;
            defenseUserAttackPower = 0;
            Debug.Log($"���� ������{defenseUserAttackPower}, ���ݺҰ�");
        }
    }
    #endregion

    public void OnBackButton()
    {
        Time.timeScale = 1f;
        Photon.Pun.PhotonNetwork.LoadLevel("2_DefenseScene");
        Photon.Pun.PhotonNetwork.Disconnect();
    }

}


