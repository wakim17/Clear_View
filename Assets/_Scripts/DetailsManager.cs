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
            SceneManager.LoadScene(currentTargetScene);
        }
    }
}