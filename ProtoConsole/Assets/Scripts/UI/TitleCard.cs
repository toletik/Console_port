using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleCard : Screen
{
	[SerializeField] private GameObject selectedButton= default;
    [SerializeField] Screen screenToInstantiate=default;

	public override void PlayButton() 
    {
		base.PlayButton();

        screenToInstantiate.OpenScreen();
        gameObject.SetActive(false);
	}

	private void OnEnable() {
		EventSystem.current.firstSelectedGameObject = selectedButton;

	}
}
