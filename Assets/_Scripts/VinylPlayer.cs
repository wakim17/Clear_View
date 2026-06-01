using UnityEngine;

/// A simple script to handle playing, pausing, and unpausing music on a vinyl player.\

[RequireComponent(typeof(AudioSource))]
public class VinylPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    /// Caches the required AudioSource and sets it to loop.
    private void Awake()
    {
        // Get the AudioSource attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        
        // Ensure the audio is set to loop
        audioSource.loop = true;
    }

    /// Toggles the music between playing and paused.
 
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
