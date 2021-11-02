using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Camera cameraTest = default;
    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG)) other.GetComponent<Player>().Die();
    }
}
