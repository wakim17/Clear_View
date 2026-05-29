using UnityEngine;

/// <summary>
/// Place this on an empty GameObject with a Trigger Collider (e.g., BoxCollider with IsTrigger = true).
/// When the player enters, it will trigger the Wisp to tell them about the ores.
/// </summary>
public class WispTriggerArea : MonoBehaviour
{
    [Tooltip("The tag of the player object that can trigger this area.")]
    public string playerTag = "Player";

    [Tooltip("The Wisp that will play the audio. If empty, it will auto-find it in the scene.")]
    public WispInteractable wisp;

    [Tooltip("The audio clip to play when the player enters the trigger. If left empty, it will play the default forest hint.")]
    public AudioClip customHintClip;

    [Tooltip("If true, the trigger can only be activated once.")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void Awake()
    {
        if (wisp == null)
        {
            wisp = FindFirstObjectByType<WispInteractable>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            
            if (wisp != null)
            {
                if (customHintClip != null)
                {
                    wisp.PlaySpecificClip(customHintClip);
                }
                else
                {
                    // Plays the default 'helpHintClip' from the Forest settings
                    wisp.PlayForestOreHint();
                }
            }
            else
            {
                Debug.LogWarning("WispTriggerArea: No WispInteractable found in the scene to play the hint!");
            }
        }
    }
}
