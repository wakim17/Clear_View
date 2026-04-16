using UnityEngine;

public class TVPowerSystem : MonoBehaviour
{
    [Header("Visuals")]
    public MeshRenderer screenRenderer;
    public Material whiteScreenMaterial;
    public GameObject hologramCanvas;

    [Header("Audio")]
    public AudioSource powerAudio;

    private bool isPoweredOn = false;

    public void PowerOn()
    {
        // This stops the TV from turning on twice if the remote is dropped and grabbed again.
        if (isPoweredOn == true) return;

        // 1. Change the screen material to white
        if (screenRenderer != null && whiteScreenMaterial != null)
        {
            screenRenderer.material = whiteScreenMaterial;
        }

        // 2. Open the holographic projection
        if (hologramCanvas != null)
        {
            hologramCanvas.SetActive(true);
        }

        // 3. Play the start sound
        if (powerAudio != null)
        {
            powerAudio.Play();
        }

        isPoweredOn = true;
    }
}