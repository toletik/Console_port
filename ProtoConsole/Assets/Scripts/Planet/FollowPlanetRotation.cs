using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlanetRotation : MonoBehaviour
{
    [SerializeField] private Rigidbody objectToManage = default;
    [SerializeField] private RotationForPlanetPart rotationInfos = default;
    [SerializeField] private string groundTag = "Sphere";

    private List<Transform> overlapingPlanetParts = new List<Transform>();

    private Coroutine followCoroutineCurrentlyRunning = null;
    private bool modeFollow = false;

    private int planetPartsOn = 0;
    public bool IsOnPlanet
    {
        get { return planetPartsOn > 0; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(groundTag) && !overlapingPlanetParts.Contains(other.transform))
        {
            planetPartsOn += 1;
            overlapingPlanetParts.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (overlapingPlanetParts.Contains(other.transform))
        {
            overlapingPlanetParts.Remove(other.transform);
        }

        if (other.CompareTag(groundTag))
        {
            planetPartsOn -= 1;
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

    private IEnumerator FollowPlanet()
    {
        while(modeFollow)
        {
            if (overlapingPlanetParts.Count > 0) 
                objectToManage.position = rotationInfos.GetOrbitalPositionRotatedWithPlanet(overlapingPlanetParts[overlapingPlanetParts.Count - 1], objectToManage.position);
            
            yield return null;
        }

        yield break;
    }
}
