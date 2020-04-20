using UnityEngine;

public abstract class AGameObjectMovementController : MonoBehaviour
{
    private Rigidbody2D gameObjectRigidBody;
    private Animator animator;

    [Range(0.01f, 20f)]
    public float speed = 10f;
    [Range(0.0f, 6f)]
    public float maxMagnitude = 2f;
    private float defaultMaxMagnitude;
    public int raveMemberCount;

    public abstract void MoveGameObjectRigidBody(Rigidbody2D gameObjectBody);

    public void Awake()
    {
        gameObjectRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    public void FixedUpdate()
    {
        MoveGameObjectRigidBody(gameObjectRigidBody);

        UpdatePlayerAnimationsBasedOnSpeed(animator, gameObjectRigidBody);
    }

    public void AddForceToRigidBody(Vector2 newForce)
    {
        gameObjectRigidBody.AddForce(newForce);
    }

    public void dampenRigidBodyVelocity(Rigidbody2D gameObjectRigidBody)
    {
        Vector2 velocity = gameObjectRigidBody.velocity;
        float magnitude = velocity.magnitude;
        float exceedMaagnitudeAmount = maxMagnitude - magnitude;

        gameObjectRigidBody.AddForce(velocity * -1 * magnitude);
    }

    public float DecaySpeed(float prevSpeedVal)
    {
        return prevSpeedVal * Time.deltaTime;
    }

    public bool exceedMaxMagnitude(Rigidbody2D gameObjectRigidBody)
    {
        return gameObjectRigidBody.velocity.magnitude < maxMagnitude;
    }

    public void logAnimatorBools()
    {
        // For checking whether the animator updated the variables or not
        bool animatorXDir = animator.GetBool("PosXDir");
        bool animatorYDir = animator.GetBool("PosYDir");
        bool animatorXGreater = animator.GetBool("GreaterXSpeed");
        Debug.Log("PosXDir: " + animatorXDir + "\n" +
            "PosYDir: " + animatorYDir + "\n" +
            "GreaterXSpeed: " + animatorXGreater + "\n");
    }

    public void UpdatePlayerAnimationsBasedOnSpeed(Animator animator, Rigidbody2D gameObjectRigidBody)
    {
        Vector2 newVelocity = gameObjectRigidBody.velocity;
        float newXSpeed = newVelocity.x;
        float newYSpeed = newVelocity.y;

        // We only want to update the booleans if the new velocity is not zero. The player should maintain the current animation otherwise
        if (newXSpeed != 0f && newYSpeed != 0f)
        {
            bool posXSpeed = newXSpeed > 0;
            bool posYSpeed = newYSpeed > 0;
            bool xSpeedGreaterThanY = Mathf.Abs(newXSpeed) > Mathf.Abs(newYSpeed);

            //logNewAnimationBools(posXSpeed, posYSpeed, xSpeedGreaterThanY);
            //logAnimatorBools();

            animator.SetBool("PosXDir", posXSpeed);
            animator.SetBool("PosYDir", posYSpeed);
            animator.SetBool("GreaterXSpeed", xSpeedGreaterThanY);
        }
    }
    public void logNewAnimationBools(bool posXSpeed, bool posYSpeed, bool xSpeedGreaterThanY)
    {

        // For checking what the various bools we're about to set are
        Debug.Log("posXSpeed: " + posXSpeed + "\n" +
            "posYSpeed: " + posYSpeed + "\n" +
            "xSpeedGreaterThanY: " + xSpeedGreaterThanY);
    }

}