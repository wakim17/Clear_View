using UnityEngine;

/// <summary>
/// A simple script to handle playing, pausing, and unpausing music on a vinyl player.
/// Make sure the GameObject has an AudioSource attached with your music clip assigned.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class VinylPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        // Get the AudioSource attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        
        // Ensure the audio is set to loop
        audioSource.loop = true;
    }

    /// <summary>
    /// Toggles the music between playing and paused.
    /// You can call this method from your XR Interactable events (like Select Entered or Activated).
    /// </summary>
    public void ToggleMusic()
    {
        if (audioSource.isPlaying)
        {
            // Pause the music
            audioSource.Pause();
        }
        else
        {
            // Resume/Play the music
            audioSource.Play();
        }
    }
}
