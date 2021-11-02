using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Banner : MonoBehaviour
{
    public static Banner Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI textfield = default;
    [SerializeField] private UnityEvent callOnEnd = default;

    public event UnityAction OnAnimationEnd
    {
        add => callOnEnd.AddListener(value);
        remove => callOnEnd.RemoveListener(value);
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
