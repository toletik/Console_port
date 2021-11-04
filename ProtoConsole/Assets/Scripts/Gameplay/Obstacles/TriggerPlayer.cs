using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerPlayer : MonoBehaviour
{
    [SerializeField] private float ejectForce = 0.5F;
    [SerializeField] private ParticleSystem onHitParticule = default;

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeScale = 0.4f;

    static IEnumerator StartScreenShake(float shakeDuration, float shakeScale)
    {
        Vector3 camDefaultPos = Camera.main.transform.localPosition;
        for (float timer = 0; timer < shakeDuration; timer += Time.deltaTime)
        {
            Camera.main.transform.localPosition = camDefaultPos + UnityEngine.Random.insideUnitSphere * shakeScale;
            yield return null;
        }
        Camera.main.transform.localPosition = camDefaultPos;
    }

    public static event Action<Player> OnCollision;

    private const string PLAYER_TAG = "Player";
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.CompareTag(PLAYER_TAG))
        {
            
            if(player.Eject(-transform.up, ejectForce))
            {
                OnCollision?.Invoke(player);
                onHitParticule.transform.position = player.transform.position;
                onHitParticule.Play();

                StartCoroutine(StartScreenShake(shakeDuration, shakeScale));
            }
        }

    }
}
