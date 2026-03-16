using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Panels")]
    public GameObject[] cutscenePanels;

    [Header("Overlays")]
    public Image blackOverlay;
    public Image flashOverlay;

    [Header("Title Card")]
    public TextMeshProUGUI titleCard;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip cutsceneMusic;
    public AudioClip flashSFX;

    [Header("Settings")]
    public float panelDuration = 3f;
    public float fadeDuration = 1f;
    public string nextSceneName = "GameLevel1";

    private void Start()
    {
        // Hide all panels at start
        foreach (var panel in cutscenePanels)
            panel.SetActive(false);

        titleCard.gameObject.SetActive(false);
        SetAlpha(blackOverlay, 1f);
        SetAlpha(flashOverlay, 0f);

        // Play music if assigned
        if (cutsceneMusic != null)
        {
            musicSource.clip = cutsceneMusic;
            musicSource.Play();
        }

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // Scene 1 - Parents holding baby
        yield return StartCoroutine(ShowPanel(0, panelDuration));

        // Scene 2 - Ray of light emerges
        yield return StartCoroutine(ShowPanel(1, 2f));

        // Blinding flash effect
        yield return StartCoroutine(FlashScreen());

        // Scene 3 - House destroyed
        yield return StartCoroutine(ShowPanel(2, panelDuration));

        // Scene 4 - House in darkness
        yield return StartCoroutine(FadeToBlack());
        yield return StartCoroutine(ShowPanel(3, panelDuration));

        // Title card - 10 Years Later
        yield return StartCoroutine(ShowTitleCard());

        // Scene 5 - Protagonist wakes up
        yield return StartCoroutine(ShowPanel(4, 3f));

        // Fade out and load game
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator ShowPanel(int index, float duration)
    {
        if (index >= cutscenePanels.Length) yield break;

        cutscenePanels[index].SetActive(true);
        yield return StartCoroutine(FadeOverlay(blackOverlay, 1f, 0f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeOverlay(blackOverlay, 0f, 1f));
        cutscenePanels[index].SetActive(false);
    }

    private IEnumerator FlashScreen()
    {
        if (flashSFX != null)
            musicSource.PlayOneShot(flashSFX);

        yield return StartCoroutine(FadeOverlay(flashOverlay, 0f, 1f, 0.2f));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeOverlay(flashOverlay, 1f, 0f, 0.8f));
    }

    private IEnumerator FadeToBlack()
    {
        yield return StartCoroutine(FadeOverlay(blackOverlay, 0f, 1f));
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator ShowTitleCard()
    {
        SetAlpha(blackOverlay, 1f);
        titleCard.gameObject.SetActive(true);

        Color c = titleCard.color;
        c.a = 0f;
        titleCard.color = c;

        // Fade text in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            titleCard.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        // Fade text out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - (t / fadeDuration));
            titleCard.color = c;
            yield return null;
        }

        titleCard.gameObject.SetActive(false);
    }

    private IEnumerator FadeOverlay(Image overlay, float from, float to, float duration = -1f)
    {
        if (duration < 0f) duration = fadeDuration;
        float t = 0f;
        SetAlpha(overlay, from);

        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(overlay, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }

        SetAlpha(overlay, to);
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
