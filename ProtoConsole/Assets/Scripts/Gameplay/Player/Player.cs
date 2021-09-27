using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelSettings levelSettings = default;
    [SerializeField] private new Rigidbody rigidbody = default;

    [SerializeField] private float speed = 1;

    private Vector3 gravityCenter = default;

    public void Awake()
    {
        gravityCenter = levelSettings.GravityCenter;
    }

    public void FixedUpdate()
    {
        rigidbody.position += (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * speed;
        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;
    }
}