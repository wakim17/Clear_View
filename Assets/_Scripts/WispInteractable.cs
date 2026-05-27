using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Allows the player to interact with the magical wisp guide.
/// Toggles a floating UI menu with options to ask for help or return to the menu.
/// </summary>
public class WispInteractable : MonoBehaviour
{
    [Header("UI Menu")]
    [Tooltip("The Canvas GameObject that contains the Wisp's floating menu buttons.")]
    public GameObject wispMenuCanvas;

    [Header("Help Audio Settings")]
    [Tooltip("Voice clips played when the player clicks the Help button.")]
    public AudioClip[] helpClips;

    [Header("Menu Return Settings")]
    [Tooltip("The name of the main menu scene to load.")]
    public string menuSceneName = "Main_LivingRoom";
    
    [Tooltip("Wait time before loading the menu scene (allows pulse animation to finish).")]
    public float loadDelay = 0.4f;

    [Header("Magical Feedback")]
    [Tooltip("Instant chime sound played when clicking the wisp.")]
    public AudioClip interactionChime;

    [Header("Visual Click Pulse")]
    [Tooltip("Should the wisp scale up briefly when clicked?")]
    public bool pulseOnInteraction = true;
    public float pulseScaleMultiplier = 1.3f;
    public float pulseDuration = 0.25f;

    private AudioSource audioSource;
    private Vector3 originalScale;
    private bool isPulsing = false;
    private bool isLoadingMenu = false;
    private int helpClipIndex = 0;

    private void Awake()
    {
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.spatialBlend = 1f; 
        audioSource.playOnAwake = false;

        // Ensure the menu starts hidden so it doesn't block view automatically
        if (wispMenuCanvas != null)
        {
            wispMenuCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Core interaction method called by XRSimpleInteractable when the Wisp is clicked.
    /// Toggles the menu on and off.
    /// </summary>
    public void Interact()
    {
        if (isLoadingMenu) return;

        // Toggle the UI Menu visibility
        if (wispMenuCanvas != null)
        {
            wispMenuCanvas.SetActive(!wispMenuCanvas.activeSelf);
        }

        // Play magical feedback
        if (audioSource != null && interactionChime != null)
        {
            audioSource.PlayOneShot(interactionChime);
        }

        if (pulseOnInteraction && !isPulsing)
        {
            StartCoroutine(PulseSequence());
        }
    }

    /// <summary>
    /// Called by the UI "Help" Button.
    /// </summary>
    public void PlayHelpAudio()
    {
        if (helpClips == null || helpClips.Length == 0) return;
        
        if (audioSource != null)
        {
            AudioClip clipToPlay = helpClips[helpClipIndex];
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }

            // Cycle to next clip for next time
            helpClipIndex = (helpClipIndex + 1) % helpClips.Length;
        }
        
        // Hide menu after asking for help to keep the screen clear
        if (wispMenuCanvas != null)
        {
            wispMenuCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Called by the UI "Main Menu" Button.
    /// </summary>
    public void ReturnToMenu()
    {
        if (isLoadingMenu) return;
        
        // Hide the menu immediately
        if (wispMenuCanvas != null)
        {
            wispMenuCanvas.SetActive(false);
        }

        // Pulse for visual feedback that the button worked
        if (pulseOnInteraction && !isPulsing)
        {
            StartCoroutine(PulseSequence());
        }

        // Start loading sequence
        StartCoroutine(LoadMenuRoutine());
    }

    private IEnumerator LoadMenuRoutine()
    {
        isLoadingMenu = true;
        yield return new WaitForSeconds(loadDelay);

        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogWarning("WispInteractable: No menu scene name specified! Cannot return to menu.");
            isLoadingMenu = false;
        }
    }

    private IEnumerator PulseSequence()
    {
        isPulsing = true;
        
        float halfDuration = pulseDuration * 0.5f;
        
        // Scale up
        float elapsed = 0f;
        Vector3 pulseScale = originalScale * pulseScaleMultiplier;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, pulseScale, elapsed / halfDuration);
            yield return null;
        }

        // Scale back down
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(pulseScale, originalScale, elapsed / halfDuration);
            yield return null;
        }

        transform.localScale = originalScale;
        isPulsing = false;
    }
}
