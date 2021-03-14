using UnityEngine;

public abstract class AbsBoatMovementController : MonoBehaviour
{
    protected Rigidbody2D gameObjectRigidBody;
    protected Animator animator;

    [Range(0.01f, 20f)]
    public float speed = 10f;
    [Range(0.0f, 6f)]
    public float maxMagnitude = 2f;

    [Min(0)]
    public int raveMemberCount = 1;

    [Min(1)]
    public int maxRaveMemberCount = 3;

    protected abstract void MoveGameObjectRigidBody(Rigidbody2D gameObjectBody);

    public virtual void Update()
    {
        UpdateAnimationBasedOnSpeed(animator, gameObjectRigidBody);
    }

    // Add a force in a random direction
    protected virtual void Wander(Rigidbody2D gameObjectBody)
    {
        float xMove = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * speed;
        float yMove = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * speed;
        AddForceToRigidBody(new Vector2(xMove, yMove), gameObjectBody);
    }

    protected virtual void AddForceToRigidBody(Vector2 newForce, Rigidbody2D gameObjectRigidBody)
    {
        gameObjectRigidBody.AddForce(newForce);
    }

    protected virtual void AddForceToRigidBody(Vector3 newForce, Rigidbody2D gameObjectRigidBody)
    {
        gameObjectRigidBody.AddForce(newForce);
    }

    protected void DampenRigidBodyVelocity(Rigidbody2D gameObjectRigidBody)
    {
        Vector2 velocity = gameObjectRigidBody.velocity;
        float magnitude = velocity.magnitude;
        float exceedMaagnitudeAmount = maxMagnitude - magnitude;

        gameObjectRigidBody.AddForce(velocity * -1 * magnitude);
    }

    protected bool exceedMaxMagnitude(Rigidbody2D gameObjectRigidBody)
    {
        return gameObjectRigidBody.velocity.magnitude > maxMagnitude;
    }

    protected void UpdateAnimationBasedOnSpeed(Animator animator, Rigidbody2D gameObjectRigidBody)
    {
        Vector2 newVelocity = gameObjectRigidBody.velocity;
        float newXSpeed = newVelocity.x;
        float newYSpeed = newVelocity.y;

        // We only want to update the booleans if the new velocity is not zero. The player should maintain the current animation otherwise
        if (newXSpeed > Mathf.Epsilon || newXSpeed < Mathf.Epsilon || newYSpeed > Mathf.Epsilon || newYSpeed < Mathf.Epsilon)
        {
            bool posXSpeed = newXSpeed > Mathf.Epsilon;
            bool posYSpeed = newYSpeed > Mathf.Epsilon;
            bool xSpeedGreaterThanY = Mathf.Abs(newXSpeed) > Mathf.Abs(newYSpeed);

            //logNewAnimationBools(posXSpeed, posYSpeed, xSpeedGreaterThanY);
            //logAnimatorBools();

            animator.SetBool("PosXDir", posXSpeed);
            animator.SetBool("PosYDir", posYSpeed);
            animator.SetBool("GreaterXSpeed", xSpeedGreaterThanY);
        }
    }

    protected void logAnimatorBools()
    {
        // For checking whether the animator updated the variables or not
        bool animatorXDir = animator.GetBool("PosXDir");
        bool animatorYDir = animator.GetBool("PosYDir");
        bool animatorXGreater = animator.GetBool("GreaterXSpeed");
        Debug.Log("PosXDir: " + animatorXDir + "\n" +
            "PosYDir: " + animatorYDir + "\n" +
            "GreaterXSpeed: " + animatorXGreater + "\n");
    }

    protected void logNewAnimationBools(bool posXSpeed, bool posYSpeed, bool xSpeedGreaterThanY)
    {

        // For checking what the various bools we're about to set are
        Debug.Log("posXSpeed: " + posXSpeed + "\n" +
            "posYSpeed: " + posYSpeed + "\n" +
            "xSpeedGreaterThanY: " + xSpeedGreaterThanY);
    }

}