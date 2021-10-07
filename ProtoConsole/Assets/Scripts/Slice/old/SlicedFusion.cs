using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedFusion : MonoBehaviour
{
    private static GameObject fusedObject;
    private bool activated = false;
    private float timer =default;
    private float timeBeforeActive = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer+= Time.deltaTime;
        if(timer>=timeBeforeActive && activated!){
            activated=true;
        }
    }

	private void OnTriggerEnter(Collider other) {
		if(activated){
            if(other.GetComponent<SlicedFusion>()!=null){
                Instantiate(fusedObject,transform.position-other.transform.position,Quaternion.identity).transform.parent=transform.parent;
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
	}

    public static void SetFusedItem(GameObject gameObjectFinal){
        fusedObject=gameObjectFinal;
    }
}
