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
    [SerializeField] private GameObject bestPlayerFeedback = default;

    private Transform cameraTransform = default;

    private void Awake()
    {
        LevelManager.OnLevelDestroy += LevelManager_OnLevelDestroy; 
        LevelManager.OnLevelSpawn += LevelManager_OnLevelSpawn;

        gameObject.SetActive(false);
    }

    private void LevelManager_OnLevelSpawn(LevelManager obj)
    {
        gameObject.SetActive(true);
        cameraTransform = Camera.main.transform;
    }

    private void LevelManager_OnLevelDestroy(LevelManager obj)
    {
        gameObject.SetActive(false);
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

    public void ActivateBestScore() => bestPlayerFeedback.SetActive(true);
    public void DesativateBestScore() => bestPlayerFeedback.SetActive(false);

    private void OnDestroy()
    {
        LevelManager.OnLevelDestroy -= LevelManager_OnLevelDestroy;
        LevelManager.OnLevelSpawn -= LevelManager_OnLevelSpawn;
    }
}