using UnityEngine;

/// Attached to a Copper Ore parent object to handle collection logic in VR.
/// Hides the ore visual, plays a sound, and notifies the Wisp guide upon collection.

public class CopperOre : MonoBehaviour
{
    [Header("Ore Settings")]
    [Tooltip("The child GameObject that represents the actual ore (to be hidden on collection).")]
    public GameObject oreChild;

    [Header("Audio")]
    [Tooltip("Sound played locally when the ore is collected.")]
    public AudioClip collectSound;

    private bool isCollected = false;
    private AudioSource audioSource;
    private Collider oreCollider;
    private WispInteractable wisp;

    /// Initializes components and sets up an AudioSource .
    private void Awake()
    {
        // Cache components for better performance
        oreCollider = GetComponent<Collider>();
        wisp = FindFirstObjectByType<WispInteractable>();

        // Set up AudioSource
        audioSource = GetComponent<AudioSource>();

    }

    /// Call this method from XRSimpleInteractable Select event.
    public void CollectOre()
    {
        if (isCollected) return;
        isCollected = true;

        // Hide the ore visual child
        if (oreChild != null)
        {
            oreChild.SetActive(false);
        }

        // Play the local collection sound
        if (collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // Disable collider to prevent multiple interactions
        if (oreCollider != null)
        {
            oreCollider.enabled = false;
        }

        // Notify the Wisp manager
        if (wisp != null)
        {
            wisp.OnOreCollected();
        }
        else
        {
            Debug.LogWarning("CopperOre: Could not find WispInteractable in the scene to notify!");
        }
    }
}
