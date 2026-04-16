using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTransition : MonoBehaviour
{
    private string currentTargetScene = "";

    // 1. The App button calls this to declare its specific destination
    public void SetTargetEnvironment(string sceneName)
    {
        currentTargetScene = sceneName;
    }

    // 2. The Play button calls this to execute the transition
    public void LoadEnvironment()
    {
        if (!string.IsNullOrEmpty(currentTargetScene))
        {
            SceneManager.LoadScene(currentTargetScene);
        }
        else
        {
            Debug.LogWarning("Portal failed: No scene name was set.");
        }
    }
}