using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : StateObjects
{
    [SerializeField] protected Transform planet = default;
    [SerializeField] protected float radiusIdle = default;
    [SerializeField] protected float idleDuration = default;
    [SerializeField] protected float attackDuration = default;
    [SerializeField] protected AnimationCurve attackCurve = default;
    [SerializeField] protected float attackCoolDown = 10;

    private Vector3 Offset = default;
    private float angleOffset = default;
    private float attackElapsedTime = 0;
    protected float elapsedTime = 0;
    private Vector3 attackEndPosition = default;
    private Vector3 attackStartPosition = default;
    private float angle = 0;
   

    private void Start()
    {
        SetModeIdle();
    
    }
    protected virtual void SetModeIdle()
    {
        doAction = DoActionIdle;
    }
    protected virtual void SetModeAttack()
    {
        doAction = DoActionAttack;
        SetAttackPosition();
    }

    private void DoActionAttack()
    {
        transform.position = Vector3.Lerp(attackStartPosition,attackEndPosition,attackCurve.Evaluate(elapsedTime/attackDuration));
        if (elapsedTime >= attackDuration) {
            ResetElapsedTime();
            angle -= Mathf.PI;
            Debug.Log(angle);
            
            SetModeIdle();

        }
    }

    private void SetAttackPosition()
    {
        attackStartPosition = transform.position;
        Debug.Log(angle);
        Vector3 direction = (planet.position - transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + direction * radiusIdle * 2,Color.red,2);
        attackEndPosition = transform.position + direction * radiusIdle * 2;
       
      
    }
     private void ManageRotation()
    {
        transform.rotation = Quaternion.LookRotation((planet.position - transform.position).normalized);
    }
    protected virtual void DoActionIdle()
    {
        ManageOrbit();
        ManageRotation();
        attackElapsedTime += Time.deltaTime;

        if (attackElapsedTime >= attackCoolDown) {
            ResetElapsedTime();

            SetModeAttack();
        }
    }

    private void ManageOrbit()
    {
        
        angle += (Mathf.PI * 2) / idleDuration / (1 / Time.deltaTime);

        transform.position = new Vector3(Mathf.Cos(angle) * radiusIdle, 0, Mathf.Sin(angle)*radiusIdle) ;

    }
    private void ResetElapsedTime()
    {
        attackElapsedTime = 0;
        elapsedTime = 0;
    }
 
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        doAction();
    }
}
