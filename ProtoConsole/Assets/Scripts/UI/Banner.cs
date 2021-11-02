using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Banner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textfield = default;
    [SerializeField] private UnityEvent callOnEnd = default;

    public event UnityAction OnAnimationEnd
    {
        add => callOnEnd.AddListener(value);
        remove => callOnEnd.RemoveListener(value);
    }

    public void PlayBanner(string text)
    {
        textfield.text = text;
        GetComponent<Animator>().enabled = true;
    }

    public void EndBanner()
    {
        callOnEnd?.Invoke();
        GetComponent<Animator>().enabled = false;
    }
}
