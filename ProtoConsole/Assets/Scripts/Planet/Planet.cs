using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlanetType
{
    Half,
    Quarter,
    Sixth
}

public class Planet : MonoBehaviour
{
    public static event Action OnResetRotation;

    [SerializeField] private RotationForPlanetPart planetPartsRotation = default;

    [Header("ScriptSettings")]
    [SerializeField] private PlanetType typeOfSphere = default;
    [SerializeField] private LevelManager levelManager = default;

    [Header("MovementSettings")]
    [SerializeField] private float[] rotationSpeed = new float[2];
    [SerializeField] private float offSetRotation = default;
    [SerializeField] private float timeScaleSpacing = default;

    private List<Transform> sphereEntityUncuted = new List<Transform>();
    private int cutLeft = default;
    private Action doAction = default;
    private float timer = default;
    private float offSet = 0.1f;

    void Start()
    {
        sphereEntityUncuted.Add(transform);

        if(typeOfSphere == PlanetType.Half)
            cutLeft=1;
        else 
            cutLeft=2;

        doAction = DoActionVoid;
    }

	private void Update() 
    {
		doAction();
        timer+=Time.deltaTime;
	}

    private void DoActionRotate()
    {
        Transform planetPart;

		for (int i = 0; i < sphereEntityUncuted.Count; i++) 
        {
            Quaternion currentRotation = Quaternion.Euler(new Vector3(0, 0, rotationSpeed[i] * Time.deltaTime));
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

    private void DoActionSpacing()
    {
    }

    private void DoActionVoid()
    {
    }

	public void CutTheSphere()
    {
        Debug.LogWarning("CUT");

        if(cutLeft > 0 && timer >= 1)
        {
            List<Transform> arrayOfGameObject = sphereEntityUncuted;

            timer   = 0;
            cutLeft -= 1;
            ResetRotation();

            sphereEntityUncuted = new List<Transform>();

			for (int i = 0; i < arrayOfGameObject.Count; i++) 
            {
				for (int j = 0; j < arrayOfGameObject[i].childCount; j++)
                {
                    sphereEntityUncuted.Add(arrayOfGameObject[i].GetChild(j));
                    
				}
			}

            if(typeOfSphere==PlanetType.Half && cutLeft==0)
                SetModeRotate();
            else if(typeOfSphere==PlanetType.Quarter)
            {
                if(cutLeft==1)
                    SetModeRotate();
                else 
                    SetModeSpacing();
            }
            else 
                SetModeVoid();
        }
    }

    private void SetModeRotate()
    {
        doAction = DoActionRotate;

        Debug.Log("---> rotate !");
    }

    private void SetModeVoid()
    {
        doAction= DoActionVoid;
        Debug.Log("---> nothing...");
    }

    private void SetModeSpacing(){
        StartCoroutine(Space());
        planetPartsRotation.ClearAllRotations();
        Debug.Log("---> spacing !");
    }


    IEnumerator Space()
    {
        SetModeVoid();

        float direction = 1;
        float length = sphereEntityUncuted.Count;

		for (float timer = 0f; timer < 1f; timer += Time.deltaTime / timeScaleSpacing)
        {
            for (int i = 0; i < length; i++) 
            {
                direction = (i >= length/2)? 1 : -1;

                sphereEntityUncuted[i].localPosition = new Vector3(0, ((i % 2 == 0)?  offSet : -offSet), offSet * direction) * timer;
		    }
            levelManager.Settings.MovingPlanetRadiusOffset = offSet * timer;

            yield return  null;
        }

        doAction = DoActionSpacing;
    }

	private void ResetRotation() 
    {
        OnResetRotation?.Invoke();

        for (int i = 0; i < sphereEntityUncuted.Count; i++)
 		{
            sphereEntityUncuted[i].transform.rotation= Quaternion.identity;
		}
	}

	public PlanetType GetTypeOfSphere()
    {
        return typeOfSphere;
    }

    private void OnDestroy()
    {
        OnResetRotation = null;
    }
}
