using UnityEngine;

/// Tracks the session time limit.
/// Triggers a final audio guide when the time expires.
public class SessionManager : MonoBehaviour
{
    [Header("Session Timing")]
    [Tooltip("Total allowed time in seconds (900 seconds = 15 minutes).")]
    public float maxSessionTime = 900f;

    [Header("Audio Guide")]
    [Tooltip("The audio source attached to the user's head to play the final message.")]
    public AudioSource guideAudioSource;
    public AudioClip finalExitMessage;

    private float currentSessionTime = 0f;
    private bool hasPlayedMessage = false;

    /// Checks the elapsed time and plays the final exit message if the limit is reached.
    private void Update()
    {
        if (!hasPlayedMessage)
        {
            currentSessionTime += Time.deltaTime;

            if (currentSessionTime >= maxSessionTime)
            {
                hasPlayedMessage = true;
                
                if (guideAudioSource != null && finalExitMessage != null)
                {
                    guideAudioSource.PlayOneShot(finalExitMessage);
                }
            }
        }
    }
}
