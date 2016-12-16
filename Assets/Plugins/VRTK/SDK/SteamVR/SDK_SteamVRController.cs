﻿// SteamVR Controller|SDK_SteamVR|003
namespace VRTK
{
#if VRTK_SDK_STEAMVR
    using UnityEngine;
    using Valve.VR;

    /// <summary>
    /// The SteamVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    public class SDK_SteamVRController : SDK_BaseController
    {
        private SteamVR_TrackedObject cachedLeftTrackedObject;
        private SteamVR_TrackedObject cachedRightTrackedObject;

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath()
        {
            return "ControllerColliders/HTCVive";
        }

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(VRTK_ControllerElements element, VRTK_DeviceFinder.ControllerHand hand, bool fullPath = false)
        {
            var suffix = (fullPath ? "/attach" : "");
            switch (element)
            {
                case VRTK_ControllerElements.AttachPoint:
                    return "Model/tip/attach";
                case VRTK_ControllerElements.Trigger:
                    return "Model/trigger" + suffix;
                case VRTK_ControllerElements.GripLeft:
                    return "Model/lgrip" + suffix;
                case VRTK_ControllerElements.GripRight:
                    return "Model/rgrip" + suffix;
                case VRTK_ControllerElements.Touchpad:
                    return "Model/trackpad" + suffix;
                case VRTK_ControllerElements.ApplicationMenu:
                    return "Model/button" + suffix;
                case VRTK_ControllerElements.SystemMenu:
                    return "Model/sys_button" + suffix;
                case VRTK_ControllerElements.Body:
                    return "Model/body";
            }
            return null;
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            var trackedObject = GetTrackedObject(controller);
            return (trackedObject ? (uint)trackedObject.index : uint.MaxValue);
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns></returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject != null && (uint)cachedLeftTrackedObject.index == index)
                {
                    return (actual ? sdkManager.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightTrackedObject != null && (uint)cachedRightTrackedObject.index == index)
                {
                    return (actual ? sdkManager.actualRightController : sdkManager.scriptAliasRightController);
                }
            }
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(GameObject controller)
        {
            var trackedObject = GetTrackedObject(controller);
            if (trackedObject)
            {
                return trackedObject.origin ? trackedObject.origin : trackedObject.transform.parent;
            }

            return null;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            var renderModel = (controller.GetComponent<SteamVR_RenderModel>() ? controller.GetComponent<SteamVR_RenderModel>() : controller.GetComponentInChildren<SteamVR_RenderModel>());
            return (renderModel ? renderModel.gameObject : null);
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            var model = renderModel.GetComponent<SteamVR_RenderModel>();
            if (model)
            {
                model.controllerModeState.bScrollWheelVisible = state;
            }
        }

        /// <summary>
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="durationMicroSec">The amount of microseconds to run the haptic pulse for.</param>
        public override void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
            if (index < uint.MaxValue)
            {
                var device = SteamVR_Controller.Input((int)index);
                device.TriggerHapticPulse(durationMicroSec, EVRButtonId.k_EButton_Axis0);
            }
        }

        /// <summary>
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.velocity;
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.angularVelocity;
        }

        /// <summary>
        /// The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis();
        }

        /// <summary>
        /// The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
        }

        /// <summary>
        /// The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return 0f;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.hairTriggerDelta;
        }

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerDown();
        }

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerUp();
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsApplicationMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        [RuntimeInitializeOnLoadMethod]
        private void Initialise()
        {
            SteamVR_Utils.Event.Listen("TrackedDeviceRoleChanged", OnTrackedDeviceRoleChanged);
            SetTrackedControllerCaches(true);
        }

        private void OnTrackedDeviceRoleChanged(params object[] args)
        {
            SetTrackedControllerCaches(true);
        }

        private void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.actualLeftController)
                {
                    cachedLeftTrackedObject = sdkManager.actualLeftController.GetComponent<SteamVR_TrackedObject>();
                }
                if (cachedRightTrackedObject == null && sdkManager.actualRightController)
                {
                    cachedRightTrackedObject = sdkManager.actualRightController.GetComponent<SteamVR_TrackedObject>();
                }
            }
        }

        private SteamVR_TrackedObject GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            SteamVR_TrackedObject trackedObject = null;

            if (IsControllerLeftHand(controller, true) || IsControllerLeftHand(controller, false))
            {
                trackedObject = cachedLeftTrackedObject;
            }
            else if (IsControllerRightHand(controller, true) || IsControllerRightHand(controller, false))
            {
                trackedObject = cachedRightTrackedObject;
            }
            return trackedObject;
        }

        private GameObject GetActualController(GameObject controller)
        {
            GameObject returnController = null;
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (IsControllerLeftHand(controller, true) || IsControllerLeftHand(controller, false))
                {
                    returnController = sdkManager.actualLeftController;
                }
                else if (IsControllerRightHand(controller, true) || IsControllerRightHand(controller, false))
                {
                    returnController = sdkManager.actualRightController;
                }
            }
            return returnController;
        }

        private static bool IsButtonPressed(uint index, ButtonPressTypes type, ulong button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);

            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetPress(button);
                case ButtonPressTypes.PressDown:
                    return device.GetPressDown(button);
                case ButtonPressTypes.PressUp:
                    return device.GetPressUp(button);
                case ButtonPressTypes.Touch:
                    return device.GetTouch(button);
                case ButtonPressTypes.TouchDown:
                    return device.GetTouchDown(button);
                case ButtonPressTypes.TouchUp:
                    return device.GetTouchUp(button);
            }

            return false;
        }
    }
#else
    public class SDK_SteamVRController : SDK_FallbackController
    {
    }
#endif
}