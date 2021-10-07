using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Player Colors")]
public class PlayerColors : ScriptableObject
{
    [SerializeField] private List<Color> colors = default;

    public Color GetColorAtIndex(int index)
    {
        return index < colors.Count ? colors[index] : colors[index % colors.Count];
    }
}
