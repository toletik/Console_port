using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    
    [SerializeField] private Transform killZone = default;
    [SerializeField] private LevelSettings settings = default;

    [SerializeField] private float moveToPlayerDuration = 3;
    [SerializeField] private float timeScaleOnStagger = 0.05f;

    private Camera camera = default;
    void Start()
    {
        killZone.position = settings.GravityCenter + (settings.PlanetRadius*transform.forward);
        
        killZone.rotation = Quaternion.LookRotation( transform.forward);
        TriggerPlayer.OnCollision += TriggerPlayer_OnCollision;
        camera = GetComponent<Camera>();
    }

    private void TriggerPlayer_OnCollision(Player player)
    {
        StartCoroutine(StaggerCoroutine(player));
        Time.timeScale = timeScaleOnStagger;
        Debug.Log("Stagger");
    }
    private IEnumerator StaggerCoroutine(Player player)
    {
        Quaternion baseRotation = transform.rotation;
        Quaternion gotoRotation = Quaternion.LookRotation(player.transform.position-transform.position);
        float elapsedTime = 0;

        while (elapsedTime<moveToPlayerDuration)
        {
            elapsedTime += Time.fixedTime;
            //transform.rotation = Quaternion.Lerp(baseRotation, gotoRotation, elapsedTime/moveToPlayerDuration);
            //camera.fieldOfView = Mathf.Lerp(90, 60, elapsedTime / moveToPlayerDuration);
           
            yield return new WaitForFixedUpdate();
        }
        elapsedTime = 0;
        while (elapsedTime < moveToPlayerDuration)
        {
            elapsedTime += Time.fixedTime;
          

            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1;
        elapsedTime = 0;
        while (elapsedTime < moveToPlayerDuration)
        {
            Debug.Log("CEST LA COROUTINE MAIS LA 2EME PAS LA 1ER ");
            elapsedTime += Time.fixedTime;
           // transform.rotation = Quaternion.Lerp(gotoRotation, baseRotation, elapsedTime / moveToPlayerDuration);
            //camera.fieldOfView = Mathf.Lerp(60, 90, elapsedTime / moveToPlayerDuration);

            yield return new WaitForFixedUpdate();
        }
    }
    private void OnDestroy()
    {
        TriggerPlayer.OnCollision -= TriggerPlayer_OnCollision;
    }

}
