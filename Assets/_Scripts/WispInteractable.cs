using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// Allows the player to interact with the wisp guide.
/// Toggles a floating UI menu with options to ask for help or return to the menu.
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

    /// Caches the original scale and initializes the AudioSource. Ensures the menu starts hidden.
    private void Awake()
    {
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; 
        audioSource.playOnAwake = false;

        if (wispMenuCanvas != null)
        {
            wispMenuCanvas.SetActive(false);
        }
    }

    /// Starts the welcome clip delay.
    private void Start()
    {
        StartCoroutine(PlayWelcomeClipWithDelay());
    }

    /// Checks if a pulse animation needs to be triggered.
    private void Update()
    {
        if (pulseOnInteraction && !isPulsing && audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(PulseSequence());
        }
    }

    /// Waits for a short duration before playing the appropriate welcome audio clip for the scene.
    private IEnumerator PlayWelcomeClipWithDelay()
    {
        yield return new WaitForSeconds(1f);

        if (audioSource == null) yield break;

        AudioClip clipToPlay = currentMode switch
        {
            LocationMode.LivingRoom => livingRoomWelcomeClip,
            LocationMode.Forest => forestWelcomeClip,
            LocationMode.Garden => gardenWelcomeClip,
            _ => null
        };

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    /// Core interaction method called by XRSimpleInteractable when the Wisp is clicked.
    /// Toggles the menu on and off.
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

        // Play feedback
        if (audioSource != null && interactionChime != null)
        {
            audioSource.PlayOneShot(interactionChime);
        }
    }

    /// Called by the UI "Help" Button. Plays specific help audio based on the current scene context.
    public void PlayHelpAudio()
    {
        if (wispMenuCanvas != null) wispMenuCanvas.SetActive(false);
        if (audioSource == null) return;

        StopCurrentAudio();

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
                    if (clipToPlay != null) audioSource.PlayOneShot(clipToPlay);
                    gardenHelpClipIndex = (gardenHelpClipIndex + 1) % gardenHelpClips.Length;
                }
                break;
        }
    }

    /// Called by CopperOre when an ore is collected. Increments counter and plays congratulatory audio.
    public void OnOreCollected()
    {
        if (currentMode != LocationMode.Forest) return;

        oresCollected = Mathf.Min(oresCollected + 1, totalOres);

        if (audioSource != null)
        {
            StopCurrentAudio();
            int oresLeft = totalOres - oresCollected;
            AudioClip remainingClip = GetRemainingOreClip(oresLeft);
            currentAudioCoroutine = StartCoroutine(PlaySequentialAudio(generalCongratsClip, remainingClip));
        }
    }

    /// Helper method to fetch the correct audio clip based on remaining ores.
    private AudioClip GetRemainingOreClip(int oresLeft)
    {
        if (remainingOresClips != null && oresLeft >= 0 && oresLeft < remainingOresClips.Length)
        {
            return remainingOresClips[oresLeft];
        }
        return null;
    }

    /// Stops any currently playing audio and running audio coroutines.
    private void StopCurrentAudio()
    {
        if (audioSource != null) audioSource.Stop();
        if (currentAudioCoroutine != null)
        {
            StopCoroutine(currentAudioCoroutine);
            currentAudioCoroutine = null;
        }
    }

    /// Plays two audio clips back-to-back using a coroutine.
    private IEnumerator PlaySequentialAudio(AudioClip firstClip, AudioClip secondClip)
    {
        if (firstClip != null)
        {
            audioSource.clip = firstClip;
            audioSource.Play();
            yield return new WaitForSeconds(firstClip.length + 0.1f); 
        }

        if (secondClip != null)
        {
            audioSource.clip = secondClip;
            audioSource.Play();
        }
    }

    /// Called by the UI "Main Menu" Button. Starts the scene loading sequence.
    public void ReturnToMenu()
    {
        if (isLoadingMenu) return;

        if (wispMenuCanvas != null) wispMenuCanvas.SetActive(false);

        if (currentMode == LocationMode.LivingRoom) return;

        if (pulseOnInteraction && !isPulsing)
        {
            StartCoroutine(PulseSequence());
        }

        StartCoroutine(LoadMenuRoutine());
    }

    /// Plays a default hint related to ores when the player enters a trigger area in the forest.
    public void PlayForestOreHint()
    {
        if (currentMode != LocationMode.Forest || audioSource == null || helpHintClip == null) return;

        StopCurrentAudio();
        audioSource.PlayOneShot(helpHintClip);
    }

    /// Plays a specific audio clip directly, stopping any currently playing clips.
    public void PlaySpecificClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        StopCurrentAudio();
        audioSource.PlayOneShot(clip);
    }

    /// Coroutine that handles waiting for animations to finish before loading the menu scene.
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

    /// Coroutine that scales the Wisp up and down briefly to provide visual feedback upon interaction.
    private IEnumerator PulseSequence()
    {
        isPulsing = true;
        
        float halfDuration = pulseDuration * 0.5f;
        
        float elapsed = 0f;
        Vector3 pulseScale = originalScale * pulseScaleMultiplier;
        
        // Scale up
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
