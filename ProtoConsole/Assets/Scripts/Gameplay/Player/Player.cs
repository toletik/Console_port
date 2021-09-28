using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelSettings levelSettings = default;
    [SerializeField] private new Rigidbody rigidbody = default;

    [SerializeField] private float speed = 1;

    private Vector3 gravityCenter = default;

    //private float phi = 0;
    //private float theta = 0;

    private void Awake()
    {
        gravityCenter = levelSettings.GravityCenter;
    }

    /*
    private void FixedUpdate()
    {
        float frameSpeed = speed * Time.fixedDeltaTime;

        phi += Input.GetAxis("Vertical") * frameSpeed;
        theta -= Input.GetAxis("Horizontal") * frameSpeed;

        float altitude = levelSettings.PlanetRadius;
        float sinPhi = Mathf.Sin(phi);

        rigidbody.position = altitude * new Vector3(sinPhi * Mathf.Sin(theta), Mathf.Cos(phi), sinPhi * Mathf.Cos(theta)) + gravityCenter;

        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;
        
    }
    */


    
    public void FixedUpdate()
    {
        Quaternion rotation;
        Vector3 eulerRotation;

        Vector3 forward = transform.forward; // Vector3.ProjectOnPlane(transform.forward, Vector3.right); //Vector3.Project(cameraOrientation.forward, transform.forward);
        Vector3 right = transform.right; // Vector3.ProjectOnPlane(transform.right, Vector3.forward); //Vector3.Project(cameraOrientation.right, transform.right);


        rigidbody.position += (forward.normalized * Input.GetAxis("Vertical") + right.normalized * Input.GetAxis("Horizontal")) * speed * Time.fixedDeltaTime;
        rigidbody.position = gravityCenter + (rigidbody.position - gravityCenter).normalized * 12.5f;

        rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;

        if (Input.GetKey(KeyCode.Space)) Debug.Log(transform.forward);



        rigidbody.rotation = Quaternion.FromToRotation(transform.forward, Vector3.ProjectOnPlane(transform.forward, Vector3.right)) * rigidbody.rotation;

        return;

        eulerRotation = rigidbody.rotation.eulerAngles;
        Vector3 rotationResult = Quaternion.Euler(eulerRotation.x, 0, eulerRotation.z) * transform.up;

        //if (Input.GetKey(KeyCode.Space))
            rigidbody.rotation = Quaternion.AngleAxis(eulerRotation.y, rigidbody.position - gravityCenter) * rigidbody.rotation;

        return;

        Debug.DrawLine(transform.position, transform.position + rotationResult.normalized * 5, Color.red);

        if (Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("Forward : " + transform.forward);
            //Debug.Log("Result : " + rotationResult);

            Debug.Log(Colinear(rotationResult, transform.forward, true));

        }

        if (Colinear(rotationResult, transform.forward) || Colinear(rotationResult, transform.right))
        {
            Debug.Log("Colinear !");
            //rigidbody.rotation = Quaternion.Euler(rigidbody.rotation.eulerAngles);
        }
        else rigidbody.rotation = Quaternion.Euler(eulerRotation.x, 0, eulerRotation.z);





        return;

        eulerRotation = rotation.eulerAngles;
        eulerRotation.y = 0;

        rigidbody.rotation = Quaternion.Euler(eulerRotation);

        
       rigidbody.rotation = Quaternion.FromToRotation(transform.up, rigidbody.position - gravityCenter) * rigidbody.rotation;

       //rotation = rigidbody.rotation.eulerAngles;
       rigidbody.rotation = Quaternion.Euler(new Vector3(rotation.x, 0, rotation.z));
       
    }

    private bool Colinear(Vector3 vectorA, Vector3 vectorB, bool debug = false)
    {
        float modifier;

        vectorA = vectorA.normalized;
        vectorB = vectorB.normalized;

        if (debug)
        {
            Debug.Log("Vector A = " + vectorA);
            Debug.Log("Vector B = " + vectorB);
        }

        if      (!Approximately(vectorB.x, 0)) { modifier = vectorA.x / vectorB.x; if (debug) Debug.LogError("X"); }
        else if (!Approximately(vectorB.y, 0)){ modifier = vectorA.y / vectorB.y; if (debug) Debug.LogError("Y"); }
        else if (!Approximately(vectorB.z, 0)) {modifier = vectorA.z / vectorB.z; if (debug) Debug.LogError("Z"); }
        else return false;

        if (debug) 
        { 
            Debug.LogWarning("Modifier = " + modifier); 
            Debug.LogWarning("----------");

            Debug.LogWarning("xA = " + vectorA.x + " & xB = " + modifier * vectorB.x);
            Debug.LogWarning("yA = " + vectorA.y + " & yB = " + modifier * vectorB.y);
            Debug.LogWarning("zA = " + vectorA.z + " & zB = " + modifier * vectorB.z);

        }

        return Approximately(vectorA.x, modifier * vectorB.x) && Approximately(vectorA.y, vectorB.y * modifier) && Approximately(vectorA.z, vectorB.z * modifier);
    }

    private bool Approximately(float a, float b, float precision = 0.005f)
    {
        return Mathf.Abs(a - b) < precision;
    }

}