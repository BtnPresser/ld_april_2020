using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject SparklerPrefab;

    private Rigidbody2D gameObjectRigidBody;
    private Animator animator;
    
    public int raveMemberCount;

    [Range(20f, 50f)]
    public float throwingForce = 35f;

    [Range(0.05f, 10f)]
    public float timeBetweenSparklers = 0.5f;
    private float lastSparklerThrownTime = 0;

    [Range(0.01f, 20f)]
    public float speed = 10f;
    [Range(0.0f, 6f)]
    public float maxMagnitude = 1f;
    public float sprintMagnitudeMultiplier = 1.3f;
    
    private float defaultMaxMagnitude;
    private float sprintMagnitude;


    [Range(1, 5)]
    public float timeBetweenActionsUpperBound = 0f;
    public float timeBetweenActionsLowerBound = 0f;
    public float timeBetweenActions = 1f;

    public bool randomTimeBetweeenActions = false;

    private float timeSinceLastAction = 0f;
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
            timeBetweenActions = UnityEngine.Random.Range(1f, 3f);
        }
    }

    public void Update()
    { 
        UpdateAnimationBasedOnRigidBodySpeed(animator, gameObjectRigidBody);
    }

    private void FixedUpdate()
    {
        timeSinceLastAction += Time.deltaTime;
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        playerInCollider = collision.gameObject.CompareTag("Player");
        raveBoatInCollider = collision.gameObject.CompareTag("RaveBoat");

        if (CanMakeAction())
        {
            DecideOnActionToTake(gameObjectRigidBody, collision.gameObject);
        }
    }

    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        playerInCollider = collision.gameObject.CompareTag("Player");
        raveBoatInCollider = collision.gameObject.CompareTag("RaveBoat");

        if (!playerInCollider && !raveBoatInCollider && CanMakeAction())
        {
            Wander(gameObjectRigidBody);
        }
    }


    private void DecideOnActionToTake(Rigidbody2D gameObjectBody, GameObject collisionObject)
    {
        MovementState state = CalculateMovementState;

        switch(state)
        {
            case MovementState.DistractRaveBoats:
                DistractRaveBoats(gameObjectRigidBody, collisionObject);
                break;

            case MovementState.MoveAwayFromPlayer:
                MoveAwayFromCollisionObject(gameObjectBody, collisionObject);
                break;

            case MovementState.ActualThoughtfulDecision:
            /*MoveTowardCollisionObject(gameObjectRigidBody, collisionObject);
            break;*/ // Currently Does nothing and simply Wanders

            default:
                Wander(gameObjectBody);
                break;
        }

        /* // Add a chance to sprint when heading towards rave boats or player on hard difficulties?
        if (kb.shiftKey.IsPressed())
        {
            maxMagnitude = sprintMagnitude;
        }
        else
        {
            maxMagnitude = defaultMaxMagnitude;
        }*/

        if (exceedMaxMagnitude(gameObjectBody))
        {
            dampenRigidBodyVelocity(gameObjectBody);
        }
    }


    private void DistractRaveBoats(Rigidbody2D gameObjectRigidBody, GameObject collisionObject)
    {
        ThrowAttractorTowardsCollisionObject(collisionObject);
        MoveTowardCollisionObject(gameObjectRigidBody, collisionObject);
        TookAction();
    }

    private void ThrowAttractorTowardsCollisionObject(GameObject collisionObject)
    {
        lastSparklerThrownTime += Time.deltaTime;

        if (lastSparklerThrownTime >= timeBetweenSparklers)
        {
            // Create an object in the world that doesn't have physics collision, just collision with the player to get a remember from them
            //Debug.Log("Threw an attractor toward the rave boat!");
            GameObject sparkler = Instantiate(SparklerPrefab, transform.position, Quaternion.identity);

            // Calculate Force we should move object toward a given position
            Vector3 directionToThrow = GetDirectionFromCurrentObjectToTargetObject(gameObjectRigidBody, collisionObject);

            ApplyForceToThrownObject(directionToThrow * throwingForce, sparkler);
            TookAction();
        }
    }

    private bool CanMakeAction()
    {
        return (timeSinceLastAction > timeBetweenActions);
    }

    private void TookAction()
    {
        //Debug.Log("Took action after: " + timeSinceLastAction + " seconds!");
        timeSinceLastAction = 0f;
    }

    // Add a force in a random direction
    private void Wander(Rigidbody2D gameObjectBody)
    {
        float xMove = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * speed;
        float yMove = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * speed;
        AddForceToRigidBody(new Vector2(xMove, yMove), gameObjectBody);
        TookAction();
    }

    private void MoveTowardCollisionObject(Rigidbody2D gameObjectRigidBody, GameObject targetObject)
    {
        Vector3 targetDirection = GetDirectionFromCurrentObjectToTargetObject(gameObjectRigidBody, targetObject);

        // Moves at 3/4 speed toward targets
        AddForceToRigidBody((targetDirection * speed * 0.75f), gameObjectRigidBody);
        TookAction();
    }

    private void MoveAwayFromCollisionObject(Rigidbody2D gameObjectBody, GameObject collisionObject)
    {
        // Calculate Force we should move object toward a given position
        Vector3 directionToMove = GetDirectionFromCurrentObjectToTargetObject(gameObjectRigidBody, collisionObject);

        // Moves at 1.5 speed away from taget
        AddForceToRigidBody(directionToMove * -1 * speed * 1.5f, gameObjectRigidBody);
        TookAction();
    }

    private static Vector3 GetDirectionFromCurrentObjectToTargetObject(Rigidbody2D gameObjectRigidBody, GameObject targetObject)
    {
        Vector3 targetObjectPosition = targetObject.transform.position;


        //Vector3 positionToMoveTo = Vector3.MoveTowards(gameObjectRigidBody.transform.position, targetObjectPosition, speed/2);
        //Debug.Log("Force to move the ai towards the play: " + positionToMoveTo + "\n" + "Target Direction: " + targetDirection);
        Vector3 targetDirection = (targetObjectPosition - gameObjectRigidBody.transform.position).normalized;
        return targetDirection;
    }

    private void ApplyForceToThrownObject(Vector2 forceToApply, GameObject objectToThrow)
    {
        AddForceToRigidBody(forceToApply, objectToThrow.GetComponent<Rigidbody2D>());
    }

    private void AddForceToRigidBody(Vector3 newForce, Rigidbody2D gameObjectRigidBody)
    {
        gameObjectRigidBody.AddForce(newForce);
    }

    private void dampenRigidBodyVelocity(Rigidbody2D gameObjectRigidBody)
    {
        Vector2 velocity = gameObjectRigidBody.velocity;
        float magnitude = velocity.magnitude;
        float exceedMaagnitudeAmount = maxMagnitude - magnitude;

        gameObjectRigidBody.AddForce(velocity * -1 * magnitude);
    }

    private bool exceedMaxMagnitude(Rigidbody2D gameObjectRigidBody)
    {
        return gameObjectRigidBody.velocity.magnitude > maxMagnitude;
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


    // Need to make some logic for how th ai deals with players
    /*
     1. If in range of play, change to toss a sparkle to attract ravers

     2. If in range of a rave ship, head towards it

     3. If in range of both player and rave boat, compare distance between player and rave boat and this unit and rave boat and determine what to do

     4. Wander
     */
    private MovementState CalculateMovementState
    {
        get
        {
            if (playerInCollider && raveBoatInCollider)
            {
                // This state is gravy if I have time
                return MovementState.ActualThoughtfulDecision;
            }
            else if (raveBoatInCollider)
            {
                return MovementState.DistractRaveBoats;
            }
            else if (playerInCollider)
            {
                return MovementState.MoveAwayFromPlayer;
            }
            return MovementState.Wander;
        }
    }

    //private void OnEnable()
    //{
    //    controls.Enable();
    //}

    //private void OnDisable()
    //{
    //    controls.Disable();
    //}
}
