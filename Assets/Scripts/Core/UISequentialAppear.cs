using UnityEngine;
using System.Collections;

// Para que las animaciones de aparecer lo botones, sean secuenciales, con un pequeño delay entre cada uno

public class UISequentialAppear : MonoBehaviour
{
    public GameObject[] elements;
    public float delayBetween = 0.08f;

    public void PlayAnimation()
    {
        StartCoroutine(ShowElements());
    }

    IEnumerator ShowElements()
    {
        foreach (GameObject element in elements)
        {
            element.SetActive(true);
            yield return new WaitForSeconds(delayBetween);
        }
    }
}