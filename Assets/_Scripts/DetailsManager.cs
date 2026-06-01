using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

/// Manages the UI transitions and data display for the main menu.
/// Handles switching between the grid and detail views, and loads target scenes.

public class DetailsManager : MonoBehaviour
{
    [Header("Menu Screens")]
    public GameObject gridMenu;
    public GameObject detailsMenu;

    [Header("Details Visuals")]
    public Image displayImage;
    public TextMeshProUGUI displayTitle; 
    public TextMeshProUGUI displayDescription; 

    [Header("Transition")]
    [Tooltip("Optional portal transition system. If assigned, loading the target scene will trigger the portal transition effect.")]
    public PortalTransition portalTransition;

    private string currentTargetScene = "";

    /// Populates the details menu with the specified data and makes it visible.
    /// "sceneName" The target scene to load if selected.
    /// "title" The title of the app.
    /// "description" A description of the app.
    /// "previewImage" An image preview for the app.

    public void OpenDetails(string sceneName, string title, string description, Sprite previewImage)
    {
        currentTargetScene = sceneName;
        displayTitle.text = title;
        displayDescription.text = description;
        displayImage.sprite = previewImage;

        gridMenu.SetActive(false);
        detailsMenu.SetActive(true);
    }

    /// Closes the details menu and returns the user to the main grid view.
    public void GoBack()
    {
        detailsMenu.SetActive(false);
        gridMenu.SetActive(true);
    }

    /// Initiates loading the target scene, using a portal transition.
    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(currentTargetScene))
        {
            if (portalTransition != null)
            {
                // Trigger the portal transition
                portalTransition.SetTargetEnvironment(currentTargetScene);
                portalTransition.LoadEnvironment();

                // Close the details menu so the user can see the portal!
                if (detailsMenu != null)
                {
                    detailsMenu.SetActive(false);
                }
            }
            else
            {
                SceneManager.LoadScene(currentTargetScene);
            }
        }
    }
}
