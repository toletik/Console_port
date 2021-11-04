using System.Collections.Generic;
using UnityEngine;

public static class RandomPositionOnPlanetZone
{
    public static List<Vector3> GeneratePositions(uint numberOfPositions, Vector3 axis, Vector3 planetCenter, float planetRadius, float maxAccessibleZoneAngle, float minAccessibleZoneAngle, float positionDelta)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 toRadiusPosition = Quaternion.AngleAxis(maxAccessibleZoneAngle, Vector3.Cross(axis, new Vector3(axis.x, 0, 0))) * (axis.normalized * planetRadius);

        if (numberOfPositions == 0) return positions;

        float currentAngle = Random.Range(0, 360);
        float deltaAngle = 360 / numberOfPositions;

        Vector3 position;

        for (int i = 0; i < numberOfPositions; i++)
        {
            position = Quaternion.AngleAxis(currentAngle, axis) * toRadiusPosition;
            if (minAccessibleZoneAngle < maxAccessibleZoneAngle) position = Vector3.Slerp(axis.normalized * planetRadius, position, Random.Range(minAccessibleZoneAngle / maxAccessibleZoneAngle, 1));
            position = Quaternion.AngleAxis(positionDelta, Vector3.Cross(position, new Vector3(0, 0, position.z))) * position;
            
            positions.Add(position + planetCenter);

            currentAngle += deltaAngle;
        }

        return positions;
    }
}
