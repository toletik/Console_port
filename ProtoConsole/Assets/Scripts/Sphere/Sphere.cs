using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private SliceEnum typeOfSphere = default;
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

    }

    private void DoActionVoid(){
    }

	public void CutTheSphere(){
        int numberOfChildren;
        int numberOfObjectInList;
        List<GameObject> arrayOfGameObject = sphereEntityUncuted;
        if(cutLeft>=0){
            cutLeft-=1;
            numberOfObjectInList=arrayOfGameObject.Count;
			for (int i = 0; i < numberOfObjectInList; i++) {
                numberOfChildren = arrayOfGameObject[i].transform.childCount;
				for (int j = 0; j < numberOfChildren; j++) {
                    sphereEntityUncuted.Add(arrayOfGameObject[i].transform.GetChild(j).gameObject);
				}
			}
        }
        if(typeOfSphere==SliceEnum.Half&& cutLeft==0)setModeRotate();
        else if (typeOfSphere==SliceEnum.Quarter&& cutLeft==1)setModeVoid();
    }

    private void setModeRotate(){
        doAction = DoActionRotate;
    }

    private void setModeVoid(){
        if(typeOfSphere==SliceEnum.Quarter && cutLeft==0)ResetRotation();
        doAction= DoActionVoid;
    }

	private void ResetRotation() {
		for (int i = 0; i < sphereEntityUncuted.Count; i++) {
            sphereEntityUncuted[i].transform.rotation.eulerAngles.Set(0,0,0);
		}
	}

	public SliceEnum GetTypeOfSphere(){
        return typeOfSphere;
    }
}
