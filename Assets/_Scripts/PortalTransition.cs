using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// Handles a visual portal transition sequence before loading a new scene.
public class PortalTransition : MonoBehaviour
{
    [Header("Portal Visuals")]
    public GameObject portalObject;
    public float transitionDelay = 1.5f;

    private string targetScene = "";
    private bool isTransitioning = false;

    private void Start()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }
    }

    /// Sets the target scene to be loaded after the transition.
    public void SetTargetEnvironment(string sceneName)
    {
        targetScene = sceneName;
    }

    /// Starts the portal transition sequence if a target scene is set.
    public void LoadEnvironment()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("Portal: No target scene name was set.");
            return;
        }

        if (!isTransitioning)
        {
            StartCoroutine(TransitionSequence());
        }
    }

    /// Coroutine that activates the portal before loading the scene.
    private IEnumerator TransitionSequence()
    {
        isTransitioning = true;

        if (portalObject != null)
        {
            portalObject.SetActive(true);
        }

        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(targetScene);
    }
}
