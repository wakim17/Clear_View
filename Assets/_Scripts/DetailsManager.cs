using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // This line is required for TextMeshPro

public class DetailsManager : MonoBehaviour
{
    [Header("Menu Screens")]
    public GameObject gridMenu;
    public GameObject detailsMenu;

    [Header("Details Visuals")]
    public Image displayImage;
    public TextMeshProUGUI displayTitle; // Upgraded to TMP
    public TextMeshProUGUI displayDescription; // Upgraded to TMP

    [Header("Transition")]
    [Tooltip("Optional portal transition system. If assigned, loading the target scene will trigger the portal transition effect.")]
    public PortalTransition portalTransition;

    private string currentTargetScene = "";

    // Added the 'title' string to the incoming data
    public void OpenDetails(string sceneName, string title, string description, Sprite previewImage)
    {
        currentTargetScene = sceneName;
        displayTitle.text = title;
        displayDescription.text = description;
        displayImage.sprite = previewImage;

        gridMenu.SetActive(false);
        detailsMenu.SetActive(true);
    }

    public void GoBack()
    {
        detailsMenu.SetActive(false);
        gridMenu.SetActive(true);
    }

    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(currentTargetScene))
        {
            if (portalTransition != null)
            {
                // Trigger the gorgeous portal transition sequence
                portalTransition.SetTargetEnvironment(currentTargetScene);
                portalTransition.LoadEnvironment();

                // Close the details menu immediately so the user can see the portal open!
                if (detailsMenu != null)
                {
                    detailsMenu.SetActive(false);
                }
            }
            else
            {
                // Graceful fallback: load immediately if portal is not configured
                SceneManager.LoadScene(currentTargetScene);
            }
        }
    }
}