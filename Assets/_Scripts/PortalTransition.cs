using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalTransition : MonoBehaviour
{
    [Header("Portal Visuals")]
    public GameObject portalObject;
    public float transitionDelay = 1.5f;
    public float scaleUpDuration = 0.5f;

    private string targetScene = "";
    private bool isTransitioning = false;
    private Vector3 originalScale; // Store the scale from the Editor

    private void Start()
    {
        if (portalObject != null)
        {
            // Cache the scale before hiding it
            originalScale = portalObject.transform.localScale;
            portalObject.SetActive(false);
        }
    }

    public void SetTargetEnvironment(string sceneName)
    {
        targetScene = sceneName;
    }

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

    private IEnumerator TransitionSequence()
    {
        isTransitioning = true;

        if (portalObject != null)
        {
            portalObject.SetActive(true);
            portalObject.transform.localScale = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < scaleUpDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / scaleUpDuration);
                
                // Smooth step for a nicer, non-robotic animation
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                
                // Scale up to the original Editor scale, not just 1
                portalObject.transform.localScale = originalScale * smoothT;
                yield return null;
            }
            portalObject.transform.localScale = originalScale;
        }

        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(targetScene);
    }
}