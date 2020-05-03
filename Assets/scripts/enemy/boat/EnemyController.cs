using System;
using UnityEngine;

public class EnemyController : AbsBoatMovementController
{
    private const string sparklerTag = "Sparkler";

    [Range(20f, 50f)]
    public float throwingForce = 35f;

    [Range(0.05f, 10f)]
    public float timeBetweenSparklers = 0.5f;
    private float lastSparklerThrownTime = 0;

    public float sprintMagnitudeMultiplier = 1.3f;


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

        maxMagnitude = 1f;
        speed = 10f;

        if (randomTimeBetweeenActions) 
        {
            timeBetweenActions = UnityEngine.Random.Range(1f, 3f);
        }
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

    // Need to figure out how this is used here
    // Should we make the more complex AI boats inherit from another interface?
    protected override void MoveGameObjectRigidBody(Rigidbody2D gameObjectBody)
    {

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
            DampenRigidBodyVelocity(gameObjectBody);
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
            GameObject sparkler = ObjectPooler.Instance.SpawnFromPool(sparklerTag, transform.position);

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
    protected override void Wander(Rigidbody2D gameObjectRigidBody)
    {
        base.Wander(gameObjectRigidBody);
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

    private Vector3 GetDirectionFromCurrentObjectToTargetObject(Rigidbody2D gameObjectRigidBody, GameObject targetObject)
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
                //return MovementState.ActualThoughtfulDecision;
                return MovementState.DistractRaveBoats;
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
