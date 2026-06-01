using UnityEngine;
using UnityEngine.UI;

/// A data container attached to a UI button.
/// Passes app-specific information (target scene, title, description, and image) to the DetailsManager when clicked.

public class AppButtonData : MonoBehaviour
{
    public DetailsManager manager;

    [Header("App Information")]
    public string targetSceneName;
    public string appTitle; 
    [TextArea]
    public string appDescription;
    public Sprite appImage;

    private Button thisButton;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(SendMyData);
    }

    /// Sends the stored app data to the DetailsManager to update the detailed UI view.

    private void SendMyData()
    {
        if (manager != null)
        {
            manager.OpenDetails(targetSceneName, appTitle, appDescription, appImage);
        }
    }
}