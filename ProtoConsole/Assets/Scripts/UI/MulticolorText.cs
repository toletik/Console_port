using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MulticolorText : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Gradient colors = default;
    [SerializeField] private float speed = 1f;

    private new TextMeshProUGUI renderer = default;
    private float progression;

    private Color defaultColor = default;
    
    private void Awake()
    {
        progression = Random.value;
        renderer = GetComponent<TextMeshProUGUI>();

        defaultColor = renderer.color;
    }

    void Update()
    {
        progression += speed * Time.deltaTime;
        if (progression > 1) progression -= 1;

        renderer.color = colors.Evaluate(progression);
    }

    private void OnDisable()
    {
        renderer.color = defaultColor;
    }
}
