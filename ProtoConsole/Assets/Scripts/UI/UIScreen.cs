///-----------------------------------------------------------------
/// Author : Joéva FOMBARON
/// Date : 11/11/2020 13:05
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Com.IsartDigital.Common.UI
{
	public class UIScreen : MonoBehaviour
	{
		public delegate void UIScreenEventHander(UIScreen screen);
		public static event UIScreenEventHander OnScreenClosed;

		[SerializeField] protected GameObject selectedButton = default;
		public void Init() 
		{

		}

		virtual public void EnterScreen()
		{
			//tween et quand fini, activate
			Activate();
		}

		virtual protected void Activate()
		{
			EventSystem.current.SetSelectedGameObject(selectedButton);
			if(selectedButton.TryGetComponent(out Button buttonComponent))buttonComponent.OnSelect(null);
		}

		virtual protected void Desactivate()
		{

		}

		virtual public void LeaveScreen()
		{
			Desactivate();
			//tween et quand fini :
			OnScreenClosed?.Invoke(this);
		}
	}
}
