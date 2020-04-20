using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSprite : MonoBehaviour
{
    private float yRotation;
    private bool invertYforSprite = false;

    [Range(0.5f, 3f)]
    public float spriteLifeTime = 1f;
    private float runningWaveLifeTime = 0f;

    private float waveSpeed = 8f;
    private Rigidbody2D waveRigidbody;


    // Start is called before the first frame update
    void Awake()
    {
        // Might not actually be useful since we want random chance everytime the wave appears
        waveRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Need to add a force and then make the wave's position offset along a sine wave for it's lifetime
        DisableWaveIfExceedingLifetime();
    }

    private void DisableWaveIfExceedingLifetime()
    {
        if(runningWaveLifeTime >= spriteLifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        runningWaveLifeTime += Time.deltaTime;
    }

    private void OnEnable()
    {
        float xSpeed = (invertYforSprite ? -1 : 1) * waveSpeed;
        float ySpeed = posOrNegOne() * (waveSpeed / 4);

        waveRigidbody.AddForce(new Vector2(xSpeed, ySpeed));
    }

    private void OnDisable()
    {
        runningWaveLifeTime = 0f;
        waveRigidbody.velocity = Vector3.zero;
        SetRandomRotation();
    }

    private void SetRandomRotation()
    {
        // Randomly reverse the scale
        if (posOrNegOne() == -1)
        {
            transform.Rotate(0f, 180f, 0f);
            invertYforSprite = true;
        } else
        {
            transform.rotation = Quaternion.identity;
            invertYforSprite = false;
        }
    }

    private static float posOrNegOne()
    {
        return UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1;
    }
}
