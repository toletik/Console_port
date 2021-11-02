using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float ejectForce = 2;
    private const string PLAYER_TAG = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG)) 
            other.GetComponent<Player>().Eject(-transform.up, ejectForce);
    }
}
