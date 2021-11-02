///-----------------------------------------------------------------
/// Author : Joéva FOMBARON
/// Date : 11/11/2020 15:01
///-----------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Common.UI 
{
	public class UIManager : MonoBehaviour {
        public static UIManager Instance { get; private set; }

        [SerializeField] private List<GameObject> allScreens = default;
		[SerializeField] private UIScreen startScreen = default;

		private List<GameObject> closedChecklist = new List<GameObject>();
		private GameObject waitingToBeAdded = null;

		private void Awake()
		{
			if (Instance){
				Destroy(gameObject);
				return;
			}
			
			Instance = this;

			UIScreen.OnScreenClosed += UIScreen_OnScreenClosed;

			foreach (GameObject screen in allScreens) {
				screen.SetActive(false);
				screen.GetComponent<UIScreen>().Init();
			}

			if (startScreen) AddScreen(startScreen.gameObject);
		}

        #region Open screen
        public void AddScreen<T>(bool closeAllOtherScreensBefore = false) where T : UIScreen
		{
			if (closeAllOtherScreensBefore)
			{
				closedChecklist = new List<GameObject>();

				foreach (GameObject screen in allScreens) {
					if (screen.activeSelf) closedChecklist.Add(screen);
				}

				if (closedChecklist.Count > 0)
				{
					waitingToBeAdded = GetScreen<T>();
					CloseAllScreensFromList(closedChecklist);
					return;
				}
			}
			
			AddScreen<T>();
		}

		/// <summary> Close the given screen if needed and add the other when it's done </summary>
		/// <typeparam name="T1">Screen to add</typeparam>
		/// <typeparam name="T2">Screen to close</typeparam>
		public void AddScreen<T1, T2>() where T1 : UIScreen where T2 : UIScreen
		{
			GameObject screenToClose = GetScreen<T2>();

			if (screenToClose != null && screenToClose.activeSelf)
			{
				waitingToBeAdded = GetScreen<T1>();
				closedChecklist = new List<GameObject>() { screenToClose };

				CloseScreen(screenToClose);
			}
			else AddScreen<T1>();
		}

		private void AddScreen<T>() where T : UIScreen
		{
			AddScreen(GetScreen<T>());
		}

		private void AddScreen(GameObject screen)
		{
			if (screen != null && !screen.activeSelf)
			{
				screen.SetActive(true);
				screen.GetComponent<UIScreen>().EnterScreen();
			}
		}
        #endregion

        #region Close screen
        public void CloseScreen<T>() where T : UIScreen
		{
			CloseScreen(GetScreen<T>());
		}

		private void CloseScreen(GameObject screen)
		{
			if (screen != null && screen.activeSelf)
				screen.GetComponent<UIScreen>().LeaveScreen();
		}

		private void CloseAllScreensFromList(List<GameObject> screensToClose)
		{
			if (screensToClose == null) return;

			for (int i = screensToClose.Count - 1; i >= 0; i--) {
				CloseScreen(screensToClose[i]);
			}
		}
        #endregion

        public GameObject GetScreen<T>() where T : UIScreen
		{
			Component screenScriptComponent;

			for (int i = 0; i < allScreens.Count; i++)
			{
				if (allScreens[i].TryGetComponent(typeof(T), out screenScriptComponent))
					return screenScriptComponent.gameObject;
			}

			return null;
		}

		private void UIScreen_OnScreenClosed(UIScreen screen)
		{
			screen?.gameObject.SetActive(false);

			if (closedChecklist.Contains(screen.gameObject)) 
			{
				closedChecklist.Remove(screen.gameObject);

				if (closedChecklist.Count == 0) 
				{
					AddScreen(waitingToBeAdded);
					waitingToBeAdded = null;
				}
			}
		}

		private void OnDestroy()
		{
			if (this == Instance) Instance = null;
			UIScreen.OnScreenClosed -= UIScreen_OnScreenClosed;
		}
	}
}