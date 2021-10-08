using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private const string PLAYER_TAG = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG)) other.GetComponent<Player>().Eject(-transform.up,2) ;
    }
}
