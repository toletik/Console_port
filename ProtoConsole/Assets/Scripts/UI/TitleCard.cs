using UnityEngine;

public class TitleCard : Screen
{
    [SerializeField] Screen screenToInstantiate=default;

	public override void PlayButton() 
    {
		base.PlayButton();

        screenToInstantiate.OpenScreen();
        gameObject.SetActive(false);
	}
}
