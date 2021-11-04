using Com.IsartDigital.Common.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleCard : UIScreen
{
	[SerializeField] GameObject selectButtonInSettings = default;
	private bool vibrationEnabled = default;
	public void OnPlayButtonClick() 
    {
        UIManager.Instance.AddScreen<ConnexionScreen>(true);
	}

	public void UpdateSelectButton(GameObject button) {
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(button);
	}

	public void SwitchVibration()
	{
		if(vibrationEnabled)
		{
			vibrationEnabled=false;
			VibrationManager.GetSingleton().enableVibrations = false;
		}
		else
		{
			vibrationEnabled=false;
			VibrationManager.GetSingleton().enableVibrations = true;
		}
	}
}
