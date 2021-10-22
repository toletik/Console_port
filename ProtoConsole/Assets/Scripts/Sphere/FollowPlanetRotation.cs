using System.Collections;
using UnityEngine;

public class FollowPlanetRotation : MonoBehaviour
{
    [SerializeField] private Rigidbody objectToManage = default;
    [SerializeField] private RotationForPlanetPart rotationInfos = default;
    [SerializeField] private string groundTag = "Sphere";

    private Transform onPlanetPart = default;
    private Coroutine followCoroutineCurrentlyRunning = null;

    private bool modeFollow = false;

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
        modeFollow = true;
        if (followCoroutineCurrentlyRunning == null && enabled) followCoroutineCurrentlyRunning = StartCoroutine(FollowPlanet());
    }

    public void StopFollowingPlanet()
    {
        modeFollow = false;

        if (followCoroutineCurrentlyRunning != null)
        { 
            StopCoroutine(followCoroutineCurrentlyRunning);
            followCoroutineCurrentlyRunning = null;
        }
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
        while(modeFollow)
        {
            if (onPlanetPart) 
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
