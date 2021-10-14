using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Level")]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private Vector3 gravityCenter = Vector3.zero;
    [SerializeField] private float planetRadius = 15;
    [SerializeField] private int levelDurationInSeconds = 120;

    private float movingPlanetRadiusOffset = 0;

    public Vector3 GravityCenter => gravityCenter;
    public float PlanetRadius => planetRadius + movingPlanetRadiusOffset;

    /// <summary> Duration of the level in seconds </summary>
    public int LevelDuration => levelDurationInSeconds;

    public void SetPlanetMovingRadiusOffset(float distance)
    {
        movingPlanetRadiusOffset = distance;
    }
}
