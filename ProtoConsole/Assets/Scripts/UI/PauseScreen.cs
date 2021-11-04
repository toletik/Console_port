using Com.IsartDigital.Common.UI;
using UnityEngine;

public class PauseScreen : UIScreen
{
    public delegate void PauseScreenEventHandler();
    public static event PauseScreenEventHandler OnLevelQuit;
    protected override void Activate()
    {
        base.Activate();
        Time.timeScale = 0;
    }
    public void ClickOnPlayButton()
    {
        //GameManager play
        UIManager.Instance.CloseScreen<PauseScreen>();
    }

    public void ClickOnQuitButton()
    {
        //Close the Game Or return To Menu and Wipe data

        OnLevelQuit?.Invoke();
        UIManager.Instance.AddScreen<ConnexionScreen>(true);
    }
    protected override void Desactivate()
    {
        base.Desactivate();
        Time.timeScale = 1;
        
    }
}
