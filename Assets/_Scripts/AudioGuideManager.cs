using UnityEngine;

/// <summary>
/// Listens to the SessionManager and plays the correct voiceover for the current state.
/// </summary>
public class AudioGuideManager : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("The audio source attached to a physical object in the room (e.g., a smart speaker).")]
    public AudioSource speakerSource;

    [Header("Voiceover Clips")]
    public AudioClip welcomeClip;
    public AudioClip promptRemoteClip;
    public AudioClip promptTVClip;

    /// <summary>
    /// Called by the SessionManager whenever the application state changes.
    /// Plays the appropriate audio instruction.
    /// </summary>
    public void PlayStateAudio(SessionManager.SessionState newState)
    {
        // Stop any currently playing audio to prevent voices overlapping.
        speakerSource.Stop();

        switch (newState)
        {
            case SessionManager.SessionState.BootSequence:
                speakerSource.PlayOneShot(welcomeClip);
                break;
            case SessionManager.SessionState.RemotePrompt:
                speakerSource.PlayOneShot(promptRemoteClip);
                break;
            case SessionManager.SessionState.TVNavigation:
                speakerSource.PlayOneShot(promptTVClip);
                break;
        }
    }
}