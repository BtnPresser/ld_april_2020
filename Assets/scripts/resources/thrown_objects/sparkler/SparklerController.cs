using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparklerController : MonoBehaviour
{
    [Range(0f, 12f)]
    public float spinSpeed = 6f;
    
    [Range(0f, 1f)]
    public float spinSlowThreshhold = 0.55f;

    [Range(1f, 8f)]
    public float sparklerLifetimeSeconds = 4.5f;
    private float timeSinceCreated = 0.0000001f;

    private Rigidbody2D sparklerRigidBody;

    private bool raveBoatInCollider = false;

    private void Awake()
    {
        sparklerRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckFizzle();
        RotateSparkler();
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        // If a rave boat is in the collider, we should move it toward the sparkler
        raveBoatInCollider = collision.gameObject.CompareTag("RaveBoat");

        if (raveBoatInCollider)
        {
            GameObject raveBoatObject = collision.gameObject;
            Vector3 directionToCollisionObject = GetDirectionFromCurrentObjectToTargetObject(sparklerRigidBody, raveBoatObject);


            RaveBoatController raveBoat = raveBoatObject.GetComponent<RaveBoatController>();
            Rigidbody2D raveBoatRigidBody = raveBoatObject.GetComponent<Rigidbody2D>();
            
            raveBoatRigidBody.AddForce(directionToCollisionObject * raveBoat.speed * 0.5f * -1f);
        }
    }

    private void RotateSparkler()
    {
        // Spin normal speed until the spin time is up, then slow down
        if (timeSinceCreated >= spinSlowThreshhold)
        {
            sparklerRigidBody.transform.Rotate(Vector3.forward * spinSpeed * (spinSlowThreshhold/timeSinceCreated));
        } else
        {
            sparklerRigidBody.transform.Rotate(Vector3.forward * spinSpeed);

        }
    }

    private void FixedUpdate()
    {
        timeSinceCreated += Time.deltaTime;
    }

    private void CheckFizzle()
    {
        if (timeSinceCreated >= sparklerLifetimeSeconds)
        {
            Destroy(gameObject);
        }
    }

    private static Vector3 GetDirectionFromCurrentObjectToTargetObject(Rigidbody2D gameObjectRigidBody, GameObject targetObject)
    {
        Vector3 targetObjectPosition = targetObject.transform.position;


        //Vector3 positionToMoveTo = Vector3.MoveTowards(gameObjectRigidBody.transform.position, targetObjectPosition, speed/2);
        //Debug.Log("Force to move the ai towards the play: " + positionToMoveTo + "\n" + "Target Direction: " + targetDirection);
        Vector3 targetDirection = (targetObjectPosition - gameObjectRigidBody.transform.position).normalized;
        return targetDirection;
    }
}
