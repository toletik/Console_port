using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelSettings levelSettings = default;
    [SerializeField] private new Rigidbody rigidbody = default;

    [SerializeField] private float speed = 1;

    private Vector3 gravityCenter = default;

    private void Awake()
    {
        gravityCenter = levelSettings.GravityCenter;
    }
    
    public void FixedUpdate()
    {
        rigidbody.position += (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * speed * Time.fixedDeltaTime;
        rigidbody.position = gravityCenter + (rigidbody.position - gravityCenter).normalized * 12.5f;

        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;
        rigidbody.rotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.right).normalized) * rigidbody.rotation;
    }
}