using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vegetable Data", menuName = "Scriptavle Object/Vegetable Data", order = -2)]
public class VegetableData : ScriptableObject
{
    //프리팹 타입
    [SerializeField]
    private string prefabType;
    public string PrefabType { get { return prefabType; } }

    //물 주기 횟수
    [SerializeField]
    private int waterCount;
    public int WaterCount { get { return waterCount; } }

    //일반 성장 시간
    [SerializeField]
    private float growthTime;
    public float GrowthTime { get { return growthTime; } }

    //침입 허용 시 성장 시간
    [SerializeField]
    private float onSearchTime;
    public float OnSearchTime { get{ return onSearchTime; } }

    //패배 시 추가 될 성장 시간
    [SerializeField]
    private float looseTime;
    public float LooseTime { get{ return looseTime;} }

    //구매 가격
    [SerializeField]
    private int purchasePrice;
    public int PurchasePrice { get{ return purchasePrice; } }



}
