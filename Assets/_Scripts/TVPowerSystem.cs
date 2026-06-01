using UnityEngine;
using System.Collections;

/// Manages the visual and audio sequence for turning on a virtual TV.
/// Uses a holographic split animation with simulated flickering.
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

    /// Caches the initial scale and sets up the CanvasGroup for opacity control.
    private void Awake()
    {
        if (hologramCanvas != null)
        {
            // Cache original scale and set up for activation sequence
            originalScale = hologramCanvas.transform.localScale;
            hologramCanvas.transform.localScale = Vector3.zero;

            // Ensure CanvasGroup is present for fading and flickering
            canvasGroup = hologramCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hologramCanvas.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            hologramCanvas.SetActive(false);
        }
    }

    /// Initiates the TV power-on sequence if it is not already powered on.
    public void PowerOn()
    {
        // This stops the TV from turning on twice if the remote is dropped and grabbed again.
        if (isPoweredOn == true) return;

        // Change the TV screen material to white
        if (screenRenderer != null && whiteScreenMaterial != null)
        {
            screenRenderer.material = whiteScreenMaterial;
        }

        // Open the holographic projection with animation
        if (hologramCanvas != null)
        {
            hologramCanvas.SetActive(true);
            if (activeAnimation != null) StopCoroutine(activeAnimation);
            activeAnimation = StartCoroutine(AnimateProjection());
        }

        // Play the start sound
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

    /// Coroutine that animates the holographic projection expanding and fading in.
    private IEnumerator AnimateProjection()
    {
        float elapsed = 0f;
        
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

                // cubic-out easing for X expansion
                float xVal = 1f - Mathf.Pow(1f - xTime, 3f);
                
                // elastic-out bounce easing for Y expansion
                float yVal = yTime == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * yTime) * Mathf.Sin((yTime - 0.075f) * (2f * Mathf.PI) / 0.3f);
                if (t < 0.4f) yVal = 0.01f; 
                
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
