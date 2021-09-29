using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Capacities")]
public class CapacityRenderer : ScriptableObject
{
    [SerializeField] private GameObject capacityConePrefab = default;

    [Header("Materials")]
    [SerializeField] private Material dashMaterial = default;
    [SerializeField] private Material jumpMaterial = default;
    [SerializeField] private Material digMaterial = default;

    public Transform GetRendererForCapacity(Capacity capacity)
    {
        GameObject cone = Instantiate(capacityConePrefab);

        cone.GetComponent<MeshRenderer>().material = capacity switch
        {
            Capacity.JUMP => jumpMaterial,
            Capacity.DASH => dashMaterial,
            Capacity.DIG => digMaterial,
            _ => cone.GetComponent<MeshRenderer>().material,
        };

        return cone.transform;
    }
}
