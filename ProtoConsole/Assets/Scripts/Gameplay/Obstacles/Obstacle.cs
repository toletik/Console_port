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

    private float elapsedTime = 0;
    private Vector3 attackEndPosition = default;
    private Vector3 attackStartPosition = default;

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
    }

    private void SetAttackPosition()
    {
        attackStartPosition = transform.position;

        Vector3 direction = (planet.position - transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + direction * radiusIdle * 2);
        attackEndPosition = transform.position + direction * radiusIdle * 2;
    }
    protected virtual void DoActionIdle()
    {
        ManageOrbit();
        SetModeAttack();
    }

    private void ManageOrbit()
    {
        float angle;
        angle = Mathf.Lerp(0, Mathf.PI * 2, elapsedTime / idleDuration);

        transform.position = new Vector3(Mathf.Cos(angle) * radiusIdle, 0, Mathf.Sin(angle));

        if (elapsedTime >= idleDuration) elapsedTime = 0;
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        doAction();
    }
}
