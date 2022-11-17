using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dragon Data", menuName = "Scriptavle Object/Dragon Data", order = -1)]
public class DragonData : ScriptableObject
{
    //타입
    [SerializeField]
    private string dragonType;
    public string DragonType { get { return dragonType; } }

    //공격력
    [SerializeField]
    private float attackPower;
    public float AttackPower { get { return attackPower; } }

    //공격 범위
    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }

    //공격 주기
    [SerializeField]
    private float attackInterval;
    public float AttackInterval { get{ return attackInterval; } }
        
    //체력
    [SerializeField]
    private float hp;
    public float HP { get { return hp; } }

    //속도
    [SerializeField]
    private float speed;
    public float Speed { get { return speed; } }

    //크기
    [SerializeField]
    private float scale;
    public float Scale { get { return scale; } }

    //등급
    [SerializeField]
    private int rare;
    public int Rare { get { return rare; } }

    //판매 가격
    [SerializeField]
    private int saleprice;
    public int SalePrice { get{ return saleprice; } }

    //킬 포인트
    [SerializeField]
    private int killPoint;
    public int KillPoint { get{ return killPoint; } }

   


    // 업그레이드 
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
