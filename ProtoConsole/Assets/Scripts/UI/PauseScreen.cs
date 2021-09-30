using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : Screen
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickOnPlayButton(){
        //GameManager play
        gameObject.SetActive(false);
    }

    public void ClickOnQuitButton(){
        //Close the Game Or return To Menu and Wipe data
    }
}
