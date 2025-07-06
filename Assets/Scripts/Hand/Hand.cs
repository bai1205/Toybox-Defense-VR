using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component.

    private float gripCurrent; // Current value of the Grip animation.
    private float triggerCurrent; // Current value of the Trigger animation.
    private float gripTarget; // Target value for the Grip animation.
    private float triggerTarget; // Target value for the Trigger animation.

    [Header("Animation Settings")]
    public float animationSpeed = 5.0f; // Speed of animation blending.

    private const string animatorGripParam = "grip"; // Animator parameter for Grip.
    private const string animatorTriggerParam = "trigger"; // Animator parameter for Trigger.

    void Start()
    {
        // Initialize the Animator component.
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Animate the Grip and Trigger values.
        AnimateHand();
    }

    /// <summary>
    /// Sets the target value for the grip animation.
    /// </summary>
    /// <param name="value">Target value (0 to 1).</param>
    public void SetGrip(float value)
    {
        gripTarget = Mathf.Clamp01(value); // Clamp the value between 0 and 1.
    }

    /// <summary>
    /// Sets the target value for the trigger animation.
    /// </summary>
    /// <param name="value">Target value (0 to 1).</param>
    public void SetTrigger(float value)
    {
        triggerTarget = Mathf.Clamp01(value); // Clamp the value between 0 and 1.
    }

    /// <summary>
    /// Smoothly interpolates and updates the Grip and Trigger animations.
    /// </summary>
    private void AnimateHand()
    {
        // Smoothly interpolate the Grip value.
        if (gripCurrent != gripTarget)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat(animatorGripParam, gripCurrent);
        }

        // Smoothly interpolate the Trigger value.
        if (triggerCurrent != triggerTarget)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat(animatorTriggerParam, triggerCurrent);
        }
    }
}
