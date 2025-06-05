using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoulCounterHandler : MonoBehaviour
{
    public TMP_Text soulCounterText;
    public Image Background;
    public Image SoulIcon;

    public int souls = 0;

    private const string SoulKey = "PlayerSoulCount";

    private Color originalTextColor;
    private Color flashTextColor = Color.red;

    private Vector3 originalScale;

    void Start()
    {
        souls = PlayerPrefs.GetInt(SoulKey, 0); // Load saved soul count
        soulCounterText.text = souls.ToString();
        originalTextColor = soulCounterText.color;
        originalScale = soulCounterText.transform.localScale;
    }

    private void Update()
    {
        // Add 100 souls with Numpad9 (for testing)
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            AddSouls(100);
        }
    }

    public void AddSouls(int amount)
    {
        souls += amount;
        PlayerPrefs.SetInt(SoulKey, souls); // Save updated soul count
        PlayerPrefs.Save();
        soulCounterText.text = souls.ToString();
        StartCoroutine(SmoothFlashEffect());
    }

    public void UpdateSoulCounter()
    {
        AddSouls(1);
    }

    public void RefreshSoulCounter()
    {
        soulCounterText.text = souls.ToString();
        StartCoroutine(SmoothFlashEffect());
    }

    private IEnumerator SmoothFlashEffect()
    {
        float duration = 0.3f;
        float time = 0f;

        Vector3 targetScale = originalScale * 1.3f;

        Color startColor = flashTextColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            soulCounterText.color = Color.Lerp(startColor, originalTextColor, t);

            Vector3 lerpedScale = Vector3.Lerp(targetScale, originalScale, t);
            soulCounterText.transform.localScale = lerpedScale;
            Background.transform.localScale = lerpedScale;
            SoulIcon.transform.localScale = lerpedScale;

            yield return null;
        }

        soulCounterText.color = originalTextColor;
        soulCounterText.transform.localScale = originalScale;
        Background.transform.localScale = originalScale;
        SoulIcon.transform.localScale = originalScale;
    }
}
