using Com.IsartDigital.Common.UI;
using nn.hid;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    #region Variables
    private const string MATERIAL_EMISSIVE_KEYWORD = "_EMISSION";
    private const string MATERIAL_EMISSIVE_COLOR = "_EmissionColor";
    private const string MATERIAL_COLOR = "_Color";

    public delegate void PlayerEventHandler(Player player, int possessedCollectibles = 0);
    public static event PlayerEventHandler OnPause;
    public event PlayerEventHandler OnDeath;
    public event PlayerEventHandler OnCollectibleUpdate;
    public event PlayerEventHandler OnScoreUpdated;

    internal int playerID = 0;

    #region Serialize fields
    [Header("References")]
    [SerializeField] private new Rigidbody rigidbody = default;
    [SerializeField] private Transform capacityRenderersContainer = default;
    [SerializeField] private Material hasCapacityToAssignMaterial = default;
    [SerializeField] private FollowPlanetRotation followPlanetRotation = default;
    [SerializeField] private GameObject tagContainer = default;

    [Header("Parameters")]
    [SerializeField] private float speed = 1;
    [SerializeField] private float maxEjectionAltitude = 25;

    [Header("Invincibility")]
    [SerializeField] private float spawnInvincibilityTime = 3;
    [SerializeField] private int numberOfInvincibilityBlink = 4;
    [SerializeField] private AnimationCurve invincibilityBlink = default;
    [SerializeField] private Color invincibilityColor = Color.grey;

    [Header("Collision")]
    [SerializeField] private float ejectionOnPlayerContactStrenght = 0.3f;
    [SerializeField] private float ejectionOnPlayerContactWithDashStrength = 0.5f;
    [SerializeField] private float ejectionDeceleration = 0.02f;
    [SerializeField] private float ejectionGravity = 0.05f;
    [SerializeField] private float upModifierOnEject = 0.1f;

    [Header("Collision - Layers")]
    [SerializeField] private string defaultGameLayerNameForPlayer = "Player";
    [SerializeField] private string invincibilityLayerNameForPlayer = "Invincibility";

    [Header("Score")]
    [SerializeField] private int initialScore = 100;
    [SerializeField] private bool allowNegativeScore = false;
    [SerializeField] private int scoreLostOnDeath = 10;
    [SerializeField] private int scoreWonOnKill = 20;
    [SerializeField] private int bonusScoreWonOnBestPlayerKill = 10;

    [Header("Capacities")]
    [SerializeField] private Jump jumpCapacity = default;
    [SerializeField] private Dash dashCapacity = default;
    [SerializeField] private Dig digCapacity = default;

    [Header("Capacities assignment")]
    [SerializeField] private bool activateAssignModeEvenWithoutAvailableSlot = false;
    [SerializeField] private bool keepAssignModeActivatedAfterAttributionSuccess = false;
    [SerializeField] private bool keepAssignModeActivatedAfterAttributionFail = true;
    #endregion

    [HideInInspector] public float AltitudeModifier = 0;
    [HideInInspector] public bool CanAddAltitudeModifier = true;
    [HideInInspector] public float MovementControlCoef = 1;
    [HideInInspector] public Vector2 ExternalVelocity = Vector2.zero;
    [HideInInspector] public bool CanBeEjected = true;

    private VibrationValue onDeathVibration = VibrationValue.Make(0.9f, 160.0f, 0.0f, 320.0f);
    private float onDeathVibrationDuration = 1f;
    private VibrationValue onCollisionVibration = VibrationValue.Make(0.40f, 160.0f, 0.55f, 320.0f);
    private float onCollisionVibrationDuration = 0.2f;

    public bool AssignationMode { get; private set; } = false;

    public bool IsBestPlayer = false;
    public int InitialScore => initialScore;
    private int score;
    public int Score
    {
        get => score;
        set
        {
            //Update score du joueur
            score = value;
            if (score < 0 && !allowNegativeScore)
                score = 0;

            OnScoreUpdated?.Invoke(this, score);
        }
    }

    private Player collidedPlayer = null;

    private int AvailableUnassignedCapacities 
    { 
        get => availableUnassignedCapacities; 
        set 
        {
            if (!(value > 0 && availableUnassignedCapacities > 0))
            {
                meshRenderer.material = (value == 0 )? defaultPlayerMaterial : hasCapacityToAssignMaterial;
            }

            availableUnassignedCapacities = value;
            OnCollectibleUpdate?.Invoke(this, availableUnassignedCapacities);
        } 
    }

    private int availableUnassignedCapacities = 0;

    private Capacity currentCapacityUsed = Capacity.NONE;

    private MeshRenderer meshRenderer = default;
    private Material defaultPlayerMaterial = default;
    private BoxCollider boxCollider = default;

    private Vector2 inputs = Vector2.zero;
    private Vector3 gravityCenter = default;
    private Vector3 ejection = default;

    private LevelSettings levelSettings = default;

    private int defaultGameLayerForPlayer;
    private int invincibilityLayerForPlayer;

    private Action doAction = default;
    #endregion

    #region Functions

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        meshRenderer = GetComponent<MeshRenderer>();
        defaultPlayerMaterial = meshRenderer.material;

        DisableAllCapacities(false);

        defaultGameLayerForPlayer = LayerMask.NameToLayer(defaultGameLayerNameForPlayer);
        invincibilityLayerForPlayer = LayerMask.NameToLayer(invincibilityLayerNameForPlayer);

        doAction = () => { };
    }

    public void FixedUpdate()
    {
        doAction.Invoke();
    }


    public void SpawnOnLevel(Vector3 position, LevelSettings currentLevelSettings)
    {
        tagContainer.SetActive(true);
        meshRenderer.enabled = true;

        rigidbody.position = position;

        levelSettings = currentLevelSettings;
        gravityCenter = levelSettings.GravityCenter;
    }

    public void SetModePlay() 
    {
        enabled = true;
        boxCollider.enabled = true;

        SetModeMove();
        StartCoroutine(PlayInvincibilityTime());
    }


    #region Movement
    private void SetModeMove()
    {
        collidedPlayer = null;

        followPlanetRotation.StartToFollowPlanet();

        doAction = DoActionMove;
        dashCapacity.gameObject.SetActive(true);        //dashCapacity en tant que gameobject de toutes les capacités
    }

    private void DoActionMove()
    {
        rigidbody.position += (transform.right * (inputs.x * MovementControlCoef + ExternalVelocity.x) + 
                               transform.forward * (inputs.y * MovementControlCoef + ExternalVelocity.y)) 
                               * (speed * Time.fixedDeltaTime);
        rigidbody.position = gravityCenter + (rigidbody.position - gravityCenter).normalized * (levelSettings.PlanetRadius + AltitudeModifier);

        InclineAccordingToPlanet();

        AltitudeModifier = 0;
        ExternalVelocity = Vector3.zero;
    }

    private void InclineAccordingToPlanet()
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
                Debug.LogError("Start assignation");
            }
        }
        else if (context.canceled) 
        { 
            AssignationMode = false; 
            Debug.LogError("End assignation"); 
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
                Capacity.DASH => new Vector3 (90, 0, DirectionProperties.RetrieveRotation(dashDirection)),
                Capacity.DIG => new Vector3 (180, 0, 0),
                _ => Vector3.zero
            });

            AvailableUnassignedCapacities--;

            if (!keepAssignModeActivatedAfterAttributionSuccess) 
                AssignationMode = false;

            return true;
        }

        if (!keepAssignModeActivatedAfterAttributionFail)
            AssignationMode = false;

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
        if (capacity == currentCapacityUsed)
            return true;
        else if (currentCapacityUsed == Capacity.DASH_AND_JUMP && (capacity == Capacity.JUMP || capacity == Capacity.DASH)) 
            return true;
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
        if (keepUnlock) 
            dashCapacity.gameObject.SetActive(false);
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
            collidedPlayer = otherPlayer;
            Eject(rigidbody.position - collision.rigidbody.position, otherPlayer.IsUsingCapacity(Capacity.DASH) ? ejectionOnPlayerContactWithDashStrength : ejectionOnPlayerContactStrenght);
            StartCoroutine(VibrationManager.GetSingleton().VibrateForOneDuringSeconds(onCollisionVibration, playerID, onCollisionVibrationDuration));
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " --> Die");

        //Animation + enabled = false;

        followPlanetRotation.StopFollowingPlanet();

        tagContainer.SetActive(false);
        meshRenderer.enabled = false;
        enabled = false;

        OnDeath?.Invoke(this, capacityRenderersContainer.childCount + (int)AvailableUnassignedCapacities);

        collidedPlayer?.UpdateScore(scoreWonOnKill + (IsBestPlayer ? bonusScoreWonOnBestPlayerKill : 0));
        UpdateScore(-scoreLostOnDeath);

        ResetValues(false);

        StartCoroutine(VibrationManager.GetSingleton().VibrateForOneDuringSeconds(onDeathVibration, playerID, onDeathVibrationDuration));
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

        followPlanetRotation.StopFollowingPlanet();

        MovementControlCoef = 1;
    }

    private void DoActionEjected()
    {
        rigidbody.position += ejection;

        ejection -= (rigidbody.position - gravityCenter).normalized * ejectionGravity;
        ejection *= 1 - ejectionDeceleration;

        Vector3 toCenter = rigidbody.position - gravityCenter;

        InclineAccordingToPlanet();

        if (toCenter.magnitude <= levelSettings.PlanetRadius)
        {
            rigidbody.position = gravityCenter + toCenter.normalized * levelSettings.PlanetRadius;
            ejection = Vector3.zero;

            SetModeMove();
        }
        else if (toCenter.magnitude > levelSettings.PlanetRadius + maxEjectionAltitude) Die();
    }
    #endregion

    public void ClickOnPause(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            Debug.Log(gameObject.name + " : Click to pause/unpause the game");
            OnPause?.Invoke(this);
            UIManager.Instance.AddScreen<PauseScreen>();
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        Score += scoreToAdd;
    }

    public void ResetValues(bool resetScore = false)
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
        collidedPlayer = null;

        boxCollider.enabled = false;

        if (resetScore)
            Score = initialScore;

        gameObject.layer = defaultGameLayerForPlayer;
    }

    #region Player invincibility
    private IEnumerator PlayInvincibilityTime()
    {
        Action<Material, Color, float> updateCorrectColorOnMaterial = default;
        Material currentMaterial = null;
        Color baseColor = default;
        bool emissiveMaterial = false;
        
        gameObject.layer = invincibilityLayerForPlayer;

        for (float elapsedTime = 0f; elapsedTime < spawnInvincibilityTime; elapsedTime += Time.deltaTime)
        {
            if (meshRenderer.material != currentMaterial)
            {
                if (currentMaterial != null)
                {
                    //Reset material précédent
                    updateCorrectColorOnMaterial(currentMaterial, baseColor, 0);
                }
                
                //Valeurs du nouveau matériau
                currentMaterial = meshRenderer.material;
                emissiveMaterial = currentMaterial.IsKeywordEnabled(MATERIAL_EMISSIVE_KEYWORD);
                
                if (emissiveMaterial)
                {
                    baseColor = currentMaterial.GetColor(MATERIAL_EMISSIVE_COLOR);
                    updateCorrectColorOnMaterial = UpdateMaterialEmissiveColor;
                }
                else
                {
                    baseColor = currentMaterial.GetColor(MATERIAL_COLOR);
                    updateCorrectColorOnMaterial = UpdateMaterialColor;
                }
            }

            updateCorrectColorOnMaterial(currentMaterial, baseColor, invincibilityBlink.Evaluate(numberOfInvincibilityBlink * elapsedTime / spawnInvincibilityTime));

            yield return null;
        }

        //DynamicGI.UpdateEnvironment();

        gameObject.layer = defaultGameLayerForPlayer;

    }

    private void UpdateMaterialColor(Material material, Color baseColor, float modifier)
    {
        material.SetColor(MATERIAL_COLOR, Color.Lerp(baseColor, invincibilityColor, modifier));
    }

    private void UpdateMaterialEmissiveColor(Material material, Color baseColor, float modifier)
    {
        material.SetColor(MATERIAL_EMISSIVE_COLOR, baseColor * (1 - modifier));
    }
    #endregion

    private void OnDestroy()
    {
        OnPause = null;
        OnDeath = null;
        OnCollectibleUpdate = null;
        OnScoreUpdated = null;
    }

    #endregion
}