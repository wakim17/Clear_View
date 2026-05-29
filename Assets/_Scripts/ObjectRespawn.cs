using UnityEngine;

/// <summary>
/// Respawns an object to its initial position if it falls below a certain height 
/// or gets too far away from its starting point.
/// </summary>
public class ObjectRespawn : MonoBehaviour
{
    [Header("Respawn Conditions")]
    [Tooltip("The object will respawn if its Y position drops below this value (e.g., falls on the floor).")]
    public float minHeight = -0.5f;

    [Tooltip("The object will respawn if it gets further than this distance from its starting position.")]
    public float maxDistance = 10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    private void Awake()
    {
        // Store the starting position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Grab the Rigidbody if it exists, to stop its movement upon respawn
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check if it fell too far down or was thrown too far away
        if (transform.position.y < minHeight || Vector3.Distance(transform.position, initialPosition) > maxDistance)
        {
            Respawn();
        }
    }

    /// <summary>
    /// Resets the object to its original position and stops its momentum.
    /// </summary>
    public void Respawn()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (rb != null)
        {
            // Reset physical momentum so it doesn't immediately slide or fly off the table again
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
