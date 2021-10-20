using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public static event Action OnResetRotation;

    [SerializeField] private RotationForPlanetPart planetPartsRotation = default;

    [Header("ScriptSettings")]
    [SerializeField] private SliceEnum typeOfSphere = default;
    [SerializeField] private LevelManager levelManager = default;

    [Header("MovementSettings")]
    [SerializeField] private float[] rotationSpeed = new float[2];
    [SerializeField] private float offSetRotation = default;
    [SerializeField] private float timeScaleSpacing = default;

    private List<GameObject> sphereEntityUncuted = new List<GameObject>();
    private int cutLeft = default;
    private Action doAction = default;
    private float timer = default;
    void Start()
    {
        sphereEntityUncuted.Add(gameObject);
        if(typeOfSphere == SliceEnum.Half)cutLeft=1;
        else cutLeft=2;
        doAction = DoActionVoid;
    }

	private void Update() {
		doAction();
        timer+=Time.deltaTime;
	}

    private void DoActionRotate()
    {
        float lenght = sphereEntityUncuted.Count;
        Transform planetPart;
        Quaternion currentRotation;

		for (int i = 0; i < lenght; i++) 
        {
            currentRotation = Quaternion.Euler(new Vector3(0, 0, rotationSpeed[i] * Time.deltaTime));
            planetPart = sphereEntityUncuted[i].transform;

            planetPart.rotation = currentRotation * planetPart.rotation;

            UpdateRotationsInfo(planetPart, currentRotation);
        }
    }

    private void UpdateRotationsInfo(Transform uncutedRotatingPart, Quaternion rotation)
    {
        for (int i = 0; i < uncutedRotatingPart.childCount; i++)
        {
            planetPartsRotation.UpdateFrameRotationForPart(uncutedRotatingPart.GetChild(i), rotation);
        }
    }

    private void DoActionSpacing(){
    }

    private void DoActionVoid(){
    }

	public void CutTheSphere(){
        int numberOfChildren;
        int numberOfObjectInList;

        List<GameObject> arrayOfGameObject = sphereEntityUncuted;

        Debug.LogWarning("CUT");

        if(cutLeft>0&& timer>=1){
            timer=0;
            ResetRotation();

            sphereEntityUncuted=new List<GameObject>();
            cutLeft-=1;
            numberOfObjectInList=arrayOfGameObject.Count;

			for (int i = 0; i < numberOfObjectInList; i++) {
                numberOfChildren = arrayOfGameObject[i].transform.childCount;
				for (int j = 0; j < numberOfChildren; j++) {
                    sphereEntityUncuted.Add(arrayOfGameObject[i].transform.GetChild(j).gameObject);
                    
				}
			}

            if(typeOfSphere==SliceEnum.Half&& cutLeft==0)SetModeRotate();

            else if(typeOfSphere==SliceEnum.Quarter){
                if(cutLeft==1)SetModeRotate();
                else SetModeSpacing();
            }

            else SetModeVoid();
        }
    }

    private void SetModeRotate(){
        doAction = DoActionRotate;

        Debug.Log("---> rotate !");
    }

    private void SetModeVoid(){
        doAction= DoActionVoid;
        Debug.Log("---> nothing...");
    }

    private void SetModeSpacing(){
        StartCoroutine("Space");
        planetPartsRotation.ClearAllRotations();
    }

    private float offSet = 0.1f;

    IEnumerator Space(){
        SetModeVoid();

        float d = 1;
        float lenght = sphereEntityUncuted.Count;
        float timer = 0;

		while (timer<1) {
            timer+= Time.deltaTime/timeScaleSpacing;
            for (int i = 0; i < lenght; i++) {
                if(i>=lenght/2)d=-1;
                else d =1;
                sphereEntityUncuted[i].transform.localPosition = new Vector3(0,offSet*(-offSet*2*(i%2)),-offSet*d)*timer;
		    }
            levelManager.Settings.SetPlanetMovingRadiusOffset(offSet*timer);
            yield return  null;
        }
        doAction=DoActionSpacing;
    }

	private void ResetRotation() 
    {
        OnResetRotation?.Invoke();

        for (int i = 0; i < sphereEntityUncuted.Count; i++) {
            sphereEntityUncuted[i].transform.rotation= Quaternion.identity;
		}
	}

	public SliceEnum GetTypeOfSphere(){
        return typeOfSphere;
    }

    private void OnDestroy()
    {
        OnResetRotation = null;
    }
}
