using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dragon Data", menuName = "Scriptavle Object/Dragon Data", order = -1)]
public class DragonData : ScriptableObject
{
    //Ÿ��
    [SerializeField]
    private string dragonType;
    public string DragonType { get { return dragonType; } }

    //���ݷ�
    [SerializeField]
    private float attackPower;
    public float AttackPower { get { return attackPower; } }

    //���� ����
    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }

    //���� �ֱ�
    [SerializeField]
    private float attackInterval;
    public float AttackInterval { get{ return attackInterval; } }
        
    //ü��
    [SerializeField]
    private float hp;
    public float HP { get { return hp; } }

    //�ӵ�
    [SerializeField]
    private float speed;
    public float Speed { get { return speed; } }

    //ũ��
    [SerializeField]
    private float scale;
    public float Scale { get { return scale; } }

    //���
    [SerializeField]
    private int rare;
    public int Rare { get { return rare; } }

    //�Ǹ� ����
    [SerializeField]
    private int saleprice;
    public int SalePrice { get{ return saleprice; } }

    //ų ����Ʈ
    [SerializeField]
    private int killPoint;
    public int KillPoint { get{ return killPoint; } }

   


    // ���׷��̵� 
    [SerializeField]
    private float upgradePower;
    public float UpgradePower { get { return upgradePower; } }

    [SerializeField]
    private float upgradeHP;
    public float UpgradeHP { get { return upgradeHP; } }

    [SerializeField]
    private float upgradeScale;
    public float UpgradeScale { get { return upgradeScale; } }

}
