using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [SerializeField] static public Transform planetPos = default;
    [SerializeField] public Transform level = default;
    [SerializeField] public Transform planetOrigin = default;
    [SerializeField] private Collectible prefabCollectible = default;
    [SerializeField] private LevelSettings levelSettings = default;

    [Header("Collectible Spawn")]
    [SerializeField] private int startCollectible = 8;
    [SerializeField] private int minNbrOfCollectible = 2;
    [SerializeField] private float collectibleSpawnZoneAngle = 50;
    [SerializeField] private float collectibleMinSpawnZoneAngle = 20;
    [SerializeField] private float collectibleSpawnPositionDelta = 1.5f;
    [SerializeField] private float radiusOnDeath = 5;

    static public List<Collectible> collectibles = new List<Collectible>();

    void Start()
    {
        planetPos = planetOrigin;
        SpawnCollectibleRandomlyOnSphere(startCollectible);

        Collectible.OnCollect += Collectible_OnCollect;
    }

    private void Collectible_OnCollect(Collectible collectible)
    {
        if (collectibles.Contains(collectible))
            collectibles.Remove(collectible);

        CheckCollectibles();
    }

    private void CheckCollectibles()
    {
        if (collectibles.Count <= minNbrOfCollectible) 
            SpawnCollectibleRandomlyOnSphere(1);
    }

    private void SpawnCollectibleRandomlyOnSphere(int nbrCollectible)
    {
        List<Vector3> randomPositions = RandomPositionOnPlanetZone.GeneratePositions((uint)nbrCollectible, -Camera.main.transform.forward, levelSettings.GravityCenter, levelSettings.PlanetRadius, collectibleSpawnZoneAngle, collectibleMinSpawnZoneAngle, collectibleSpawnPositionDelta);
        Vector3 newPos;

        for (int i = 0; i < nbrCollectible; i++)
        {
            newPos = randomPositions[i];

            Collectible collectible = Instantiate(prefabCollectible, newPos, Quaternion.identity,level);
            collectible.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(planetOrigin.up, newPos - levelSettings.GravityCenter), Vector3.Cross(planetOrigin.up - planetOrigin.position, newPos - planetOrigin.position)) * collectible.transform.rotation;
            collectibles.Add(collectible);
        }        
    }

     public void LoseCollectibleWhenDead(Transform playerTransform,int nbrOfCollectible)
    {
        float angle;

        for (int i = 0; i < nbrOfCollectible; i++)
        {
            angle = Random.Range(0, 2 * Mathf.PI );
            CreateCollectibleInTheAir(new Vector3(radiusOnDeath * Mathf.Cos(angle), 0, radiusOnDeath * Mathf.Sin(angle)) + playerTransform.position);
        }
    }

    private void CreateCollectibleInTheAir(Vector3 newPos)
    {
        Collectible collectible = Instantiate(prefabCollectible, newPos, Quaternion.identity,level) ;

        collectibles.Add(collectible);

        collectible.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(planetOrigin.up, newPos), Vector3.Cross(planetOrigin.up - planetOrigin.position, newPos - planetOrigin.position)) * collectible.transform.rotation;
        collectible.SetModeFalling();
    }

    private void OnDestroy()
    {
        Collectible.OnCollect -= Collectible_OnCollect;
    }
}
