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

    [SerializeField] private float radiusOnDeath = 5;
    


    static public List<Collectible> collectibles = new List<Collectible>();
    void Start()
    {
       
        //LoseCollectibleWhenDead(debugTransform, 2);
        SpawnCollectibleRandomlyOnSphere(100);
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
            newPos =   Quaternion.FromToRotation(Vector3.up, -Camera.main.transform.forward) *(newPos - Vector3.zero);



            Collectible collectible = Instantiate(prefabCollectible, newPos, Quaternion.identity);
            collectible.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(planetOrigin.up, newPos), Vector3.Cross( planetOrigin.up - planetOrigin.position,newPos - planetOrigin.position)) * collectible.transform.rotation;
            collectibles.Add(collectible);
        }        
    }
    private void LoseCollectibleWhenDead(Transform playerTransform,int nbrOfCollectible)
    {
        float angle;
        float ratio;
        Vector3 newPos = default;

        for (int i = 0; i < nbrOfCollectible; i++)
        {
            ratio = (float)i / nbrOfCollectible;

            angle = 2 * Mathf.PI * ratio;



            newPos  = new Vector3(radiusOnDeath * Mathf.Cos(angle),
                0,
                radiusOnDeath * Mathf.Sin(angle))+playerTransform.position;

            CreateCollectible(prefabCollectible, newPos);


        }

    }
    private void CreateCollectible(Collectible prefab,Vector3 newPos)
    {
        Collectible collectible = Instantiate(prefab, newPos, Quaternion.identity);
        collectibles.Add(collectible);
    }
    private void CreateCollectible(Collectible prefab, Vector3 newPos,Transform parent)
    {
        Collectible collectible = Instantiate(prefab, newPos, Quaternion.identity,parent);
        collectibles.Add(collectible);
    }
    private void OnDestroy()
    {
        Collectible.OnCollect -= Collectible_OnCollect;
    }



}
