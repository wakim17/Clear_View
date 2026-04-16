using UnityEngine;
using UnityEngine.UI;

public class AppButtonData : MonoBehaviour
{
    public DetailsManager manager;

    [Header("App Information")]
    public string targetSceneName;
    public string appTitle; // Added the new title variable
    [TextArea]
    public string appDescription;
    public Sprite appImage;

    private Button thisButton;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(SendMyData);
    }

    private void SendMyData()
    {
        if (manager != null)
        {
            // Now sending 4 pieces of information instead of 3
            manager.OpenDetails(targetSceneName, appTitle, appDescription, appImage);
        }
    }
}