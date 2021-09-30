using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Capacities")]
public class CapacityRenderer : ScriptableObject
{
    [SerializeField] private GameObject capacityConePrefab = default;

    [Header("Materials")]
    [SerializeField] private Material dashMaterial = default;
    [SerializeField] private Material jumpMaterial = default;
    [SerializeField] private Material digMaterial = default;
    [SerializeField] private Material noneMaterial = default;

    public Transform GetRendererForCapacity(Capacity capacity, Transform parent)
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
}
