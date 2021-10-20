using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Planet Parts Rotation Infos")]
public class RotationForPlanetPart : ScriptableObject
{
    private Dictionary<Transform, Quaternion> planetPartToRotation = default;
    private Vector3 planetCenter = Vector3.zero;

    public void InitLevelValues(Vector3 planetCenter, bool clearPreviousValues = true)
    {
        this.planetCenter = planetCenter;
        if (clearPreviousValues) planetPartToRotation = new Dictionary<Transform, Quaternion>();
    }

    public void UpdateFrameRotationForPart(Transform part, Quaternion rotation)
    {
        if (planetPartToRotation.ContainsKey(part)) planetPartToRotation[part] = rotation;
        else planetPartToRotation.Add(part, rotation);
    }

    public Vector3 GetOrbitalPositionRotatedWithPlanet(Transform planetPartToFollow, Vector3 position)
    {
        if (planetPartToRotation.ContainsKey(planetPartToFollow))
        {
            return planetPartToRotation[planetPartToFollow] * (position - planetCenter);
        }
        else return position;
    }
}