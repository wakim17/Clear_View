using UnityEngine;

/// <summary>
/// Controls the core session loop and tracks the 15-minute time limit.
/// Triggers the final audio guide when the time expires.
/// </summary>
public class SessionManager : MonoBehaviour
{
    public enum SessionState
    {
        BootSequence,
        RemotePrompt,
        TVNavigation,
        EnvironmentTransition,
        SessionReview
    }

    [Header("State Tracking")]
    public SessionState currentState;

    [Header("Session Timing")]
    [Tooltip("Total allowed time in seconds (900 seconds = 15 minutes).")]
    public float maxSessionTime = 900f;

    [Header("Audio Guide")]
    [Tooltip("The audio source attached to the user's head to play the final message.")]
    public AudioSource guideAudioSource;
    public AudioClip finalExitMessage;

    private float currentSessionTime = 0f;

    private void Start()
    {
        SetState(SessionState.BootSequence);
    }

    private void Update()
    {
        if (currentState != SessionState.SessionReview)
        {
            currentSessionTime += Time.deltaTime;

            if (currentSessionTime >= maxSessionTime)
            {
                SetState(SessionState.SessionReview);
            }
        }
    }

    public void SetState(SessionState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case SessionState.SessionReview:
                // Play the final instruction directly into the user's ears.
                if (guideAudioSource != null && finalExitMessage != null)
                {
                    guideAudioSource.PlayOneShot(finalExitMessage);
                }
                break;
        }
    }
}