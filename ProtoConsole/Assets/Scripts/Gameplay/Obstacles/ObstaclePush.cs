using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePush : Obstacle
{
    // Start is called before the first frame update
    [SerializeField]private Transform side1  = default;
    [SerializeField]private Transform side2 = default;

    private Transform[] sides = default;

    private bool growingScaleX = true;
    private bool growingScaleZ = true;

    private float currentScaleX = 0.1f;
    private float currentScaleZ = 0.1f;

    private float elapsedTimeX = 0;
    private float elapsedTimeZ = 0;
    void Start()
    {
        sides = new Transform[]{
                side1,
                side2 
        };

        SetModeIdle();
    }
    
    // Update is called once per frame
    void Update()
    {
        IncrementeElapseTime(Time.deltaTime);
        doAction();
    }

    private void IncrementeElapseTime(float deltaTime)
    {
       
        elapsedTime += deltaTime;
    }
    private void ManageElapseTimeScale(float deltaTime)
    { 
        elapsedTimeZ += deltaTime / 2;
        elapsedTimeX += deltaTime ;

        
       
        if(elapsedTimeX>= 1)
        {
            elapsedTimeX = 0;
            growingScaleX = !growingScaleX;
        }
        if (elapsedTimeZ >= 1)
        {
            elapsedTimeZ = 0;
            growingScaleZ = !growingScaleZ;
        }


    }
    protected override void DoActionIdle()
    {
        base.DoActionIdle();
        ManageElapseTimeScale(Time.deltaTime);
        ManageScale();
    }

    private void ManageScale()
    {
        float newScaleX = 0;
        float newScaleZ = 0;

        newScaleZ =  TestGrowing(growingScaleX, elapsedTimeX);
        newScaleX=  TestGrowing(growingScaleZ, elapsedTimeZ);

        for (int i = 0; i < sides.Length; i++)
        {
            sides[i].localScale = new Vector3(newScaleX, 1, newScaleZ);
        }
    }

    private float TestGrowing(bool growing,float elapsedTime)
    {
        if(growing) return Mathf.Lerp(0.1f, 0.2f, elapsedTime);
        else return  Mathf.Lerp(0.2f, 0.1f, elapsedTime);
    }
}
