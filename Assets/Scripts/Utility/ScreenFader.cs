using UnityEngine;
using UnityEngine.UI;

using System.Collections;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public float startAlpha = 1f;
    public float targetAlpha = 0f;
    public float delay = 0f;
    public float timeToFade = 1f;

    private float increment;
    private float currrentAlpha;

    private MaskableGraphic graphic;
    private Color originalColor;

    private void Start()
    {
        graphic = GetComponent<MaskableGraphic>();

        originalColor = graphic.color;
        currrentAlpha = startAlpha;

        Color tempColor =
            new Color(originalColor.r, originalColor.g, originalColor.b, currrentAlpha);
        graphic.color = tempColor;

        increment = (targetAlpha - startAlpha) / timeToFade * Time.deltaTime;

        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(delay);

        while (Mathf.Abs(targetAlpha - currrentAlpha) > 0.01f)
        {
            yield return new WaitForEndOfFrame();

            currrentAlpha += increment;

            Color tempColor =
                new Color(originalColor.r, originalColor.g, originalColor.b, currrentAlpha);
            graphic.color = tempColor;
        }
    }
}
