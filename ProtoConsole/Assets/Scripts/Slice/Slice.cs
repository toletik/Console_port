using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Slice : MonoBehaviour
{
    [SerializeField] private  string sphereTag = "Sphere";

	private void OnTriggerExit(Collider other) 
	{
		if(other.CompareTag(sphereTag))
		{
			if(other.TryGetComponent<Planet>(out Planet sphere)) 
				sphere.CutTheSphere();
		}
	}
}
