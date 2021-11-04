using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private Animator animator = default;
    [SerializeField] private float lerpDuration = 4;
    [Space(8)]
    [SerializeField] private float fullPlayerEjectionForce = 0.3f;

    public delegate void CollectibleEventHandler(Collectible collectible);
    static public event CollectibleEventHandler OnCollect;

    private List<Collectible> list = new List<Collectible>();

    private Vector3 planetPos = default;
    private Vector3 originalPos = default;
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
            if (player.CollectCapacityToAssign())
            {
                OnCollect?.Invoke(this);
                Destroy(gameObject);
            }
            else player.Eject(player.transform.position - transform.position, fullPlayerEjectionForce);
        }
        else 
            Debug.LogWarning("Invalid recuperation");
    }

    private IEnumerator GravityCoroutine()
    {
        RaycastHit hit;

        for (float elapsedTime = 0f; elapsedTime < lerpDuration; elapsedTime += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(originalPos, planetPos, elapsedTime / lerpDuration);

            if (Physics.Raycast(transform.position, -transform.up, out hit, 1) && hit.transform.gameObject.CompareTag(SPHERE_TAG))
            {
                transform.SetParent(hit.collider.gameObject.transform);
                Landing();
            }
            
            yield return null;
        }
    }

    private void Landing()
    {
        StopAllCoroutines();
        animator.SetTrigger("Land");
        SetAnimFalling(false);
    }

    public void SetModeFalling()
    {
        SetAnimFalling(true);
    }

    private void SetAnimFalling(bool value)
    {
        animator.SetBool("Falling", value);
    }
}
