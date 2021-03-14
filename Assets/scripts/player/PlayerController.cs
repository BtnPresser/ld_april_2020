using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerController : AbsBoatMovementController
{
    public GameObject floatingText;

    private Keyboard kb;

    private float calculatedSprintMagnitude;
    public float sprintMagnitudeMultiplier = 1.5f;
    
    private float originalMaxMagnitude;
    private bool newRaveMemberPickedUp = false;
    private bool droppingOffRaveMembers = false;

    // Setup the Player
    private void Awake()
    {
        gameObjectRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        kb = InputSystem.GetDevice<Keyboard>();

        speed = 10f;

        raveMemberCount = 0;
        maxRaveMemberCount = 10;

        maxMagnitude = 2f;

        originalMaxMagnitude = maxMagnitude;
        calculatedSprintMagnitude = maxMagnitude * sprintMagnitudeMultiplier;
    }

    public override void Update()
    {
           base.Update();
           if (newRaveMemberPickedUp)
           {
               ShowRaveMemberPickupText();
           }

           if (droppingOffRaveMembers)
           {
               DropOffRaveMembers();
           }
    }

    public void FixedUpdate()
    {
        MoveGameObjectRigidBody(gameObjectRigidBody);
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
            if (raveMemberCount >= maxRaveMemberCount)
            {
                ShowFloatingText("Max Ravers!");
                raveMemberCount = maxRaveMemberCount; // In quick collisions, we can run over the max and then overflow into negatives
            }
            else
            {
                if ((raveMemberCount + raveBoatMemberCount) > maxRaveMemberCount)
                {
                    int maxRaversPlayerCanAcquire = maxRaveMemberCount - raveMemberCount;
                    raveMemberCount += maxRaversPlayerCanAcquire;

                    raveBoatController.raveMemberCount = raveBoatMemberCount - maxRaversPlayerCanAcquire;
                    Debug.Log("Picked up " + raveBoatMemberCount + " Rave Member(s)!");
                    newRaveMemberPickedUp = true;
                }
                else
                {
                    raveMemberCount += raveBoatMemberCount;
                    collision.gameObject.SetActive(false); // Set to inactive so the Pool can respawn this
                    Debug.Log("Picked up " + raveBoatMemberCount + " Rave Member(s)!");
                    newRaveMemberPickedUp = true;
                }
            }

        } else if (collision.gameObject.CompareTag("Lighthouse"))
        {
            if (raveMemberCount > 0)
            {
                // Do something similar to the above so that a lighthouse has a limit of Rave members they need to keep going
                int lightHouseRaveMemberCount = collision.gameObject.GetComponent<LightHouseController>().raveMemberCount += raveMemberCount;
                Debug.Log("Dropping off " + raveMemberCount + " Rave Members at lighthouse! \n" + "This LightHouse now has " + lightHouseRaveMemberCount + " Rave Member(s)");
                droppingOffRaveMembers = true;
            }

        }

    }

    protected override void MoveGameObjectRigidBody(Rigidbody2D gameObjectBody)
    {
        Vector2 newForce = new Vector2(GetXKeyDown(), GetYKeyDown());
        CheckSprintKey();

        /*float playerMagnitude = gameObjectBody.velocity.magnitude;
        Debug.Log("Current Velocity Is: " + (gameObjectBody.velocity) + "\n" +
            "Current Magnitude Is :" + playerMagnitude + "\n" +
            "Current new force is: " + newForce);*/
        /*Debug.Log("New Force is: " + newForce);*/

        if (exceedMaxMagnitude(gameObjectBody))
        {
            DampenRigidBodyVelocity(gameObjectBody);
        }
        else/* if (!newForce.Equals(Vector2.zero))*/
        {
            AddForceToRigidBody(newForce, gameObjectBody);
        }
    }

    private float GetYKeyDown()
    {
        if (kb.wKey.IsPressed())
        {
            return (speed * 1);
        }
        else if (kb.sKey.IsPressed())
        {
            return (speed * -1);
        }

        return 0f;
    }

    private float GetXKeyDown()
    {
        if (kb.dKey.IsPressed())
        {
            return (speed * 1);
        }
        else if (kb.aKey.IsPressed())
        {
            return (speed * -1);
        }

        return 0f;
    }

    private void CheckSprintKey()
    {
        if (kb.spaceKey.IsPressed())
        {
            maxMagnitude = calculatedSprintMagnitude;
        }
        else
        {
            maxMagnitude = originalMaxMagnitude;
        }
    }

    private void DropOffRaveMembers()
    {
        ShowRaveMemberDroppedOffText();
        raveMemberCount = 0;
        droppingOffRaveMembers = false;
    }

    private void ShowRaveMemberPickupText()
    {
        newRaveMemberPickedUp = false;
        ShowFloatingText(raveMemberCount + "/10");
    }

    private void ShowRaveMemberDroppedOffText()
    {
        ShowFloatingText("-" + raveMemberCount);
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
