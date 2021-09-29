using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static Screen[] listOfScreen = default;
    // Start is called before the first frame update
    void Start()
    {
        listOfScreen = GetComponents<Screen>(); 
    }

    public static Screen GetScreen(Component screen){
        Screen foundedScreen=default;

		for (int i = 0; i < listOfScreen.Length; i++) {
            
		}

        return foundedScreen;
    }
}
