using TMPro;
using UnityEngine;

public class LightHouseController : MonoBehaviour
{
    private int previousRaveMemberCount = 0;
    public int raveMemberCount = 0;

    public GameObject floatingText;
    

    // Update is called once per frame
    void Update()
    {
        if (previousRaveMemberCount < raveMemberCount)
        {
            previousRaveMemberCount = raveMemberCount;
            ShowFloatingText();
        }
    }

    private void ShowFloatingText()
    {
        GameObject go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = "" + raveMemberCount; // Weirdness if just doing .ToString()
    }
}
