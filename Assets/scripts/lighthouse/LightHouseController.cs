using System;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightHouseController : MonoBehaviour
{
    private int previousRaveMemberCount = 0;
    public int raveMemberCount = 0;

    [Range(0, 50)]
    public int spinSpeed = 20;

    public GameObject floatingText;

    public Light2D spinningLight;

    private void Start()
    {
        spinningLight = gameObject.GetComponentInChildren<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (previousRaveMemberCount < raveMemberCount)
        {
            previousRaveMemberCount = raveMemberCount;
            ShowFloatingText();
        }

        SpinLight();
    }

    private void SpinLight()
    {
        spinningLight.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    private void ShowFloatingText()
    {
        GameObject go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = "" + raveMemberCount; // Weirdness if just doing .ToString()
    }
}
