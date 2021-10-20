using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform planetOrigin = default;
    [SerializeField] private Collectible prefabCollectible = default;

    [SerializeField] private int startCollectible = 0;
    [SerializeField] private int minNbrOfCollectible = 2;

    static public List<Collectible> collectibles = new List<Collectible>();
    void Start()
    {
        SpawnCollectibleRandomlyOnSphere(startCollectible);
        Collectible.OnCollect += Collectible_OnCollect;
    }

    private void Collectible_OnCollect(Collectible collectible)
    {
        collectibles.RemoveAt(collectibles.IndexOf(collectible));
        CheckCollectibles();
    }

    private void CheckCollectibles()
    {
        int currentCollectible = collectibles.Count;

        if(currentCollectible <= minNbrOfCollectible) SpawnCollectibleRandomlyOnSphere(1);
    }

    private void SpawnCollectibleRandomlyOnSphere(int nbrCollectible)
    {
        Vector3 newPos = default;
      
        for (int i = 0; i < nbrCollectible; i++)
        {
            newPos = Random.onUnitSphere * 13;
            newPos.y = Mathf.Abs(newPos.y);
      
            Collectible collectible = Instantiate(prefabCollectible, newPos, Quaternion.identity);
            collectible.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(planetOrigin.up, newPos), Vector3.Cross( planetOrigin.up - planetOrigin.position,newPos - planetOrigin.position)) * collectible.transform.rotation;
            collectibles.Add(collectible);
        }        
    }
    private void OnDestroy()
    {
        Collectible.OnCollect -= Collectible_OnCollect;
    }



}
