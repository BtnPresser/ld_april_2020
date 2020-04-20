using UnityEngine;

public class BoundaryData : MonoBehaviour
{
    public float leftBound;
    public float rightBound;
    public float upperBound;
    public float lowerBound;

    private void Awake()
    {
        BoxCollider2D colliderWithGreatestX = null;
        BoxCollider2D colliderWithGreatestY = null;
        BoxCollider2D colliderWithLowestX = null;
        BoxCollider2D colliderWithLowestY = null;

        float greatestX = 0f;
        float greatestY = 0f;
        float leastX = 0f;
        float leastY = 0f;

        BoxCollider2D[] boxCollider2D = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D collider in boxCollider2D)
        {
            float x = collider.transform.position.x;
            float y = collider.transform.position.y;

            if (x > greatestX)
            {
                colliderWithGreatestX = collider;
                greatestX = x;
            }
            if (x < leastX)
            {
                colliderWithLowestX = collider;
                leastX = x;
            }
            if (y > greatestY)
            {
                colliderWithGreatestY = collider;
                greatestY = y;
            }
            if (y < leastY)
            {
                colliderWithLowestY = collider;
                leastY = y;
            }
        }

        leftBound = colliderWithLowestX.transform.position.x + colliderWithLowestX.bounds.size.x;
        rightBound = colliderWithGreatestX.transform.position.x - colliderWithGreatestX.bounds.size.x;
        upperBound = colliderWithGreatestY.transform.position.y - colliderWithGreatestY.bounds.size.y;
        lowerBound = colliderWithLowestY.transform.position.y + colliderWithLowestY.bounds.size.y * 5; // Not sure what is up with this lower bound, so just multiple the box size


        // Debug.Log("Map bounds should be: " + eftBound + "," + rightBound + "," + upperBound + "," + lowerBound);
    }
}
