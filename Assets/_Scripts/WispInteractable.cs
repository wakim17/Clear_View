using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Allows the player to interact with the magical wisp guide.
/// Toggles a floating UI menu with options to ask for help or return to the menu.
/// </summary>
public class WispInteractable : MonoBehaviour
{
    // TEMPLATE FOR ADDING ANOTHER ENVIRONMENT:
    // 1. Add your new environment to the enum below (e.g., LivingRoom, Forest, Garden, NewArea).
    // 2. Create a new section of variables with [Header("NewArea Settings")].
    // 3. Go to the PlayHelpAudio() method and add a 'case LocationMode.NewArea:' to handle the logic.
    public enum LocationMode { LivingRoom, Forest, Garden }

    [Header("Context")]
    [Tooltip("Set this to match the scene to change how the Wisp behaves.")]
    public LocationMode currentMode = LocationMode.LivingRoom;

    [Header("UI Menu")]
    [Tooltip("The Canvas GameObject that contains the Wisp's floating menu buttons.")]
    public GameObject wispMenuCanvas;

    [Header("Living Room Settings")]
    [Tooltip("Played once upon loading the Living Room scene, after a 1 second delay.")]
    public AudioClip livingRoomWelcomeClip;
    [Tooltip("Voice clips played when the player clicks the Help button in the living room.")]
    public AudioClip[] helpClips;
    
    [Header("Forest Settings")]
    [Tooltip("Played once upon loading the Forest scene, after a 1 second delay.")]
    public AudioClip forestWelcomeClip;
    public int totalOres = 5;
    [Tooltip("Played when an ore is collected (e.g., 'Good job!')")]
    public AudioClip generalCongratsClip;
    [Tooltip("Played on Help click (e.g., 'Ores are in the area.')")]
    public AudioClip helpHintClip;
    [Tooltip("Array index = number of ores left (0 to 5)")]
    public AudioClip[] remainingOresClips;

    private int oresCollected = 0;

    [Header("Garden Settings")]
    [Tooltip("Played once upon loading the Garden scene, after a 1 second delay.")]
    public AudioClip gardenWelcomeClip;
    [Tooltip("Voice clips played when the player clicks the Help button in the garden.")]
    public AudioClip[] gardenHelpClips;

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
    private int gardenHelpClipIndex = 0;
    private Coroutine currentAudioCoroutine;

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

    private void Start()
    {
        StartCoroutine(PlayWelcomeClipWithDelay());
    }

    private void Update()
    {
        if (pulseOnInteraction && !isPulsing && audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(PulseSequence());
        }
    }

