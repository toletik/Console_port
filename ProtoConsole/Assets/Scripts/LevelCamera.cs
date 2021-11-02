using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform killZone = default;
    [SerializeField] private LevelSettings settings = default;
    void Start()
    {
        killZone.position = settings.GravityCenter + (settings.PlanetRadius*transform.forward);
        Debug.Log(killZone.position);
        killZone.rotation = Quaternion.LookRotation( transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
