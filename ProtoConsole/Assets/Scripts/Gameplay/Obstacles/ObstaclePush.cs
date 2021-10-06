using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePush : Obstacle
{
    // Start is called before the first frame update
    void Start()
    {
        SetModeIdle();
    }

    protected override void DoActionIdle()
    {
        base.DoActionIdle();
        ManageRotation();
    }

    private void ManageRotation()
    {
        transform.rotation = Quaternion.LookRotation((planet.position - transform.position).normalized);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        doAction();
    }
}
