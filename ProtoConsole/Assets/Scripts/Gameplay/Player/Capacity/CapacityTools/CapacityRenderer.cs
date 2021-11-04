using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Capacities")]
public class CapacityRenderer : ScriptableObject
{
    [SerializeField] private GameObject capacityConePrefab = default;
    [SerializeField] private AnimationCurve colorFeedbackForCooldown = default;

    [Header("Capacity Materials")]
    [SerializeField] private Material dashMaterial = default;
    [SerializeField] private Material jumpMaterial = default;
    [SerializeField] private Material digMaterial = default;
    [SerializeField] private Material noneMaterial = default;

    [Header("Capacity used color")]
    [SerializeField] private Color dashMaterialUsed = Color.black;
    [SerializeField] private Color jumpMaterialUsed = Color.black;
    [SerializeField] private Color digMaterialUsed = Color.black;

    public AnimationCurve ColorInterpolationCurveForCooldown => colorFeedbackForCooldown;

    public Transform CreateRendererForCapacity(Capacity capacity, Transform parent)
    {
        GameObject cone = Instantiate(capacityConePrefab, parent);

        if (capacity != Capacity.NONE)
        {
            cone.GetComponentInChildren<MeshRenderer>().material = capacity switch
            {
                Capacity.JUMP => jumpMaterial,
                Capacity.DASH => dashMaterial,
                Capacity.DIG => digMaterial,
                _ => noneMaterial
            };
        }
        
        return cone.transform;
    }

    public Color GetUsedMaterialColorForCapacity(Capacity capacity)
    {
        return capacity switch
        {
            Capacity.DIG => digMaterialUsed,
            Capacity.DASH => dashMaterialUsed,
            Capacity.JUMP => jumpMaterialUsed,
            _ => Color.black
        };
    }
}
