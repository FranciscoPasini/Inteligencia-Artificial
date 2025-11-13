using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; 
    [SerializeField] private float fadeInDuration = 0.8f;
    [SerializeField] private float visibleDuration = 10.0f;
    [SerializeField] private float fadeOutDuration = 0.8f;
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private Button skipButton; 

    private bool isSkipping = false;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(() => Skip());
        }
    }

    void Start()
    {
        StartCoroutine(PlaySplash());
    }

    IEnumerator PlaySplash()
    {
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));

        // Tiempo visible (puede saltarse)
        float timer = 0f;
        while (timer < visibleDuration && !isSkipping)
        {
            timer += Time.unscaledDeltaTime; 
            yield return null;
        }

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));

        // Cargar la siguiente escena
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float val = Mathf.Lerp(from, to, duration <= 0 ? 1f : t / duration);
            if (canvasGroup != null) canvasGroup.alpha = val;
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = to;
    }

    public void Skip()
    {
        if (!isSkipping)
        {
            isSkipping = true;
            // opcional: detener todas las corrutinas y lanzar la transición inmediatamente
            StopAllCoroutines();
            StartCoroutine(SkipAndLoad());
        }
    }

    IEnumerator SkipAndLoad()
    {
        // Rápido fade out (o directo LoadScene)
        yield return StartCoroutine(Fade(canvasGroup != null ? canvasGroup.alpha : 1f, 0f, 0.2f));
        SceneManager.LoadScene(nextSceneName);
    }
}
