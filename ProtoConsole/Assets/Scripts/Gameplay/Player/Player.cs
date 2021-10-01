using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public delegate void PlayerEventHandler(Player player);
    public static event PlayerEventHandler OnPause;

    [Header("References")]
    [SerializeField] private LevelSettings levelSettings = default;
    [SerializeField] private new Rigidbody rigidbody = default;
    [SerializeField] private Transform capacityRenderersContainer = default;
    [SerializeField] private Material hasCapacityToAssignMaterial = default;

    [Header("Parameters")]
    [SerializeField] private float speed = 1;
    [SerializeField] private float ejectionOnPlayerContactStrenght = 0.3f;
    [SerializeField] private float ejectionDeceleration = 0.02f;
    [SerializeField] private float ejectionGravity = 0.05f;

    [Header("Capacities")]
    [SerializeField] private Jump jumpCapacity = default;
    [SerializeField] private Dash dashCapacity = default;
    [SerializeField] private Dig digCapacity = default;

    [Header("Capacities assignment")]
    [SerializeField] private bool activateAssignModeEvenWithoutAvailableSlot = false;
    [SerializeField] private bool keepAssignModeActivatedAfterAttributionSuccess = false;
    [SerializeField] private bool keepAssignModeActivatedAfterAttributionFail = true;

    [HideInInspector] public float AltitudeModifier = 0;
    [HideInInspector] public bool CanAddAltitudeModifier = true;
    [HideInInspector] public float MovementControlCoef = 1;
    [HideInInspector] public Vector2 ExternalVelocity = Vector2.zero;
    
    public bool AssignationMode { get; private set; }
    public bool CanBeEjected = true;

    private MeshRenderer meshRenderer = default;
    private Material defaultPlayerMaterial = default;

    private uint availableUnassignedCapacities = 0;
    private Capacity currentCapacityUsed = Capacity.NONE;

    private Vector2 inputs = Vector2.zero;
    private Vector3 gravityCenter = default;
    private Vector3 ejection = default;

    private Action doAction = default;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultPlayerMaterial = meshRenderer.material;

        gravityCenter = levelSettings.GravityCenter;
        DisableAllCapacities(false);

        SetModeMove();
    }

    public void FixedUpdate()
    {
        doAction.Invoke();
    }

    #region Movement
    private void SetModeMove()
    {
        doAction = DoActionMove;
        dashCapacity.gameObject.SetActive(true);
    }

    private void DoActionMove()
    {
        rigidbody.position += (transform.right * (inputs.x * MovementControlCoef + ExternalVelocity.x) + transform.forward * (inputs.y * MovementControlCoef + ExternalVelocity.y)) * (speed * Time.fixedDeltaTime);
        rigidbody.position = gravityCenter + (rigidbody.position - gravityCenter).normalized * (levelSettings.PlanetRadius + AltitudeModifier);

        RotateAccordingToPlanet();

        AltitudeModifier = 0;
        ExternalVelocity = Vector3.zero;
    }

    private void RotateAccordingToPlanet()
    {
        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;
        rigidbody.rotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.right).normalized) * rigidbody.rotation;
    }

    public void UpdateMoveInputs(InputAction.CallbackContext callback)
    {
        inputs = callback.ReadValue<Vector2>();
    }
    #endregion

    #region Capacities
    public void SwitchCapacityAssignationMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (availableUnassignedCapacities > 0 || activateAssignModeEvenWithoutAvailableSlot)
            {
                AssignationMode = true;
                Debug.LogWarning("Start assignation");
            }
        }
        else if (context.canceled) 
        { 
            AssignationMode = false; 
            Debug.LogWarning("End assignation"); 
        }
    }

    public void CollectCapacityToAssign() 
    {
        if (availableUnassignedCapacities == 0) 
            meshRenderer.material = hasCapacityToAssignMaterial;

        availableUnassignedCapacities++;
    }

    public bool TryAddCapacity(Capacity type, Direction dashDirection = default)
    {
        if (availableUnassignedCapacities > 0 && ActivateCapacityIfNew(type, out PlayerCapacity capacity, dashDirection))
        {
            capacity.CreateRenderer(capacityRenderersContainer).localRotation = Quaternion.Euler(type switch
            {
                Capacity.JUMP => Vector3.zero,
                Capacity.DASH => new Vector3 (90, 0, (int)dashDirection),
                Capacity.DIG => new Vector3 (180, 0, 0),
                _ => Vector3.zero
            });

            availableUnassignedCapacities--;

            if (availableUnassignedCapacities == 0) 
                meshRenderer.material = defaultPlayerMaterial;

            if (!keepAssignModeActivatedAfterAttributionSuccess) 
                AssignationMode = false;

            return true;
        }

        if (!keepAssignModeActivatedAfterAttributionFail) AssignationMode = false;

        return false;
    }

    private bool ActivateCapacityIfNew(Capacity type, out PlayerCapacity capacityScript, Direction dashDirection = default)
    {
        switch (type)
        {
            case Capacity.JUMP:
                capacityScript = jumpCapacity;
                return jumpCapacity.enabled ? false : jumpCapacity.enabled = true;

            case Capacity.DASH:
                dashCapacity.enabled = true;
                capacityScript = dashCapacity;
                return dashCapacity.TryAddDirection(dashDirection);                

            case Capacity.DIG:
                capacityScript = digCapacity;
                return digCapacity.enabled ? false : digCapacity.enabled = true;

            default:
                capacityScript = default;
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

    private void DisableAllCapacities(bool keepUnlock = true)
    {
        if (keepUnlock) dashCapacity.gameObject.SetActive(false);
        else 
        {
            dashCapacity.enabled = false;
            jumpCapacity.enabled = false;
            digCapacity.enabled = false;
        }
    }
    #endregion

    #region Interactions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            Debug.Log("Aie !");
            Eject(rigidbody.position - collision.rigidbody.position, ejectionOnPlayerContactStrenght);
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " --> Die");
    }

    public void Eject(Vector3 direction, float strength)
    {
        if (CanBeEjected)
        {
            ejection = direction.normalized * strength;
            SetModeEjected();
        }
    }

    private void SetModeEjected()
    {
        doAction = DoActionEjected;
        DisableAllCapacities();
    }

    private void DoActionEjected()
    {
        rigidbody.position += ejection;

        ejection -= (rigidbody.position - gravityCenter).normalized * ejectionGravity;
        ejection *= 1 - ejectionDeceleration;

        Vector3 toCenter = rigidbody.position - gravityCenter;

        RotateAccordingToPlanet();

        if (toCenter.magnitude <= levelSettings.PlanetRadius)
        {
            rigidbody.position = gravityCenter + toCenter.normalized * levelSettings.PlanetRadius;
            ejection = Vector3.zero;

            SetModeMove();
        }
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