using System.Collections;
using UnityEngine;

public class ChangeHeightCapacity : MonoBehaviour
{
    [SerializeField] private float movementHeight = 3;
    [SerializeField] private float movementDuration = 0.8f;
    [SerializeField] private AnimationCurve movementCurve = default;

    [Header("Controls")]
    [SerializeField] protected KeyCode activateKey = KeyCode.Keypad1;

    private Player player = default;
    private Coroutine currentMovement = null;

    private void OnEnable()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKey) && player.CanAddAltitudeModifier) 
        {
            player.CanAddAltitudeModifier = false; 
            currentMovement = StartCoroutine(UpdateHeight()); 
        }
    }

    private IEnumerator UpdateHeight()
    {
        float elapsedTime = 0;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            player.AltitudeModifier = Mathf.LerpUnclamped(0, movementHeight, movementCurve.Evaluate(elapsedTime / movementDuration));

            yield return null;
        }

        player.AltitudeModifier = 0;
        currentMovement = null;

        player.CanAddAltitudeModifier = true;

        yield break;
    }
}