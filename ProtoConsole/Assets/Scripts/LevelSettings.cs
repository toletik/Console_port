using UnityEngine;


[CreateAssetMenu(menuName = "Settings/Level")]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private Vector3 gravityCenter = Vector3.zero;

    public Vector3 GravityCenter => gravityCenter;
}
