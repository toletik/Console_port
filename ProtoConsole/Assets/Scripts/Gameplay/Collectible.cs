using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private Direction optionalDashDirection = default;

    public Capacity capacityType = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) && player.TryAddCapacity(capacityType, optionalDashDirection))
        {
            Debug.Log("Capacity taken !");
            Destroy(gameObject);
        }
        else Debug.Log("Invalid recuperation");
    }
}
