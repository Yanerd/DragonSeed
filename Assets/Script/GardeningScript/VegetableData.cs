using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vegetable Data", menuName = "Scriptavle Object/Vegetable Data", order = -2)]
public class VegetableData : ScriptableObject
{
    //������ Ÿ��
    [SerializeField]
    private string prefabType;
    public string PrefabType { get { return prefabType; } }

    //�� �ֱ� Ƚ��
    [SerializeField]
    private int waterCount;
    public int WaterCount { get { return waterCount; } }

    //�Ϲ� ���� �ð�
    [SerializeField]
    private float growthTime;
    public float GrowthTime { get { return growthTime; } }

    //ħ�� ��� �� ���� �ð�
    [SerializeField]
    private float onSearchTime;
    public float OnSearchTime { get{ return onSearchTime; } }

    //�й� �� �߰� �� ���� �ð�
    [SerializeField]
    private float looseTime;
    public float LooseTime { get{ return looseTime;} }

    //���� ����
    [SerializeField]
    private int purchasePrice;
    public int PurchasePrice { get{ return purchasePrice; } }



}
