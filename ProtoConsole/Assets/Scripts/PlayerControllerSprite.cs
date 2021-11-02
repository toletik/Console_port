using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Player Controller Sprite")]
public class PlayerControllerSprite : ScriptableObject
{
	[SerializeField] private List<Sprite> imgController = default;

	public List<Sprite> ImgController {
		get {
			return imgController;
		}

		set {
			imgController=value;
		}
	}
}
