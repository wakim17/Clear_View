using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// Unity 6 / XRIT 3.x compatible remote pointer.
/// Bypasses the Interaction Group lock by manually invoking UI clicks AND UI hovers.
/// </summary>
public class RemotePointerToggle : MonoBehaviour
{
    [Header("Component References")]
    public XRGrabInteractable grabComponent;
    public XRRayInteractor controllerRay;
    public XRInteractorLineVisual lineVisual;

    [Header("Laser Aiming")]
    public Transform remoteTipTransform;

    private Transform originalRayOrigin;

    private void OnEnable()
    {
        grabComponent.selectEntered.AddListener(TurnOnLaser);
        grabComponent.selectExited.AddListener(TurnOffLaser);

        // Listen for the trigger pull
        grabComponent.activated.AddListener(ClickUI);

        // Listen for the laser touching a UI element
        if (controllerRay != null)
        {
            controllerRay.uiHoverEntered.AddListener(ForceHoverEnter);
            controllerRay.uiHoverExited.AddListener(ForceHoverExit);
        }
    }

    private void OnDisable()
    {
        grabComponent.selectEntered.RemoveListener(TurnOnLaser);
        grabComponent.selectExited.RemoveListener(TurnOffLaser);
        grabComponent.activated.RemoveListener(ClickUI);

        if (controllerRay != null)
        {
            controllerRay.uiHoverEntered.RemoveListener(ForceHoverEnter);
            controllerRay.uiHoverExited.RemoveListener(ForceHoverExit);
        }
    }

    private void TurnOnLaser(SelectEnterEventArgs args)
    {
        if (controllerRay != null && lineVisual != null)
        {
            originalRayOrigin = controllerRay.rayOriginTransform;

            if (remoteTipTransform != null)
            {
                controllerRay.rayOriginTransform = remoteTipTransform;
            }

            controllerRay.enabled = true;
            lineVisual.enabled = true;
        }
    }

    private void TurnOffLaser(SelectExitEventArgs args)
    {
        if (controllerRay != null && lineVisual != null)
        {
            controllerRay.rayOriginTransform = originalRayOrigin;
            controllerRay.enabled = false;
            lineVisual.enabled = false;
        }
    }

    private void ClickUI(ActivateEventArgs args)
    {
        if (controllerRay != null && controllerRay.TryGetCurrentUIRaycastResult(out RaycastResult result))
        {
            Button hitButton = result.gameObject.GetComponentInParent<Button>();
            if (hitButton != null)
            {
                hitButton.onClick.Invoke();
            }
        }
    }

    private void ForceHoverEnter(UIHoverEventArgs args)
    {
        if (args.uiObject != null)
        {
            // Find the UI component and force it to highlight
            Selectable uiElement = args.uiObject.GetComponentInParent<Selectable>();
            if (uiElement != null)
            {
                uiElement.OnPointerEnter(new PointerEventData(EventSystem.current));
            }
        }
    }

    private void ForceHoverExit(UIHoverEventArgs args)
    {
        if (args.uiObject != null)
        {
            // Find the UI component and force it to return to normal
            Selectable uiElement = args.uiObject.GetComponentInParent<Selectable>();
            if (uiElement != null)
            {
                uiElement.OnPointerExit(new PointerEventData(EventSystem.current));
            }
        }
    }
}