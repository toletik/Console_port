using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Level")]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private Vector3 gravityCenter = Vector3.zero;
    [SerializeField] private float planetRadius = 15;

    private float movingPlanetRadiusOffset = 0;

    public Vector3 GravityCenter => gravityCenter;
    public float PlanetRadius => planetRadius + movingPlanetRadiusOffset;

    public void SetPlanetMovingRadiusOffset(float distance)
    {
        movingPlanetRadiusOffset = distance;
    }
}
