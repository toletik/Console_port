using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public delegate void PlayerEventHandler(Player player);
    public static event PlayerEventHandler OnPause;

    [Header("References")]
    [SerializeField] private LevelSettings levelSettings = default;
    [SerializeField] private new Rigidbody rigidbody = default;
    [SerializeField] private CapacityRenderer capacityConeSpawner = default;
    [SerializeField] private Transform capacityRenderersContainer = default;

    [Header("Parameters")]
    [SerializeField] private float speed = 1;

    [Header("Capacities")]
    [SerializeField] private Jump jumpCapacity = default;
    [SerializeField] private Dash dashCapacity = default;
    [SerializeField] private Dig digCapacity = default;

    [HideInInspector] public float AltitudeModifier = 0;
    [HideInInspector] public bool CanAddAltitudeModifier = true;
    [HideInInspector] public float MovementControlCoef = 1;
    [HideInInspector] public Vector2 ExternalVelocity = Vector2.zero;

    private Capacity currentCapacityUsed = Capacity.NONE;

    private Vector2 inputs = Vector2.zero;
    private Vector3 gravityCenter = default;

    private void Awake()
    {
        gravityCenter = levelSettings.GravityCenter;

        dashCapacity.enabled = false;
        jumpCapacity.enabled = false;
        digCapacity.enabled = false;
    }

    #region Movement
    public void UpdateMoveInputs(InputAction.CallbackContext callback)
    {
        inputs = callback.ReadValue<Vector2>();
    }

    public void FixedUpdate()
    {
        rigidbody.position += (transform.right * (inputs.x * MovementControlCoef + ExternalVelocity.x) + transform.forward * (inputs.y * MovementControlCoef + ExternalVelocity.y)) * (speed * Time.fixedDeltaTime);
        rigidbody.position = gravityCenter + (rigidbody.position - gravityCenter).normalized * (levelSettings.PlanetRadius + AltitudeModifier);

        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;
        rigidbody.rotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.right).normalized) * rigidbody.rotation;

        AltitudeModifier = 0;
        ExternalVelocity = Vector3.zero;
    }
    #endregion

    #region Capacities
    public bool TryAddCapacity(Capacity type, Direction dashDirection = default)
    {
        if (ActivateCapacityIfNew(type, dashDirection))
        {
            capacityConeSpawner.GetRendererForCapacity(type, capacityRenderersContainer).localRotation = Quaternion.Euler(type switch
            {
                Capacity.JUMP => Vector3.zero,
                Capacity.DASH => new Vector3 (90, 0, (int)dashDirection),
                Capacity.DIG => new Vector3 (180, 0, 0),
                _ => Vector3.zero
            });

            return true;
        }

        return false;
    }

    private bool ActivateCapacityIfNew(Capacity type, Direction dashDirection = default)
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

    public bool IsUsingCapacity(Capacity capacity)
    {
        if (capacity == currentCapacityUsed) return true;
        else if (currentCapacityUsed == Capacity.DASH_AND_JUMP && (capacity == Capacity.JUMP || capacity == Capacity.DASH)) return true;
        else return false;
    }

    public void StartCapacity(Capacity capacity)
    {
        currentCapacityUsed = (Capacity)((int)currentCapacityUsed + (int)capacity);
    }

    public void EndCapacity(Capacity capacity)
    {
        currentCapacityUsed = (Capacity)((int)currentCapacityUsed - (int)capacity);
    }
    #endregion

    public void ClickOnPause(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            Debug.Log(gameObject.name + " : Click to pause/unpause the game");
            OnPause?.Invoke(this);
        }
    }
}