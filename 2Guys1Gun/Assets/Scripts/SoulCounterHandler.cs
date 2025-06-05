using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoulCounterHandler : MonoBehaviour
{
    public TMP_Text soulCounterText;
    public Image Background;
    public Image SoulIcon;

    private int souls = 0;

    private Color originalTextColor;
    private Color flashTextColor = Color.red;

    private Vector3 originalScale;

    void Start()
    {
        soulCounterText.text = "0";
        originalTextColor = soulCounterText.color;
        originalScale = soulCounterText.transform.localScale;
    }

    private void Update()
    {
        //add 100 souls if i press numpad9
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            souls += 100;
            soulCounterText.text = souls.ToString();
            StartCoroutine(SmoothFlashEffect());
        }
    }

    public void UpdateSoulCounter()
    {
        souls++;
        soulCounterText.text = souls.ToString();

        StartCoroutine(SmoothFlashEffect());
    }

    private IEnumerator SmoothFlashEffect()
    {
        float duration = 0.3f;
        float time = 0f;

        // Get current scale to prevent stacking animation bugs
        Vector3 currentScale = soulCounterText.transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;

        // Set color flash start
        Color startColor = flashTextColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Color lerp
            soulCounterText.color = Color.Lerp(startColor, originalTextColor, t);

            // Scale lerp from current to original
            Vector3 lerpedScale = Vector3.Lerp(targetScale, originalScale, t);
            soulCounterText.transform.localScale = lerpedScale;
            Background.transform.localScale = lerpedScale;
            SoulIcon.transform.localScale = lerpedScale;

            yield return null;
        }

        // Final correction
        soulCounterText.color = originalTextColor;
        soulCounterText.transform.localScale = originalScale;
        Background.transform.localScale = originalScale;
        SoulIcon.transform.localScale = originalScale;
    }
}
