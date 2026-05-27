using UnityEngine;
using System.Collections;

public class TVPowerSystem : MonoBehaviour
{
    [Header("Visuals")]
    public MeshRenderer screenRenderer;
    public Material whiteScreenMaterial;
    public GameObject hologramCanvas;

    [Header("Audio")]
    public AudioSource powerAudio;
    [Tooltip("Optional custom audio clip for the projection animation.")]
    public AudioClip projectionAudioClip;

    [Header("Projection Animation")]
    [Tooltip("How long the projection opening animation takes in seconds.")]
    public float animationDuration = 0.6f;
    [Tooltip("Should it use a holographic split expansion (expand width first, then height)?")]
    public bool useHolographicSplit = true;
    [Tooltip("Enable subtle high-frequency signal flickering during the boot-up.")]
    public bool enableHolographicFlicker = true;

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private bool isPoweredOn = false;
    private Coroutine activeAnimation;

    private void Awake()
    {
        if (hologramCanvas != null)
        {
            // Cache original scale and set up for activation sequence
            originalScale = hologramCanvas.transform.localScale;
            hologramCanvas.transform.localScale = Vector3.zero;

            // Ensure CanvasGroup is present for opacity fading and flickering
            canvasGroup = hologramCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hologramCanvas.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            hologramCanvas.SetActive(false);
        }
    }

    public void PowerOn()
    {
        // This stops the TV from turning on twice if the remote is dropped and grabbed again.
        if (isPoweredOn == true) return;

        // 1. Change the screen material to white
        if (screenRenderer != null && whiteScreenMaterial != null)
        {
            screenRenderer.material = whiteScreenMaterial;
        }

        // 2. Open the holographic projection with animation
        if (hologramCanvas != null)
        {
            hologramCanvas.SetActive(true);
            if (activeAnimation != null) StopCoroutine(activeAnimation);
            activeAnimation = StartCoroutine(AnimateProjection());
        }

        // 3. Play the start sound
        if (powerAudio != null)
        {
            if (projectionAudioClip != null)
            {
                powerAudio.PlayOneShot(projectionAudioClip);
            }
            else
            {
                powerAudio.Play();
            }
        }

        isPoweredOn = true;
    }

    private IEnumerator AnimateProjection()
    {
        float elapsed = 0f;
        
        // Initial state reset
        hologramCanvas.transform.localScale = new Vector3(0f, 0f, originalScale.z);
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            if (useHolographicSplit)
            {
                // Holographic split: first 40% expands width (X), next 60% expands height (Y)
                float xTime = Mathf.Clamp01(t / 0.4f);
                float yTime = Mathf.Clamp01((t - 0.4f) / 0.6f);

                // Beautiful cubic-out easing for X expansion
                float xVal = 1f - Mathf.Pow(1f - xTime, 3f);
                
                // Premium elastic-out bounce easing for Y expansion
                float yVal = yTime == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * yTime) * Mathf.Sin((yTime - 0.075f) * (2f * Mathf.PI) / 0.3f);
                if (t < 0.4f) yVal = 0.01f; // keep Y very thin during X expand
                
                hologramCanvas.transform.localScale = new Vector3(
                    originalScale.x * xVal,
                    originalScale.y * yVal,
                    originalScale.z
                );
            }
            else
            {
                // Standard elastic-out scaling on all axes
                float scaleVal = t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.075f) * (2f * Mathf.PI) / 0.3f);
                hologramCanvas.transform.localScale = originalScale * scaleVal;
            }

            // Opacity and flicker logic
            if (canvasGroup != null)
            {
                float targetAlpha = Mathf.Min(1f, t * 1.5f);
                if (enableHolographicFlicker && t < 0.9f)
                {
                    // Introduce a sci-fi signal flicker that stabilizes near the end
                    float flicker = Random.Range(0.8f, 1.0f);
                    if (Random.value < 0.15f) flicker = Random.Range(0.4f, 0.7f); // brief drops
                    canvasGroup.alpha = targetAlpha * flicker;
                }
                else
                {
                    canvasGroup.alpha = targetAlpha;
                }
            }

            yield return null;
        }

        // Guarantee perfect final state
        hologramCanvas.transform.localScale = originalScale;
        if (canvasGroup != null) canvasGroup.alpha = 1f;
        activeAnimation = null;
    }
}