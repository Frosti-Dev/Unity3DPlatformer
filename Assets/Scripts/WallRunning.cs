using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;

//currently not in use because it wouldnt work with the code used in this project. 
public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask wallRunnable;
    public LayerMask ground;
    public float wallRunForce = 200f;
    public float maxWallRunTime = 1.5f;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance = 0.7f;
    public float minJumpHeight = 2f;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    public InputAction moveAction;
    private AdvancedMoveController advancedMoveController;
    private Rigidbody rb;

    private void Awake()
    {
        moveAction = new InputAction("Movement", type: InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

       
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        advancedMoveController = GetComponent<AdvancedMoveController>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if(advancedMoveController.isWallrunning) 
            WallRunningMovement();
    }


    //will check if walls are close to the player
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, wallRunnable);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, wallRunnable);

    }

    //checks if the player is above the ground
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position,Vector3.down,minJumpHeight, ground);
    }

    //checks states to start wall run
    private void StateMachine()
    {
        horizontalInput = moveAction.ReadValue<Vector2>().x;
        verticalInput = moveAction.ReadValue<Vector2>().y;

        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!advancedMoveController.isWallrunning)
                StartWallRun();

            else
            {
                if(advancedMoveController.isWallrunning)
                    StopWallRun();
            }
        }
    }

    //starts wall run
    private void StartWallRun()
    {
        Debug.Log("Start Wallrunning");
        advancedMoveController.isWallrunning = true;
        advancedMoveController.maxVelocity = advancedMoveController.wallrunSpeed;
    }

    //while on wall movement
    private void WallRunningMovement()
    {

        rb.useGravity = false; //turns off gravity of rigidbody
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //push to wall force
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }


    //stop the wall run
    private void StopWallRun()
    {
        Debug.Log("End Wallrunning");
        advancedMoveController.isWallrunning = false;
        advancedMoveController.maxVelocity = 9f;
    }
}
