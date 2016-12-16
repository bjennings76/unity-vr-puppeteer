﻿// Fallback Controller|SDK_Fallback|003
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    /// <remarks>
    /// This is the fallback class that will just return default values.
    /// </remarks>
    public class SDK_FallbackController : SDK_BaseController
    {
        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath()
        {
            return "";
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
            return "";
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            return uint.MaxValue;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns></returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(GameObject controller)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            return base.GetControllerLeftHand(actual);
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            return base.GetControllerRightHand(actual);
        }

        /// <summary>
        /// The IsControllerLeftHand method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual = false)
        {
            return base.IsControllerLeftHand(controller, actual);
        }

        /// <summary>
        /// The IsControllerRightHand method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual = false)
        {
            return base.IsControllerRightHand(controller, actual);
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            return null;
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
        }

        /// <summary>
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="durationMicroSec">The amount of microseconds to run the haptic pulse for.</param>
        public override void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
        }

        /// <summary>
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocityOnIndex(uint index)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        /// <summary>
        /// The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        /// <summary>
        /// The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return 0f;
        }

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsApplicationMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsApplicationMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsApplicationMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsApplicationMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsApplicationMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsApplicationMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return false;
        }
    }
}