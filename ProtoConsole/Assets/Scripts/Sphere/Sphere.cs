using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private SliceEnum typeOfSphere = default;
    private List<GameObject> sphereEntityUncuted = new List<GameObject>();
    private int cutLeft = default;
    void Start()
    {
        sphereEntityUncuted.Add(gameObject);
        if(typeOfSphere == SliceEnum.Half)cutLeft=1;
        else cutLeft=2;
    }

    public void CutTheSphere(){
        int numberOfChildren;
        List<GameObject> arrayOfGameObject = sphereEntityUncuted;
        if(cutLeft>=0){
            Debug.Log("cutStillAvailable");
            cutLeft-=1;
			for (int i = 0; i < arrayOfGameObject.Count; i++) {
                if(arrayOfGameObject[i] == null)break;
                numberOfChildren = arrayOfGameObject[i].transform.childCount;
				for (int j = 0; j < numberOfChildren; j++) {
                    sphereEntityUncuted.Add(arrayOfGameObject[i].transform.GetChild(j).gameObject);
				}
			}
        }
        Debug.Log(sphereEntityUncuted.Count);
    }

    public SliceEnum GetTypeOfSphere(){
        return typeOfSphere;
    }
}
