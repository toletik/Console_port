using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [Header("ScriptSettings")]
    [SerializeField] private SliceEnum typeOfSphere = default;

    [Header("MovementSettings")]
    [SerializeField] private float[] rotationSpeed = new float[2];
    [SerializeField] private float offSetRotation = default;

    private List<GameObject> sphereEntityUncuted = new List<GameObject>();
    private int cutLeft = default;
    private Action doAction = default;
    void Start()
    {
        sphereEntityUncuted.Add(gameObject);
        if(typeOfSphere == SliceEnum.Half)cutLeft=1;
        else cutLeft=2;
        doAction = DoActionVoid;
    }

	private void Update() {
		doAction();
	}

    private void DoActionRotate(){
		for (int i = 0; i < 2; i++) {
            sphereEntityUncuted[i].transform.Rotate(new Vector3(0,0,rotationSpeed[i]*Time.deltaTime));
		}
    }

    private void DoActionVoid(){
    }

	public void CutTheSphere(){
        int numberOfChildren;
        int numberOfObjectInList;

        List<GameObject> arrayOfGameObject = sphereEntityUncuted;


        if(cutLeft>=0){

            ResetRotation();

            sphereEntityUncuted=new List<GameObject>();
            cutLeft-=1;
            numberOfObjectInList=arrayOfGameObject.Count;

			for (int i = 0; i < numberOfObjectInList; i++) {
                numberOfChildren = arrayOfGameObject[i].transform.childCount;
                Debug.Log(numberOfChildren);
				for (int j = 0; j < numberOfChildren; j++) {
                    sphereEntityUncuted.Add(arrayOfGameObject[i].transform.GetChild(j).gameObject);
				}
			}
        }

        if(typeOfSphere==SliceEnum.Half&& cutLeft==0)SetModeRotate();
        else SetModeVoid();
    }

    private void SetModeRotate(){
        doAction = DoActionRotate;
    }

    private void SetModeVoid(){
        doAction= DoActionVoid;
    }

	private void ResetRotation() {
		for (int i = 0; i < sphereEntityUncuted.Count; i++) {
            sphereEntityUncuted[i].transform.rotation= Quaternion.identity;
		}
	}

	public SliceEnum GetTypeOfSphere(){
        return typeOfSphere;
    }
}
