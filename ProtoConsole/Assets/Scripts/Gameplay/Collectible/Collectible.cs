using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public delegate void CollectibleEventHandler(Collectible collectible);
    static public event CollectibleEventHandler OnCollect;
    private List<Collectible> list = new List<Collectible>();

    private void Start()
    {
        list = CollectibleManager.collectibles;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Debug.Log("Capacity taken !");

            player.CollectCapacityToAssign();
            OnCollect?.Invoke(this);
            Destroy(gameObject);
        }
        else Debug.Log("Invalid recuperation");
    }
}
