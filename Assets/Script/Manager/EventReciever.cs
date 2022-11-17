using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnAttackStartEvent();
public delegate void OnAttackEndEvent();

public delegate void OnEnableTransferDamageEvent();
public delegate void OnDisableTransferDamageEvent();

public delegate void OnPlayerHPChangeEvent(float curHp, float maxHp);

public delegate void OnDragonHPChangeEvent(float curHp, float maxHp);

public class EventReciever : MonoBehaviour
{
    public OnAttackEndEvent callBackAttackEndEvent = null;
    public OnAttackStartEvent callBackAttackStartEvent = null;

    public OnEnableTransferDamageEvent callBackEnableTransferDamageEvent = null;
    public OnDisableTransferDamageEvent callBackDisableTransferDamageEvent = null;

    public OnPlayerHPChangeEvent callBackPlayerHPChangeEvent = null;

    public OnDragonHPChangeEvent callBackDragonHPChangeEvent = null;


   

    public void AttackStartEvent()
    {
        if (callBackAttackStartEvent != null)
            callBackAttackStartEvent();
    }
    public void AttackEndEvent()
    {
        if (callBackAttackEndEvent != null)
            callBackAttackEndEvent();
    }

    public void EnableDMGEvent()
    {
        if (callBackEnableTransferDamageEvent != null)
            callBackEnableTransferDamageEvent();
    }
    public void DisableDMGEvent()
    {
        if (callBackDisableTransferDamageEvent != null)
            callBackDisableTransferDamageEvent();
    }
}
