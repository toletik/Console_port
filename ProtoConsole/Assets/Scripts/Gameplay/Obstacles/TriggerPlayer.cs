using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerPlayer : MonoBehaviour
{
    [SerializeField] private float ejectForce = 0.5F;
    [SerializeField] private ParticleSystem onHitParticule = default;

    public static event Action<Player> OnCollision;

    private const string PLAYER_TAG = "Player";
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.CompareTag(PLAYER_TAG))
        {
            player.Eject(-transform.up, ejectForce);
            OnCollision?.Invoke(player);
            onHitParticule.transform.position = player.transform.position;
            onHitParticule.Play();
        }

    }
}
