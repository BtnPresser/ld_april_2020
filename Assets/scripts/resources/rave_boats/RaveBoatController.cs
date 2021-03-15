using UnityEngine;

public class RaveBoatController : AbsBoatMovementController
{
    public float sprintMagnitudeMultiplier = 1.3f;

    [Range(1, 3)]
    public float timeBetweenActionsUpperBound = 0f;
    public float timeBetweenActionsLowerBound = 0f;
    private float timeSinceLastAction = 0f;
    public float timeBetweenActions = 1.5f;

    public bool randomTimeBetweeenActions = false;


    // Setup the Player
    private void Awake()
    {
        gameObjectRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        maxMagnitude = 1f;
        speed = 7f;
        maxRaveMemberCount = 4;

        if (randomTimeBetweeenActions)
        {
            timeBetweenActions = UnityEngine.Random.Range(1f, 2f);
        }

        raveMemberCount = UnityEngine.Random.Range(1, maxRaveMemberCount);
    }

    public void FixedUpdate()
    {
        if (CanMakeAction())
        {
            MoveGameObjectRigidBody(gameObjectRigidBody);
        }
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        // Check if there is an attractor or stranded raver and add forces towards it when possible
    }

    protected override void MoveGameObjectRigidBody(Rigidbody2D gameObjectBody)
    {
        Wander(gameObjectRigidBody);
    }

    // Add a force in a random direction
    protected override void Wander(Rigidbody2D gameObjectRigidBody)
    {
        base.Wander(gameObjectRigidBody);
        TookAction();
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
}
