using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Debug.Log("Capacity taken !");

            player.CollectCapacityToAssign();
            Destroy(gameObject);
        }
        else
            Debug.LogWarning("Invalid recuperation");
    }
}
