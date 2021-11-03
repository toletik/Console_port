using System.Collections.Generic;
using UnityEngine;

public static class RandomPositionOnPlanetZone
{
    public static List<Vector3> GeneratePositions(uint numberOfPositions, Vector3 axis, Vector3 planetCenter, float planetRadius, float accessibleZoneAngle, float positionDelta, bool insideZone)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 toRadiusPosition = Quaternion.AngleAxis(accessibleZoneAngle, Vector3.Cross(axis, new Vector3(axis.x, 0, 0))) * axis;

        float currentAngle = Random.Range(0, 360);
        float deltaAngle = 360 / numberOfPositions;

        Quaternion.AngleAxis(currentAngle, axis);

        Vector3 position;

        for (int i = 0; i < numberOfPositions; i++)
        {
            position = Quaternion.AngleAxis(currentAngle, axis) * toRadiusPosition;

            if (insideZone) 
                position = Quaternion.AngleAxis(currentAngle, Vector3.Cross(axis, position)) * toRadiusPosition;

            
            positions.Add(position);

            currentAngle += deltaAngle;
        }

        return positions;
    }
}
