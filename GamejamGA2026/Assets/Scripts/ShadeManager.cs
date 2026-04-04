using UnityEngine;

public class ShadeManager : MonoBehaviour
{
    [SerializeField]
    Material shadeMaterial;

    private float shadeAlpha = 1f;

    bool isFadingIn = false;

    public void FadeIn(float duration)
    {
        if (!isFadingIn)
        {
            StopAllCoroutines();
        }
        isFadingIn = true;
        StartCoroutine(FadeInCoroutine(duration));
    }

    public void FadeOut(float duration)
    {
        if (isFadingIn)
        {
            StopAllCoroutines();
        }
        isFadingIn = false;
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeInCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shadeAlpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            shadeMaterial.SetFloat("_Radius", shadeAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shadeAlpha = 0f;
        shadeMaterial.SetFloat("_Radius", shadeAlpha);
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shadeAlpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            shadeMaterial.SetFloat("_Radius", shadeAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shadeAlpha = 1f;
        shadeMaterial.SetFloat("_Radius", shadeAlpha);
    }   

    private void Update()
    {
        // For testing purposes, you can trigger fade in/out with keys
        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeIn(2f); // Fade in over 2 seconds
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            FadeOut(0f); // Instantly fade out
        }
    }

}
