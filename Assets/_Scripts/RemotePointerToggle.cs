using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class RemotePointerToggle : MonoBehaviour
{
    [Header("Component References")]
    public XRGrabInteractable grabComponent;
    public XRRayInteractor controllerRay;
    public XRInteractorLineVisual lineVisual;

    [Header("Laser Aiming")]
    public Transform remoteTipTransform;

    private Transform originalRayOrigin;

    /// Subscribes to XR grab and hover events.
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

    /// Unsubscribes from XR grab and hover events.
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

    /// Turns on the laser pointer when the controller is selected/grabbed.
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

    /// Turns off the laser pointer when the controller is released.
    private void TurnOffLaser(SelectExitEventArgs args)
    {
        if (controllerRay != null && lineVisual != null)
        {
            controllerRay.rayOriginTransform = originalRayOrigin;
            controllerRay.enabled = false;
            lineVisual.enabled = false;
        }
    }

    /// Manually triggers a UI click on the hovered button when the trigger is pulled.
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

    /// Forces a UI element to highlight when hovered by the laser.
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

    /// Forces a UI element to return to normal when the laser exits.
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
