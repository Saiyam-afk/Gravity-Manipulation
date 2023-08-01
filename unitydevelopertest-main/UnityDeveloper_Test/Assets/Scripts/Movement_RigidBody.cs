using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement_RigidBody : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeReference] private float speed = 5f;
    private Rigidbody rb;
    [SerializeReference] float turnSmoothTime = 0.1f;
    Vector3 direction;
    Vector3 lookDirection;
    [SerializeField] private Transform mesh;

    [Header("Animations")]
    [SerializeField] private Animator animatior;

    [Header("Jump Variables")]
    [SerializeField] private float fallingVelocity = 5f;
    bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeReference] private float jumpHeight;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private float inAirTime = 0f;
    [SerializeField] private float jumpIntensity = -15;

    [Header("Camera Variables")]
    public Transform cam;
    public CinemachineFreeLook cinemachine;

    [Header("Gravity Variables")]
    [SerializeField] private float desiredDuration;
    [SerializeField] private Transform camPos;
    [SerializeField] private GameObject hollowgram;
    [SerializeField] bool gravCheck;
    float camy;
    float r;

    public Hollowgram hollowgramScript;
    private enum gravDir
    {
        X_POS, X_NEG, Y_POS, Y_NEG, Z_POS, Z_NEG
    }
    private enum gravDirIndicator
    {
        X_POS, X_NEG, Y_POS, Y_NEG, Z_POS, Z_NEG
    }

    [SerializeField] private gravDir previousGravity;
    [SerializeField] private gravDir currentGravity;

    [SerializeField]
    private gravDirIndicator gravInd;

    // Start is called before the first frame update
    void Start()
    {
        currentGravity = gravDir.Y_NEG;
        rb = gameObject.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        GravityManager();
        GravityChangeInput();
        camy = UnityEditor.TransformUtils.GetInspectorRotation(camPos).y;

        if (isGrounded) animatior.SetBool("isGrounded", true);
        else animatior.SetBool("isGrounded", false);
    }
    void FixedUpdate()
    {
        if (!isGrounded)
        {
            inAirTime = inAirTime + Time.deltaTime;
            rb.AddForce(-transform.up * fallingVelocity * inAirTime);
        }
        if (isGrounded)
        {
            inAirTime = 0f;
        }
        if (isJumping)
        {
            Jump();
            StartCoroutine(waitJump());
        }
    }

    IEnumerator waitJump()
    {
        yield return new WaitForSeconds(0.5f);
        if (isGrounded)
        {
            isJumping = false;
        }
    }
    void Jump()
    {
        rb.AddForce(transform.up * jumpIntensity, ForceMode.Impulse);
    }

    void Move() 
    { 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = cam.forward * vertical; direction += cam.right * horizontal;
        direction = direction.normalized;
        direction = direction * speed;
        if (currentGravity == gravDir.Y_NEG)direction.y = 0f;
        if (currentGravity == gravDir.Y_POS)direction.y = 0f;
        if (currentGravity == gravDir.Z_POS)direction.z = 0f;
        if (currentGravity == gravDir.Z_NEG)direction.z = 0f;
        if (currentGravity == gravDir.X_POS)direction.x = 0f;
        if (currentGravity == gravDir.X_NEG)direction.x = 0f;
        
        rb.velocity = direction;

        camPos.position = cam.position;
        camPos.eulerAngles = cam.eulerAngles;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
        }

        if (direction.magnitude > 0.1f) animatior.SetBool("isRunning", true);
        else animatior.SetBool("isRunning", false);
        if(direction.magnitude >= 0.1f) mesh.forward = direction;
        mesh.localEulerAngles = new Vector3(0f, mesh.localEulerAngles.y, 0f);
    }

    void GravityManager()
    {
        if (currentGravity == gravDir.Y_NEG)RotatePlayerGravYAxis(0f);
        if (currentGravity == gravDir.Y_POS)RotatePlayerGravYAxis(180f);
        if (currentGravity == gravDir.Z_POS)RotatePlayerGravZAxis(-90f);
        if (currentGravity == gravDir.Z_NEG)RotatePlayerGravZAxis(90f);
        if (currentGravity == gravDir.X_POS)RotatePlayerGravXAxis(90f);
        if (currentGravity == gravDir.X_NEG)RotatePlayerGravXAxis(-90f);
    }

    void GravityChangeInput()
    {
        if (currentGravity == gravDir.Y_NEG && isGrounded)
        {
            if (camy > -45 && camy <= 45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
            }
            if (camy > 45 && camy <= 135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
            }
            if (camy > 135 || camy <= -135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
            }
            if (camy > -135 && camy <= -45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow))StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow))StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
            }
        }

        if (currentGravity == gravDir.Z_NEG && isGrounded)
        {
            if (camy > -45 && camy <= 45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
            }
            if (camy > 45 && camy <= 135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
            }
            if (camy > 135 || camy <= -135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
            }
            if (camy > -135 && camy <= -45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
            }
        }

        if (currentGravity == gravDir.Z_POS && isGrounded)
        {
            if (camy > -45 && camy <= 45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
            }
            if (camy > 45 && camy <= 135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
            }
            if (camy > 135 || camy <= -135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
            }
            if (camy > -135 && camy <= -45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
            }
        }

        if (currentGravity == gravDir.X_POS && isGrounded)
        {
            if (camy > -45 && camy <= 45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
            }
            if (camy > 45 && camy <= 135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
            }
            if (camy > 135 || camy <= -135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
            }
            if (camy > -135 && camy <= -45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
            }
        }

        if (currentGravity == gravDir.X_NEG && isGrounded)
        {
            if (camy > -45 && camy <= 45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
            }
            if (camy > 45 && camy <= 135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
            }
            if (camy > 135 || camy <= -135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
            }
            if (camy > -135 && camy <= -45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Y_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
            }
        }

        if (currentGravity == gravDir.Y_POS && isGrounded)
        {
            if (camy > -45 && camy <= 45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
            }
            if (camy > 45 && camy <= 135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
            }
            if (camy > 135 || camy <= -135)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
            }
            if (camy > -135 && camy <= -45)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_NEG));
                if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.X_POS));
                if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_POS));
                if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(gravityIndicator(gravDirIndicator.Z_NEG));
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (gravCheck && gravInd == gravDirIndicator.Z_POS)
        {
            hollowgramScript.RotatePlayerGravZAxis(-90f);
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(gravityCoolDown(gravDir.Z_POS));
                hollowgram.SetActive(false);
            }
        }

        if (gravCheck && gravInd == gravDirIndicator.Z_NEG)
        {
            hollowgramScript.RotatePlayerGravZAxis(90f);
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(gravityCoolDown(gravDir.Z_NEG));
                hollowgram.SetActive(false);
            }
        }

        if (gravCheck && gravInd == gravDirIndicator.X_NEG)
        {
            hollowgramScript.RotatePlayerGravXAxis(-90f);
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(gravityCoolDown(gravDir.X_NEG));
                hollowgram.SetActive(false);
            }
        }

        if (gravCheck && gravInd == gravDirIndicator.X_POS)
        {
            hollowgramScript.RotatePlayerGravXAxis(90f);
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(gravityCoolDown(gravDir.X_POS));
                hollowgram.SetActive(false);
            }
        }

        if (gravCheck && gravInd == gravDirIndicator.Y_NEG)
        {
            hollowgramScript.RotatePlayerGravYAxis(0f);
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(gravityCoolDown(gravDir.Y_NEG));
                hollowgram.SetActive(false);
            }
        }

        if (gravCheck && gravInd == gravDirIndicator.Y_POS)
        {
            hollowgramScript.RotatePlayerGravYAxis(180f);
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(gravityCoolDown(gravDir.Y_POS));
                hollowgram.SetActive(false);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.R)) //TO RESET THE GRAVITY
        {
            StartCoroutine(gravityCoolDown(gravDir.Y_NEG));
        }
    }

    IEnumerator gravityCoolDown(gravDir grav)
    {
        yield return new WaitForSeconds(0.3f);
        previousGravity = currentGravity;
        currentGravity = grav;
        yield return new WaitForSeconds(1f);
    }    

    IEnumerator gravityIndicator(gravDirIndicator grav)
    {
        gravCheck = true;
        yield return new WaitForSeconds(0.1f);
        gravInd = grav;
        hollowgram.SetActive(enabled);
    }
    void RotatePlayerGravZAxis(float ang)
    {
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.x, ang, ref r, desiredDuration);
        Vector3 desiredRot = new Vector3(Angle, 0f, 0f);
        transform.rotation = Quaternion.Euler(desiredRot);
    }

    void RotatePlayerGravXAxis(float ang)
    {

        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, ang, ref r, desiredDuration);
        Vector3 desiredRot = new Vector3(0f, 0f, Angle);
        transform.rotation = Quaternion.Euler(desiredRot);
    }

    void RotatePlayerGravYAxis(float ang)
    {
        if (previousGravity == gravDir.Z_POS || previousGravity == gravDir.Z_NEG)
        {
            if (ang == 0)
            {
                float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.x, ang, ref r, desiredDuration);
                Vector3 desiredRot = new Vector3(Angle, 0f, 0f);
                transform.rotation = Quaternion.Euler(desiredRot);
            }
            else
            {
                float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, ang, ref r, desiredDuration);
                Vector3 desiredRot = new Vector3(0f, 0f, Angle);
                transform.rotation = Quaternion.Euler(desiredRot);
            }
        }
        else
        {
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, ang, ref r, desiredDuration);
            Vector3 desiredRot = new Vector3(0f, 0f, Angle);
            transform.rotation = Quaternion.Euler(desiredRot);
        }
    }
}