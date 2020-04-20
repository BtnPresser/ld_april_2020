using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaveBoatSpawner : MonoBehaviour
{
    private string PoolTag = "RaveBoat";
    public BoundaryData gameBounds;

    [Range(0, 50)]
    public int numberOfRaveBoatsAllowedInGame = 30;

    [Range(0.001f, 1f)]
    public float timeBetweenRaveBoatSpawns = 4f;
    private float timeSinceLastRaveBoatSpawned = 0f;

    // Spawn should probably be done in Update and not FixedUpdate
    private void FixedUpdate()
    {
        if (ShouldRaveBoatSpawn())
        {
            ObjectPooler.Instance.SpawnFromPool(PoolTag, GetRandomPositionInGameBounds());
            timeSinceLastRaveBoatSpawned = 0f;
        }
    }

    private bool ShouldRaveBoatSpawn()
    {
        timeSinceLastRaveBoatSpawned += Time.deltaTime;
        if (timeSinceLastRaveBoatSpawned >= timeBetweenRaveBoatSpawns && ObjectPooler.Instance.PoolActiveCountForTag(PoolTag) < numberOfRaveBoatsAllowedInGame)
        {
            return true;
        }
        return false;
    }

    private Vector2 GetRandomPositionInGameBounds()
    {
        float randomXinGameBounds = UnityEngine.Random.Range(gameBounds.leftBound, gameBounds.rightBound);
        float randomYinGameBounds = UnityEngine.Random.Range(gameBounds.lowerBound, gameBounds.rightBound);
        Vector2 positionForRaveBoat = new Vector2(randomXinGameBounds, randomYinGameBounds);
        //Debug.Log("Attempting to place wave object at: " + positionForWave);

        return positionForRaveBoat;
    }
}
