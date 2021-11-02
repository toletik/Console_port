using Com.IsartDigital.Common.UI;

public class TitleCard : UIScreen
{
	public void OnPlayButtonClick() 
    {
        UIManager.Instance.AddScreen<ConnexionScreen>(true);
	}
}
