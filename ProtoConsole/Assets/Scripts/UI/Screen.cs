using System.Collections;
using UnityEngine;

public class Screen : MonoBehaviour
{
    public virtual void OpenScreen()
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseScreen()
    {
        gameObject.SetActive(false);
    }

    public virtual void LeaveButton()
    {

    }

	public virtual void PlayButton()
    {

	}
}
