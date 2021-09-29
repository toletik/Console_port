using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private LevelSettings levelSettings = default;
    [SerializeField] private new Rigidbody rigidbody = default;

    [SerializeField] private float speed = 1;

    [Header("Capacities")]
    [SerializeField] private Jump jumpCapacity = default;
    [SerializeField] private Dash dashCapacity = default;
    [SerializeField] private Dig digCapacity = default;

    [HideInInspector] public float AltitudeModifier = 0;
    [HideInInspector] public bool CanAddAltitudeModifier = true;

    private Vector3 gravityCenter = default;

    private void Awake()
    {
        gravityCenter = levelSettings.GravityCenter;

        dashCapacity.enabled = false;
        jumpCapacity.enabled = false;
        digCapacity.enabled = false;
    }
    
    public void FixedUpdate()
    {
        rigidbody.position += (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * speed * Time.fixedDeltaTime;
        rigidbody.position = gravityCenter + (rigidbody.position - gravityCenter).normalized * (levelSettings.PlanetRadius + AltitudeModifier);

        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;
        rigidbody.rotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.right).normalized) * rigidbody.rotation;

        AltitudeModifier = 0;
    }

    public bool TryAddCapacity(Capacity type, Direction dashDirection = default)
    {
        switch (type)
        {
            case Capacity.JUMP:
                return jumpCapacity.enabled ? false : jumpCapacity.enabled = true;

            case Capacity.DASH:
                dashCapacity.enabled = true;
                return dashCapacity.TryAddDirection(dashDirection);                

            case Capacity.DIG:
                return digCapacity.enabled ? false : digCapacity.enabled = true;

            default:
                return false;
        }
    }
}