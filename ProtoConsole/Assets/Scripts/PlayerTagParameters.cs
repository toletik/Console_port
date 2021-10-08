using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Player Tag")]
public class PlayerTagParameters : ScriptableObject
{
    [SerializeField] private string tagPrefix = "P";
    [SerializeField] private bool colorArrowUnderTag = true;
    [SerializeField] private List<Color> colors = default;

    public string TagPrefix => tagPrefix;

    public bool UpdateArrowColor => colorArrowUnderTag;

    public Color GetColorAtIndex(int index)
    {
        return index < colors.Count ? colors[index] : colors[index % colors.Count];
    }
}
