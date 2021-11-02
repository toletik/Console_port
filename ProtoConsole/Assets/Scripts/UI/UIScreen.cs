///-----------------------------------------------------------------
/// Author : Joéva FOMBARON
/// Date : 11/11/2020 13:05
///-----------------------------------------------------------------

using UnityEngine;

namespace Com.IsartDigital.Common.UI
{
	public class UIScreen : MonoBehaviour
	{
		public delegate void UIScreenEventHander(UIScreen screen);
		public static event UIScreenEventHander OnScreenClosed;

		virtual public void Init()
		{

		}

		virtual public void EnterScreen()
		{
			//tween et quand fini, activate
			Activate();
		}

		virtual protected void Activate()
		{

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
