using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float lerpDuration = 4;
    [SerializeField] private ParticleSystem landingParticule = default;
    public delegate void CollectibleEventHandler(Collectible collectible);
    static public event CollectibleEventHandler OnCollect;

    private List<Collectible> list = new List<Collectible>();

    private Vector3 planetPos = default;
    private Vector3 originalPos = default;
    private float elapsedTime = default;
    private const string SPHERE_TAG = "Sphere";

    private void Start()
    {
        list = CollectibleManager.collectibles;
        planetPos = CollectibleManager.planetPos.position;
        StartGravity();
    }

    private void StartGravity()
    {
        originalPos = transform.position;
        StartCoroutine(GravityCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.CollectCapacityToAssign();
            OnCollect?.Invoke(this);
            Destroy(gameObject);
        }
        else
            Debug.LogWarning("Invalid recuperation");
        
    }

    private IEnumerator GravityCoroutine()
    {
        RaycastHit hit;
        while (elapsedTime< lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(originalPos, planetPos, elapsedTime / lerpDuration);
            
            if( Physics.Raycast(transform.position,-transform.up,out hit, 1)&& hit.transform.gameObject.CompareTag(SPHERE_TAG)){

                Landing();
            }
            yield return null;
        }
       
    }
    private void Landing()
    {
        StopAllCoroutines();
        landingParticule.Play();
    }


}
