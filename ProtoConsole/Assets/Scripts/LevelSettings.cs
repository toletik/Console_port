using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Level")]
public class LevelSettings : ScriptableObject
{
    [Header("Planet")]
    [SerializeField] private Vector3 gravityCenter = Vector3.zero;
    [SerializeField] private float planetRadius = 15;

    [Header("Time")]
    [SerializeField] private int levelDurationInSeconds = 120;
    [SerializeField] private float respawnPlayerCooldownInSeconds = 4;

    private float movingPlanetRadiusOffset = 0;



    //Getter
    public Vector3 GravityCenter => gravityCenter;
    public float PlanetRadius => planetRadius + movingPlanetRadiusOffset;

    /// <summary> Duration of the level in seconds </summary>
    public int LevelDuration => levelDurationInSeconds;

    /// <summary> Respawn duration in seconds </summary>
    public float RespawnPlayerCooldownDuration => respawnPlayerCooldownInSeconds;

    public float MovingPlanetRadiusOffset
    {
        set
        {
            movingPlanetRadiusOffset = value;
        }
    }
}
