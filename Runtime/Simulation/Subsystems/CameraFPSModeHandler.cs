using System;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Input handler for movement in the device view and game view when using <c>CameraPoseProvider</c>.
    /// </summary>
    class CameraFPSModeHandler
    {
        const float k_PitchClamp = 85f;

        enum Control
        {
            Move,
            Look,
            Sprint,
            Unlock
        }

        readonly ButtonActionReferenceWrapper k_UnlockInputActionWrapper;

        readonly ValueActionReferenceWrapper<Vector3> k_MoveInputActionWrapper =
            new(XRSimulationPreferences.Instance.moveInputActionReference, Control.Move);
        readonly ValueActionReferenceWrapper<Vector2> k_LookInputActionWrapper =
            new(XRSimulationPreferences.Instance.lookInputActionReference, Control.Look);
        readonly ButtonActionReferenceWrapper k_SprintInputActionWrapper =
            new(XRSimulationPreferences.Instance.sprintInputActionReference, Control.Sprint);


        /// <summary>
        /// Use <c>MovementBounds</c> bounds to restrict movement.
        /// </summary>
        internal bool useMovementBounds { get; set; }

        /// <summary>
        /// The bounds of the movement area.
        /// </summary>
        internal Bounds movementBounds { get; set; }

        internal CameraFPSModeHandler()
        {
            if (XRSimulationPreferences.Instance.unlockInputActionReference != null)
            {
                k_UnlockInputActionWrapper = new ButtonActionReferenceWrapper(
                    XRSimulationPreferences.Instance.unlockInputActionReference,
                    Control.Unlock);
            }
            else
            {
                Debug.Log($"InputActionReference({Control.Unlock}) not provided. Navigation Actions will be active by default.");
            }
        }

        void OnUnlockInputEvent(object sender, bool controlsActive)
        {
            ToggleNavigationActions(controlsActive);
        }

        internal void OnEnable()
        {
            if (k_UnlockInputActionWrapper != null)
            {
                k_UnlockInputActionWrapper.ToggleInputAction(true);
                k_UnlockInputActionWrapper.actionStarted += OnUnlockInputEvent;
                k_UnlockInputActionWrapper.actionCanceled += OnUnlockInputEvent;
            }
            else
                ToggleNavigationActions(true);
        }

        internal void OnDisable()
        {
            if (k_UnlockInputActionWrapper != null)
            {
                k_UnlockInputActionWrapper.ToggleInputAction(false);
                k_UnlockInputActionWrapper.actionStarted -= OnUnlockInputEvent;
                k_UnlockInputActionWrapper.actionCanceled -= OnUnlockInputEvent;
            }
            else
                ToggleNavigationActions(false);
        }

        void ToggleNavigationActions(bool controlsActive)
        {
            k_MoveInputActionWrapper.ToggleInputAction(controlsActive);
            k_LookInputActionWrapper.ToggleInputAction(controlsActive);
            k_SprintInputActionWrapper.ToggleInputAction(controlsActive);
        }

        Vector3 GetMovementDirection()
        {
            var moveVector = k_MoveInputActionWrapper.value;
            var sprintSpeed = k_SprintInputActionWrapper.value;

            if (moveVector.sqrMagnitude < float.Epsilon)
                return Vector3.zero;

            var speedModifier = XRSimulationPreferences.Instance.moveSpeed;

            if (sprintSpeed)
                speedModifier *= XRSimulationPreferences.Instance.moveSpeedModifier;

            return Time.deltaTime * speedModifier * moveVector.normalized;
        }

        /// <summary>
        /// Calculate the new pose for the camera based on the fps mode movement.
        /// </summary>
        /// <param name="pose">Current Pose of the transform we are going to move.</param>
        /// <param name="invertY">Invert the Y axis of the mouse position.</param>
        /// <returns>The new camera pose.</returns>
        internal Pose CalculateMovement(Pose pose, bool invertY = false)
        {
            var rotation = CalculateMouseRotation(pose.rotation, invertY);
            var position = pose.position + rotation.ConstrainYawNormalized() * GetMovementDirection();

            if (useMovementBounds && movementBounds != default && !movementBounds.Contains(position))
                position = movementBounds.ClosestPoint(position);

            return new Pose(position, rotation);
        }

        Quaternion CalculateMouseRotation(Quaternion rotation, bool invertY = false)
        {
            var eulerAngles = rotation.eulerAngles;
            var yaw = eulerAngles.y;
            var pitch = eulerAngles.x;
            if (pitch > 180)
                pitch = pitch - 360;

            const float pixelsToDegrees = 0.01f * Mathf.Rad2Deg;
            var lookSpeedModifier = pixelsToDegrees * XRSimulationPreferences.Instance.lookSpeed;
            var lookVector = k_LookInputActionWrapper.value;

            var deltaY = lookVector.y;
            if (invertY)
                deltaY *= -1;

            yaw += lookVector.x * lookSpeedModifier;
            pitch += deltaY * lookSpeedModifier;

            pitch = Mathf.Clamp(pitch, -k_PitchClamp, k_PitchClamp);

            return Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);
        }

        /// <summary>
        /// Utility class for managing <c>InputActionReference</c> instances and tracking their input values
        /// </summary>
        abstract class InputActionReferenceWrapper<T> where T : struct
        {
            bool m_Active;

            readonly Control k_Control;
            readonly string k_ActionMissingMessage;
            readonly string k_MismatchValueTypeMessage;

            internal readonly InputActionReference k_InputActionReference;
            protected internal T value { get; protected set; }

            internal event EventHandler<T> actionCanceled;

            /// <summary>
            /// Callback that will be used to obtain new values from an active input event.
            /// </summary>
            protected abstract EventHandler<InputAction.CallbackContext> activeEventCallback { get; }

            protected InputActionReferenceWrapper(InputActionReference inputActionReference, Control control)
            {
                k_InputActionReference = inputActionReference;
                k_Control = control;

                k_ActionMissingMessage = $"Cannot find InputAction({k_Control})";
                k_MismatchValueTypeMessage = $"InputAction({k_Control}) does not match expected value type: {typeof(T).Name}";

                if (k_InputActionReference == null)
                    Debug.LogWarning($"Missing InputActionReference({k_Control}). Navigation controls will not be fully functional.");
            }

            /// <summary>
            /// Enable/disable event listener for tracking active input values
            /// <param name="action">Target input action being subscribed/unsubscribed to.</param>
            /// <param name="shouldListen">Whether the listener should be attached/detached.</param>
            /// </summary>
            protected abstract void ToggleActiveEventListener(InputAction action, bool shouldListen);

            protected void OnActiveEvent(InputAction.CallbackContext context)
            {
                try
                {
                    activeEventCallback(context.action, context);
                }
                catch (InvalidOperationException)
                {
                    Debug.LogError(k_MismatchValueTypeMessage);
                }
            }

            void OnActionCanceled(InputAction.CallbackContext context)
            {
                value = default;
                actionCanceled?.Invoke(this, value);
            }

            /// <summary>
            /// Enable/disable <c>InputAction</c> activity and event listeners
            /// </summary>
            internal void ToggleInputAction(bool active)
            {
                if (active == m_Active || k_InputActionReference == null)
                    return;

                var action = k_InputActionReference.action;

                if (action == null)
                {
                    Debug.LogWarning(k_ActionMissingMessage);
                    return;
                }

                if (active)
                {
                    action.Enable();
                    action.canceled += OnActionCanceled;
                }
                else
                {
                    action.Disable();
                    action.canceled -= OnActionCanceled;
                }

                ToggleActiveEventListener(action, active);

                m_Active = active;
            }
        }

        class ValueActionReferenceWrapper<T> : InputActionReferenceWrapper<T> where T : struct
        {
            protected override EventHandler<InputAction.CallbackContext> activeEventCallback => OnActionPerformed;

            internal ValueActionReferenceWrapper(InputActionReference inputActionReference, Control control) :
                base(inputActionReference, control) {}

            protected override void ToggleActiveEventListener(InputAction action, bool shouldListen)
            {
                if (shouldListen)
                    action.performed += OnActiveEvent;
                else
                    action.performed -= OnActiveEvent;
            }

            void OnActionPerformed(object sender, InputAction.CallbackContext context)
            {
                value = context.ReadValue<T>();
            }
        }

        class ButtonActionReferenceWrapper : InputActionReferenceWrapper<bool>
        {
            internal event EventHandler<bool> actionStarted;
            protected override EventHandler<InputAction.CallbackContext> activeEventCallback => OnActionStarted;

            internal ButtonActionReferenceWrapper(InputActionReference inputActionReference, Control control) :
                base(inputActionReference, control) {}

            protected override void ToggleActiveEventListener(InputAction action, bool shouldListen)
            {
                if (shouldListen)
                    action.started += OnActiveEvent;
                else
                    action.started -= OnActiveEvent;
            }

            void OnActionStarted(object sender, InputAction.CallbackContext context)
            {
                value = context.ReadValueAsButton();
                actionStarted?.Invoke(this, value);
            }
        }
    }
}
