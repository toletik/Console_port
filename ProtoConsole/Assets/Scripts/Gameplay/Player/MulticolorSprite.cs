using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MulticolorSprite : MonoBehaviour
{
    [SerializeField] private Gradient colors = default;
    [SerializeField] private float speed = 1f;

    private new SpriteRenderer renderer = default;
    private float progression;

    private void Awake()
    {
        progression = Random.value;
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        progression += speed * Time.deltaTime;
        if (progression > 1) progression -= 1;

        renderer.color = colors.Evaluate(progression);
    }
}
