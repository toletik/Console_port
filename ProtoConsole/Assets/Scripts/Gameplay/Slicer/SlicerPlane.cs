using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicerPlane : MonoBehaviour
{
    [SerializeField] private float waitingTimeBeforeSlice = default;
    [SerializeField] private float timeToMove = default;
    [SerializeField] private Transform[] listOfPoint = default;

    private Vector3 nextDirection = default;
    private Vector3 startPosition = default;
    private bool activated= default;
    private bool terminus= default;
    private int index= default;
    private float timer= default;
    // Start is called before the first frame update
    void Start()
    {
        startPosition=transform.position;
        if(listOfPoint.Length>1) nextDirection = listOfPoint[1].position;
        else throw new Exception("List Is not long enough");
    }

    // Update is called once per frame
    void Update()
    {
        if(!terminus)timer+= Time.deltaTime;
        
        if(!activated)CheckIfWaitingTimePassed();
        else Move();
    }

    private void CheckIfWaitingTimePassed(){
        if(timer>=waitingTimeBeforeSlice){
            timer=0;
            activated=true;
        }
    }

	private void Move() {
	    transform.position=Vector3.Lerp(startPosition,nextDirection,timer*timeToMove/listOfPoint.Length);
        if(transform.position== nextDirection)IncrementIndex();
	}

    private void IncrementIndex(){
        index+=1;
        if(index>=listOfPoint.Length-1){
            terminus = true;
            activated = false;
        }
    }
}
