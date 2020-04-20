using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    [Range(0, 50)]
    public int numberOfWavesAllowedOnScreen = 15;

    [Range(0.001f, 1f)]
    public float timeBetweenWaveSpawns = 0.1f;
    private float timeSinceLastWaveSpawned = 0f;

    private void Awake()
    {
        camera = Camera.main;
    }

    // Spawn should probably be done in Update and not FixedUpdate
    private void FixedUpdate()
    {
        if (WaveShouldSpawn())
        {
            ObjectPooler.Instance.SpawnFromPool("Wave", GetRandomPositionInViewport());
            timeSinceLastWaveSpawned = 0f;
        }
    }

    private bool WaveShouldSpawn()
    {
        timeSinceLastWaveSpawned += Time.deltaTime;
        if (timeSinceLastWaveSpawned >= timeBetweenWaveSpawns)
        {
            return true;
        }
        return false;
    }

    private Vector2 GetRandomPositionInViewport()
    {
        Rect viewportPixelRect = camera.pixelRect;
        // Use this to expand past the viewport a bit in order to create waves for players moving a little bit faster
        const int offset = 1;

        Vector2 screenBounds = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 screenOrigo = camera.ScreenToWorldPoint(Vector2.zero);

        float randomXinViewport = UnityEngine.Random.Range(screenOrigo.x - offset, screenBounds.x + offset);
        float randomYinViewport = UnityEngine.Random.Range(screenOrigo.y - offset, screenBounds.y + offset);
        Vector2 positionForWave = new Vector2(randomXinViewport, randomYinViewport);
        //Debug.Log("Attempting to place wave object at: " + positionForWave);

        return positionForWave;
    }
}
