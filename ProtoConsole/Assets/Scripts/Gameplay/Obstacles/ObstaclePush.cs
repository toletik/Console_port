using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePush : Obstacle
{
    // Start is called before the first frame update
    [SerializeField]private Transform side1  = default;
    [SerializeField]private Transform side2 = default;
    [SerializeField] private float timeScaleX = 2;
    [SerializeField] private float timeScaleZ = 1;
    [SerializeField] private float timeScaleMove = 4;


    private Transform[] sides = default;

    private bool growingScaleX = true;
    private bool growingScaleZ = true;

    private bool movingLeft = false;

    private float elapsedTimeX = 0;
    private float elapsedTimeZ = 0;

    private float elapsedTimeMove = 0;

    private const float  OFFSET_LEFT = 0.2f;
    private const float  OFFSET_RIGHT = 0.1f;



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
        elapsedTimeZ += deltaTime / timeScaleZ;
        elapsedTimeX += deltaTime /timeScaleX;
        elapsedTimeMove += deltaTime/timeScaleMove;
        
       
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
        if (elapsedTimeMove >= 1)
        {
            elapsedTimeMove = 0;
            movingLeft = !movingLeft;
        }


    }
    protected override void DoActionIdle()
    {
        base.DoActionIdle();
        ManageElapseTimeScale(Time.deltaTime);
        ManageScale();
        ManageMove();
    }

    private void ManageScale()
    {
        float newScaleX = 0;
        float newScaleZ = 0;

        newScaleZ = TestElapsedTimeScale(growingScaleX, elapsedTimeX);
        newScaleX= TestElapsedTimeScale(growingScaleZ, elapsedTimeZ);

        for (int i = 0; i < sides.Length; i++)
        {
            sides[i].localScale = new Vector3(newScaleX, 1, newScaleZ);
        }
    }
    private void ManageMove()
    {
        float newPos = 0;

        newPos = TestElapsedTimeMove(movingLeft, elapsedTimeMove);
        Debug.Log(newPos);

        for (int i = 0; i < sides.Length; i++)
        {
            Transform currentSide = sides[i];
            

            currentSide.localPosition =new Vector3(newPos, 1, 0);
        }
    }

    private float TestElapsedTimeScale(bool growing,float elapsedTime)
    {
        if(growing) return Mathf.Lerp(OFFSET_RIGHT, OFFSET_LEFT, elapsedTime);
        else return  Mathf.Lerp(OFFSET_LEFT, OFFSET_RIGHT, elapsedTime);
    }
    private float TestElapsedTimeMove(bool direction,float elapsedTime)
    {
        if (movingLeft) return Mathf.Lerp(-1, 1, elapsedTimeMove);
        else return Mathf.Lerp(1, -1,elapsedTimeMove);
    }
    
    
}