    private IEnumerator PlayWelcomeClipWithDelay()
    {
        yield return new WaitForSeconds(1f);

        if (audioSource == null) yield break;

        AudioClip clipToPlay = null;
        switch (currentMode)
        {
            case LocationMode.LivingRoom:
                clipToPlay = livingRoomWelcomeClip;
                break;
            case LocationMode.Forest:
                clipToPlay = forestWelcomeClip;
                break;
            case LocationMode.Garden:
                clipToPlay = gardenWelcomeClip;
                break;
        }

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    /// <summary>
    /// Core interaction method called by XRSimpleInteractable when the Wisp is clicked.
    /// Toggles the menu on and off.
    /// </summary>
    public void Interact()
    {
        if (isLoadingMenu) return;

        if (pulseOnInteraction && !isPulsing)
        {
            StartCoroutine(PulseSequence());
        }

        // If we are in the Living Room, skip the menu and directly play help audio
        if (currentMode == LocationMode.LivingRoom)
        {
            PlayHelpAudio();
            return;
        }

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
    }

    /// <summary>
    /// Called by the UI "Help" Button.
    /// </summary>
    public void PlayHelpAudio()
    {
        // Hide menu after asking for help to keep the screen clear
        if (wispMenuCanvas != null)
        {
            wispMenuCanvas.SetActive(false);
        }

        if (audioSource != null)
        {
            audioSource.Stop(); // Stop any current audio
            if (currentAudioCoroutine != null)
            {
                StopCoroutine(currentAudioCoroutine);
            }

            switch (currentMode)
            {
                // TEMPLATE FOR ADDING ANOTHER ENVIRONMENT:
                // case LocationMode.NewArea:
                //     // Add logic here, such as:
                //     // audioSource.PlayOneShot(newAreaClip);
                //     break;

                case LocationMode.LivingRoom:
                    if (helpClips != null && helpClips.Length > 0)
                    {
                        AudioClip clipToPlay = helpClips[helpClipIndex];
                        if (clipToPlay != null)
                        {
                            if (interactionChime != null)
                            {
                                currentAudioCoroutine = StartCoroutine(PlaySequentialAudio(interactionChime, clipToPlay));
                            }
                            else
                            {
                                audioSource.PlayOneShot(clipToPlay);
                            }
                        }
                        helpClipIndex = (helpClipIndex + 1) % helpClips.Length;
                    }
                    else if (interactionChime != null)
                    {
                        audioSource.PlayOneShot(interactionChime);
                    }
                    break;
                case LocationMode.Forest:
                    int oresLeft = totalOres - oresCollected;
                    AudioClip remainingClip = GetRemainingOreClip(oresLeft);
                    
                    if (helpHintClip != null || remainingClip != null)
                    {
                        currentAudioCoroutine = StartCoroutine(PlaySequentialAudio(helpHintClip, remainingClip));
                    }
                    break;
                case LocationMode.Garden:
                    if (gardenHelpClips != null && gardenHelpClips.Length > 0)
                    {
                        AudioClip clipToPlay = gardenHelpClips[gardenHelpClipIndex];
                        if (clipToPlay != null)
                        {
                            audioSource.PlayOneShot(clipToPlay);
                        }
                        gardenHelpClipIndex = (gardenHelpClipIndex + 1) % gardenHelpClips.Length;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Called by CopperOre when an ore is collected.
    /// </summary>
    public void OnOreCollected()
    {
        if (currentMode != LocationMode.Forest) return;

        oresCollected++;
        if (oresCollected > totalOres) oresCollected = totalOres; // Cap it just in case

        if (audioSource != null)
        {
            // If already playing audio, stop it so we can play the congrats immediately
            audioSource.Stop(); 
            if (currentAudioCoroutine != null)
            {
                StopCoroutine(currentAudioCoroutine);
            }
            
            int oresLeft = totalOres - oresCollected;
            AudioClip remainingClip = GetRemainingOreClip(oresLeft);
            currentAudioCoroutine = StartCoroutine(PlaySequentialAudio(generalCongratsClip, remainingClip));
        }
    }

    private AudioClip GetRemainingOreClip(int oresLeft)
    {
        if (remainingOresClips != null && oresLeft >= 0 && oresLeft < remainingOresClips.Length)
        {
            return remainingOresClips[oresLeft];
        }
        return null;
    }

    private IEnumerator PlaySequentialAudio(AudioClip firstClip, AudioClip secondClip)
    {
        if (firstClip != null)
        {
            audioSource.clip = firstClip;
            audioSource.Play();
            yield return new WaitForSeconds(firstClip.length + 0.1f); // small gap
        }

        if (secondClip != null)
        {
            audioSource.clip = secondClip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Called by the UI "Main Menu" Button.
    /// </summary>
    public void ReturnToMenu()
    {
        if (isLoadingMenu) return;

        // The living room IS the main menu, so don't reload the scene if we are already here.
        if (currentMode == LocationMode.LivingRoom)
        {
            if (wispMenuCanvas != null)
            {
                wispMenuCanvas.SetActive(false);
            }
            return;
        }
        
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

    public void PlayForestOreHint()
    {
        if (currentMode != LocationMode.Forest) return;

        if (audioSource != null && helpHintClip != null)
        {
            audioSource.Stop();
            if (currentAudioCoroutine != null) StopCoroutine(currentAudioCoroutine);

            audioSource.PlayOneShot(helpHintClip);
        }
    }

    public void PlaySpecificClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            if (currentAudioCoroutine != null) StopCoroutine(currentAudioCoroutine);

            audioSource.PlayOneShot(clip);
        }
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
