using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private string PoolTag = "EnemyBoat";
    public BoundaryData gameBounds;

    [Range(0, 50)]
    public int numberOfEnemiesAllowedInGame = 15;

    [Range(0.001f, 1f)]
    public float timeBetweenEnemySpawns = 5f;
    private float timeSinceLastEnemySpawned = 0f;

    // Spawn should probably be done in Update and not FixedUpdate
    private void FixedUpdate()
    {
        if (ShouldEnemySpawn())
        {
            ObjectPooler.Instance.SpawnFromPool(PoolTag, GetRandomPositionInGameBounds());
            timeSinceLastEnemySpawned = 0f;
        }
    }

    private bool ShouldEnemySpawn()
    {
        timeSinceLastEnemySpawned += Time.deltaTime;
        if (timeSinceLastEnemySpawned >= timeBetweenEnemySpawns && ObjectPooler.Instance.PoolActiveCountForTag(PoolTag) < numberOfEnemiesAllowedInGame)
        {
            return true;
        }
        return false;
    }

    private Vector2 GetRandomPositionInGameBounds()
    {
        float randomXinGameBounds = UnityEngine.Random.Range(gameBounds.leftBound, gameBounds.rightBound);
        float randomYinGameBounds = UnityEngine.Random.Range(gameBounds.lowerBound, gameBounds.rightBound);
        Vector2 positionForEnemy = new Vector2(randomXinGameBounds, randomYinGameBounds);
        //Debug.Log("Attempting to place wave object at: " + positionForWave);

        return positionForEnemy;
    }
}
