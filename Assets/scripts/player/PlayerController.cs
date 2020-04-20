using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D gameObjectRigidBody;
    private Animator animator;

    public GameObject floatingText;

    [Range(0.01f, 20f)]
    public float speed = 10f;
    [Range(0.0f, 6f)]
    public float maxMagnitude = 2f;
    private float defaultMaxMagnitude;
    public int raveMemberCount = 0;
    public int maxRaveMembers = 10;


    private Keyboard kb;
    
    private float sprintMagnitude;
    public float sprintMagnitudeMultiplier = 1.5f;

    // Setup the Player
    private void Awake()
    {
        gameObjectRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        kb = InputSystem.GetDevice<Keyboard>();

        defaultMaxMagnitude = maxMagnitude;
        sprintMagnitude = maxMagnitude * sprintMagnitudeMultiplier;
    }

    public void FixedUpdate()
    {
        MoveGameObjectRigidBody(gameObjectRigidBody);

        UpdatePlayerAnimationsBasedOnSpeed(animator, gameObjectRigidBody);
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
     /*   Debug.Log("A Collision Occured! \n Here's the collision tag: " + collision.tag +
            "Here's the gameObject tag: " + collision.gameObject.tag);*/
        
        if (collision.gameObject.CompareTag("RaveBoat"))
        {
            // It would be nice if RaveBoat was split into a classes where one represents their data and the other represents the controller
            RaveBoatController raveBoatController = collision.gameObject.GetComponent<RaveBoatController>();
            int raveBoatMemberCount = raveBoatController.raveMemberCount;
            if (raveMemberCount >= maxRaveMembers)
            {
                ShowFloatingText("Max Ravers!");
                raveMemberCount = maxRaveMembers; // In quick collisions, we can run over the max and then overflow into negatives
            }
            else
            {
                if ((raveMemberCount + raveBoatMemberCount) > maxRaveMembers)
                {
                    int maxRaversPlayerCanAcquire = maxRaveMembers - raveMemberCount;
                    raveMemberCount += maxRaversPlayerCanAcquire;

                    raveBoatController.raveMemberCount = raveBoatMemberCount - maxRaversPlayerCanAcquire;
                    Debug.Log("Picked up " + raveBoatMemberCount + " Rave Member(s)!");
                    ShowFloatingText();
                }
                else
                {
                    raveMemberCount += raveBoatMemberCount;
                    collision.gameObject.SetActive(false); // Set to inactive so the Pool can respawn this
                    Debug.Log("Picked up " + raveBoatMemberCount + " Rave Member(s)!");
                    ShowFloatingText();
                }
            }



        } else if (collision.gameObject.CompareTag("Lighthouse"))
        {
            if (raveMemberCount > 0)
            {
                // Do something similar to the above so that a lighthouse has a limit of Rave members they need to keep going
                int lightHouseRaveMemberCount = collision.gameObject.GetComponent<LightHouseController>().raveMemberCount += raveMemberCount;
                Debug.Log("Dropping off " + raveMemberCount + " Rave Members at lighthouse! \n" + "This LightHouse now has " + lightHouseRaveMemberCount + " Rave Member(s)");
                ShowFloatingText("-" + raveMemberCount.ToString());
                raveMemberCount = 0;
            }

        }

    }

    private void MoveGameObjectRigidBody(Rigidbody2D gameObjectBody)
    {
        Vector2 newForce = new Vector2(0, 0);

        if (kb.wKey.IsPressed())
        {
            newForce.y = 1 * speed;
        }
        else if (kb.sKey.IsPressed())
        {
            newForce.y = -1 * speed;
        }

        if (kb.dKey.IsPressed())
        {
            newForce.x = 1 * speed;
        }
        else if (kb.aKey.IsPressed())
        {
            newForce.x = -1 * speed;
        }

        if (kb.spaceKey.IsPressed())
        {
            maxMagnitude = sprintMagnitude;
        } else
        {
            maxMagnitude = defaultMaxMagnitude;
        }

        float playerMagnitude = gameObjectBody.velocity.magnitude;
        /*Debug.Log("Current Velocity Is: " + (playerBody.velocity) + "\n" +
            "Current Magnitude Is :" + playerMagnitude);*/

        if (exceedMaxMagnitude(gameObjectBody))
        {
            dampenRigidBodyVelocity(gameObjectBody);
        } else
        {
            AddForceToRigidBody(newForce, gameObjectRigidBody);
        }
    }

    private void AddForceToRigidBody(Vector2 newForce, Rigidbody2D gameObjectRigidBody)
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

    private void UpdatePlayerAnimationsBasedOnSpeed(Animator animator, Rigidbody2D gameObjectRigidBody)
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

    private void ShowFloatingText()
    {
        ShowFloatingText(raveMemberCount + "/10");
    }

    private void ShowFloatingText(string textToShow)
    {
        GameObject go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = textToShow;
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
