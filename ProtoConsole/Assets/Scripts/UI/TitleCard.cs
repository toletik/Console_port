using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCard : Screen
{
    [SerializeField] GameObject screenToInstantiate=default;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void PlayButton() {
		base.PlayButton();
        screenToInstantiate.SetActive(true);
        gameObject.SetActive(false);
	}
}
