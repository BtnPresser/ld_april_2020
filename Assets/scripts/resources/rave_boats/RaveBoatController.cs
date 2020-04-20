using UnityEngine;

public class RaveBoatController : MonoBehaviour
{
    private Rigidbody2D gameObjectRigidBody;
    private Animator animator;

    [Range(0.01f, 20f)]
    public float speed = 7f;
    [Range(0.0f, 6f)]
    public float maxMagnitude = 1f;
    public float sprintMagnitudeMultiplier = 1.3f;
    private float defaultMaxMagnitude;
    private float sprintMagnitude;

    [Range(1, 5)]
    public int maxRaversPerBoat = 3;
    public int raveMemberCount;

    [Range(1, 3)]
    public float timeBetweenActionsUpperBound = 0f;
    public float timeBetweenActionsLowerBound = 0f;
    private float timeSinceLastAction = 0f;
    public float timeBetweenActions = 1.5f;

    public bool randomTimeBetweeenActions = false;

    private bool playerInCollider = false;
    private bool raveBoatInCollider = false;


    // Setup the Player
    private void Awake()
    {
        gameObjectRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        defaultMaxMagnitude = maxMagnitude;
        sprintMagnitude = maxMagnitude * sprintMagnitudeMultiplier;

        if (randomTimeBetweeenActions)
        {
            timeBetweenActions = UnityEngine.Random.Range(1f, 2f);
        }

        raveMemberCount = UnityEngine.Random.Range(1, maxRaversPerBoat);
    }

    public void Update()
    {
        UpdateAnimationBasedOnRigidBodySpeed(animator, gameObjectRigidBody);
    }

    public void FixedUpdate()
    {
        if (CanMakeAction())
        {
            Wander(gameObjectRigidBody);
        }
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        // Check if there is an attractor or stranded raver and add forces towards it when possible
    }

    private bool CanMakeAction()
    {
        timeSinceLastAction += Time.deltaTime;
        if (timeSinceLastAction > timeBetweenActions)
        {
            return true;
        }

        return false;
    }

    private void TookAction()
    {
        //Debug.Log("Took action after: " + timeSinceLastAction + " seconds!");
        timeSinceLastAction = 0f;
    }

    private void MoveTowardCollisionObject(Rigidbody2D gameObjectRigidBody, GameObject targetObject)
    {
        Vector3 targetObjectPosition = targetObject.transform.position;

        //Vector3 positionToMoveTo = Vector3.MoveTowards(gameObjectRigidBody.transform.position, targetObjectPosition, speed/2);
        //Debug.Log("Force to move the ai towards the play: " + positionToMoveTo + "\n" + "Target Direction: " + targetDirection);
        Vector3 targetDirection = (targetObjectPosition - gameObjectRigidBody.transform.position).normalized;

        AddForceToRigidBody((targetDirection * speed / 2), gameObjectRigidBody);
        TookAction();
    }

    // Add a force in a random direction
    private void Wander(Rigidbody2D gameObjectBody)
    {
        float xMove = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * speed;
        float yMove = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * speed;
        AddForceToRigidBody(new Vector2(xMove, yMove), gameObjectBody);
        TookAction();
    }

    private void AddForceToRigidBody(Vector3 newForce, Rigidbody2D gameObjectRigidBody)
    {
        gameObjectRigidBody.AddForce(newForce);
        dampenRigidBodyVelocity(gameObjectRigidBody);
    }

    private void dampenRigidBodyVelocity(Rigidbody2D gameObjectRigidBody)
    {
        Vector2 velocity = gameObjectRigidBody.velocity;
        float magnitude = velocity.magnitude;

        gameObjectRigidBody.AddForce(velocity * -1 * magnitude);
    }

    private void UpdateAnimationBasedOnRigidBodySpeed(Animator animator, Rigidbody2D gameObjectRigidBody)
    {
        Vector2 velocity = gameObjectRigidBody.velocity;
        float xSpeed = velocity.x;
        float ySpeed = velocity.y;

        // We only want to update the booleans if the new velocity is not zero. The player should maintain the current animation otherwise
        if (xSpeed != 0f && ySpeed != 0f)
        {
            bool posXSpeed = xSpeed > 0;
            bool posYSpeed = ySpeed > 0;
            bool xSpeedGreaterThanY = Mathf.Abs(xSpeed) > Mathf.Abs(ySpeed);

            //logNewAnimationBools(posXSpeed, posYSpeed, xSpeedGreaterThanY);
            //logAnimatorBools();

            animator.SetBool("PosXDir", posXSpeed);
            animator.SetBool("PosYDir", posYSpeed);
            animator.SetBool("GreaterXSpeed", xSpeedGreaterThanY);
        }

    }

    private static void logNewAnimationBools(bool posXSpeed, bool posYSpeed, bool xSpeedGreaterThanY)
    {

        // For checking what the various bools we're about to set are
        Debug.Log("posXSpeed: " + posXSpeed + "\n" +
            "posYSpeed: " + posYSpeed + "\n" +
            "xSpeedGreaterThanY: " + xSpeedGreaterThanY);
    }

    private void logAnimatorBools()
    {
        // For checking whether the animator updated the variables or not
        bool animatorXDir = animator.GetBool("PosXDir");
        bool animatorYDir = animator.GetBool("PosYDir");
        bool animatorXGreater = animator.GetBool("GreaterXSpeed");
        Debug.Log("PosXDir: " + animatorXDir + "\n" +
            "PosYDir: " + animatorYDir + "\n" +
            "GreaterXSpeed: " + animatorXGreater + "\n");
    }

    private MovementState CalculateMovementState
    {
        get
        {
            if (playerInCollider && raveBoatInCollider)
            {
                return MovementState.MoveAwayFromPlayer;
            }
            else if (raveBoatInCollider)
            {
                return MovementState.DistractRaveBoats;
            }
            else if (playerInCollider)
            {
                return MovementState.ActualThoughtfulDecision;
            }
            return MovementState.Wander;
        }
    }
}
