using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public delegate void PlayerEventHandler(Player player, int possessedCollectibles = 0);
    public static event PlayerEventHandler OnPause;
    public event PlayerEventHandler OnDeath;
    public event PlayerEventHandler OnCollectibleUpdate;

    [Header("References")]
    [SerializeField] private new Rigidbody rigidbody = default;
    [SerializeField] private Transform capacityRenderersContainer = default;
    [SerializeField] private Material hasCapacityToAssignMaterial = default;

    [Header("Parameters")]
    [SerializeField] private float speed = 1;

    [Header("Collision")]
    [SerializeField] private float ejectionOnPlayerContactStrenght = 0.3f;
    [SerializeField] private float ejectionOnPlayerContactWithDashStrength = 0.5f;
    [SerializeField] private float ejectionDeceleration = 0.02f;
    [SerializeField] private float ejectionGravity = 0.05f;
    [SerializeField] private float upModifierOnEject = 0.1f;

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
    [HideInInspector] public bool CanBeEjected = true;

    public bool AssignationMode { get; private set; } = false;

    private uint AvailableUnassignedCapacities 
    { 
        get => _availableUnassignedCapacities; 
        set 
        {
            if (!(value > 0 && _availableUnassignedCapacities > 0))
            {
                meshRenderer.material = value == 0 ? defaultPlayerMaterial : hasCapacityToAssignMaterial;
            }

            _availableUnassignedCapacities = value;
            OnCollectibleUpdate?.Invoke(this, (int)_availableUnassignedCapacities);
        } 
    }

    private uint _availableUnassignedCapacities = 0;
    private Capacity currentCapacityUsed = Capacity.NONE;

    private MeshRenderer meshRenderer = default;
    private Material defaultPlayerMaterial = default;
    
    private Vector2 inputs = Vector2.zero;
    private Vector3 gravityCenter = default;
    private Vector3 ejection = default;

    private LevelSettings levelSettings = default;

    private Action doAction = default;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultPlayerMaterial = meshRenderer.material;

        DisableAllCapacities(false);
        GetComponent<BoxCollider>().enabled = false;

        doAction = () => { };
    }

    public void SpawnOnLevel(Vector3 position, LevelSettings currentLevelSettings)
    {
        gameObject.SetActive(true);

        rigidbody.position = position;

        levelSettings = currentLevelSettings;
        gravityCenter = levelSettings.GravityCenter;
    }

    [ContextMenu("Set Mode Play")]
    public void StartGame() 
    { 
        SetModeMove();
        GetComponent<BoxCollider>().enabled = true;
    }

    public void FixedUpdate()
    {
        doAction.Invoke();
    }

    #region Movement
    private void SetModeMove()
    {
        doAction = DoActionMove;
        dashCapacity.gameObject.SetActive(true);        //dashCapacity en tant que gameobject de toutes les capacités
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
            if (AvailableUnassignedCapacities > 0 || activateAssignModeEvenWithoutAvailableSlot)
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
        AvailableUnassignedCapacities++;
    }

    public bool TryAddCapacity(Capacity type, Direction dashDirection = default)
    {
        if (AvailableUnassignedCapacities > 0 && ActivateCapacityIfNew(type, out PlayerCapacity capacity, dashDirection))
        {
            capacity.CreateRenderer(capacityRenderersContainer).localRotation = Quaternion.Euler(type switch
            {
                Capacity.JUMP => Vector3.zero,
                Capacity.DASH => new Vector3 (90, 0, (int)dashDirection),
                Capacity.DIG => new Vector3 (180, 0, 0),
                _ => Vector3.zero
            });

            AvailableUnassignedCapacities--;

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
        if (collision.gameObject.TryGetComponent(out Player otherPlayer))
        {
            Debug.Log("Aie !");
            Eject(rigidbody.position - collision.rigidbody.position, otherPlayer.IsUsingCapacity(Capacity.DASH) ? ejectionOnPlayerContactWithDashStrength : ejectionOnPlayerContactStrenght);
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " --> Die");

        //Animation + enabled = false;

        gameObject.SetActive(false);
        enabled = false;

        OnDeath?.Invoke(this, capacityRenderersContainer.childCount + (int)AvailableUnassignedCapacities);
    }

    public void Eject(Vector3 direction, float strength)
    {
        if (CanBeEjected)
        {
            ejection = (direction + (rigidbody.position - levelSettings.GravityCenter).normalized * upModifierOnEject).normalized * strength;
            SetModeEjected();
        }
    }

    private void SetModeEjected()
    {
        doAction = DoActionEjected;
        DisableAllCapacities();

        MovementControlCoef = 1;
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

    public void ResetValues()
    {
        dashCapacity.ResetCapacity();
        digCapacity.ResetCapacity();
        jumpCapacity.ResetCapacity();

        AvailableUnassignedCapacities = 0;

        for (int i = capacityRenderersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(capacityRenderersContainer.GetChild(i).gameObject);
        }

        AssignationMode = false;
        CanBeEjected = true;
        currentCapacityUsed = Capacity.NONE;
    }
}