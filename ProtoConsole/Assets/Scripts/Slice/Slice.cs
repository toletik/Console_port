using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Slice : MonoBehaviour
{
    [SerializeField] private  SliceEnum typeOfSlicer = default;
    [SerializeField] private  string sphereTag = "Sphere";

	private void OnTriggerExit(Collider other) {
		if(other.CompareTag(sphereTag)){
			if(other.GetComponent<Sphere>().GetTypeOfSphere()==typeOfSlicer)other.GetComponent<Sphere>().CutTheSphere();
			Debug.Log("TriggerExit");
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
