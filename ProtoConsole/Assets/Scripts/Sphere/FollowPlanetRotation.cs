using System.Collections;
using UnityEngine;

public class FollowPlanetRotation : MonoBehaviour
{
    [SerializeField] private Transform objectToManage = default;
    [SerializeField] private RotationForPlanetPart rotationInfos = default;
    [SerializeField] private string groundTag = "Sphere";

    private Transform onPlanetPart = default;
    private Coroutine followCoroutineCurrentlyRunning = default;

    private void Awake()
    {
        Sphere.OnResetRotation += Sphere_OnResetRotation;
    }

    private void Sphere_OnResetRotation()
    {
        StartCoroutine(PauseFollowPlanetForFrame());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("<-- ENTER : " + other.name);        

        if (other.CompareTag(groundTag))
        {
            onPlanetPart = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(" --> EXIT : " + other.name);

        if (other.transform == onPlanetPart)
        {
            onPlanetPart = null;
        }
    }

    public void StartToFollowPlanet()
    {
        if (followCoroutineCurrentlyRunning == null) followCoroutineCurrentlyRunning = StartCoroutine(FollowPlanet());
    }

    public void StopFollowingPlanet()
    {
        if (followCoroutineCurrentlyRunning != null) StopCoroutine(followCoroutineCurrentlyRunning);
    }

    private IEnumerator PauseFollowPlanetForFrame()
    {
        StopFollowingPlanet();
        yield return null;

        StartToFollowPlanet();
        yield break;
    }

    private IEnumerator FollowPlanet()
    {
        while(onPlanetPart)
        {
            Debug.Log("Rotaaaate");
            objectToManage.position = rotationInfos.GetOrbitalPositionRotatedWithPlanet(onPlanetPart, objectToManage.position);
            yield return null;
        }

        yield break;
    }

    private void OnDestroy()
    {
        Sphere.OnResetRotation -= Sphere_OnResetRotation;
    }
}
