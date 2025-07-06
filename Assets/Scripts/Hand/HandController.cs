using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TriggerGripReader : MonoBehaviour
{
    public ActionBasedController controller; // Reference to the Action-Based Controller.
    public Hand hand; // Reference to the Hand script controlling the Animator.
    public XRNode controllerNode = XRNode.RightHand;
    void Update()
    {
        if (controller == null || hand == null)
        {
            Debug.LogError("Controller or Hand is not assigned!");
            return;
        }

        // Read the grip value and pass it to the Hand script.
        float gripValue = controller.selectAction.action.ReadValue<float>();
        hand.SetGrip(gripValue);

        // Read the trigger value and pass it to the Hand script.
        float triggerValue = controller.activateAction.action.ReadValue<float>();
        hand.SetTrigger(triggerValue);
        ButtonController(XRNode.RightHand);
        ButtonController(XRNode.LeftHand);

    }


    public void ButtonController(XRNode controllerNode)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);

        // (primaryButton)
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed) && isAPressed)
        {
            Debug.Log("Button A is pressed.");
         

        }

        // (secondaryButton)
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed) && isBPressed)
        {
            Debug.Log("Button B is pressed.");
        }
    }

}