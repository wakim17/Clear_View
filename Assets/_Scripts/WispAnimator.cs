using UnityEngine;

/// <summary>
/// Gives life to the visual guide by applying a mathematical sine wave 
/// to its vertical position, creating a soft, continuous hovering effect.
/// </summary>
public class WispAnimator : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("How fast the wisp bobs up and down.")]
    public float hoverSpeed = 2f;
    [Tooltip("How high and low the wisp travels from its starting point.")]
    public float hoverHeight = 0.05f;

    // We store the original starting position so the wisp always hovers around its spawn point.
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Calculate the new Y position using a Sine wave based on the current game time.
        // This guarantees a perfectly smooth, endless looping animation without needing Unity's Animator window.
        float newY = startPosition.y + (Mathf.Sin(Time.time * hoverSpeed) * hoverHeight);

        // Apply the new vertical position while keeping X and Z exactly the same.
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}