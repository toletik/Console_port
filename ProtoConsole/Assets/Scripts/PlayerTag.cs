using TMPro;
using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private Transform target = default;
    [SerializeField] private Vector3 offset = Vector3.up;

    [Space(8)]
    [SerializeField] private TextMeshPro textfield = default;
    [SerializeField] private SpriteRenderer arrow = default;

    private Transform cameraTransform = default;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.position = target.position + offset;
        transform.LookAt(2 * transform.position - cameraTransform.position, cameraTransform.up);
    }

    public void DisplayPlayer(string text, Color color, bool updateArrowColor = true)
    {
        DisplayPlayer(text);

        textfield.color = color;
        if (updateArrowColor) arrow.color = color;
    }

    public void DisplayPlayer(string text)
    {
        textfield.text = text;
    }
}