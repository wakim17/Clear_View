using UnityEngine;

/// Controls the movement and floating animation of the Wisp guide.
/// Can follow a target or move between waypoints.
public class WispAnimator : MonoBehaviour
{
    public enum NavigationMode { Sequential, Random, FollowPlayer }
    
    public NavigationMode navigationMode = NavigationMode.FollowPlayer;

    [Header("Navigation Setup")]
    public Transform[] waypoints;
    public Transform playerTransform;
    public Vector3 localFollowOffset = new Vector3(0.6f, -0.2f, 1.0f);

    [Header("Movement Tuning")]
    public float flySpeed = 1.5f;
    public float rotationSpeed = 2f;
    public float hoverSpeed = 0.5f;
    public float hoverHeight = 0.01f;

    private int waypointIndex = 0;
    private Vector3 basePosition;
    private Transform mainCameraTransform;

    /// Stores the initial starting position of the Wisp and caches the main camera.
    private void Start()
    {
        basePosition = transform.position;
        
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    /// Updates the Wisp's position and rotation every frame, applying a micro-hover effect.
    private void Update()
    {
        Vector3 targetPos = GetTargetPosition();
        
        // 1. Move the base position towards targetPos
        basePosition = Vector3.Lerp(basePosition, targetPos, Time.deltaTime * flySpeed);
        
        // 2. Apply a micro-hover on top of the base position
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = basePosition + new Vector3(0f, hoverOffset, 0f);

        // 3. Rotate to face the player or target (Horizontal only, to keep UI flat)
        Vector3 lookTarget = navigationMode == NavigationMode.FollowPlayer ? GetPlayerTransform().position : targetPos;
        
        // Flatten the target height so the Wisp doesn't pitch up/down
        lookTarget.y = transform.position.y;
        
        Vector3 lookDir = lookTarget - transform.position;
        
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        // 4. Waypoint Progression
        if (navigationMode != NavigationMode.FollowPlayer && waypoints.Length > 0)
        {
            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                waypointIndex = navigationMode == NavigationMode.Sequential 
                    ? (waypointIndex + 1) % waypoints.Length 
                    : Random.Range(0, waypoints.Length);
            }
        }
    }

    /// Calculates the next position the Wisp should move towards based on the player position.
    private Vector3 GetTargetPosition()
    {
        if (navigationMode == NavigationMode.FollowPlayer)
        {
            // TransformPoint applies the offset relative to the camera's local rotation
            return GetPlayerTransform().TransformPoint(localFollowOffset);
        }
        
        return waypoints.Length > 0 ? waypoints[waypointIndex].position : transform.position;
    }

    /// Retrieves the player's transform, falling back to the cached main camera if one is not assigned.
    private Transform GetPlayerTransform()
    {
        if (playerTransform != null) return playerTransform;
        return mainCameraTransform;
    }
}
