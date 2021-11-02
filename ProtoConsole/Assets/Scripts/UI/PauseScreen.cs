using Com.IsartDigital.Common.UI;

public class PauseScreen : UIScreen
{
    public void ClickOnPlayButton()
    {
        //GameManager play
        UIManager.Instance.CloseScreen<PauseScreen>();
    }

    public void ClickOnQuitButton()
    {
        //Close the Game Or return To Menu and Wipe data

        //UIManager.Instance.AddScreen<ConnexionScreen>(true);
    }
}
