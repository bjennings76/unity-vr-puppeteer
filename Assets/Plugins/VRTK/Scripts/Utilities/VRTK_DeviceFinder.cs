// Device Finder|Utilities|90040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Device Finder offers a collection of static methods that can be called to find common game devices such as the headset or controllers, or used to determine key information about the connected devices.
    /// </summary>
    public class VRTK_DeviceFinder : MonoBehaviour
    {
        /// <summary>
        /// Possible devices.
        /// </summary>
        /// <param name="Headset">The headset.</param>
        /// <param name="Left_Controller">The left hand controller.</param>
        /// <param name="Right_Controller">The right hand controller.</param>
        public enum Devices
        {
            Headset,
            Left_Controller,
            Right_Controller,
        }

        /// <summary>
        /// Controller hand reference.
        /// </summary>
        /// <param name="None">No hand is assigned.</param>
        /// <param name="Left">The left hand is assigned.</param>
        /// <param name="Right">The right hand is assigned.</param>
        public enum ControllerHand
        {
            None,
            Left,
            Right
        }

        /// <summary>
        /// The GetSDKController returns the game object that is associated to the actual SDK controller.
        /// </summary>
        /// <param name="controller">The game object of a controller to check for.</param>
        /// <returns>The game object of the actual controller used by the sdk.</returns>
        public static GameObject GetSDKController(GameObject controller)
        {
            if (controller)
            {
                var isAlias = controller.GetComponent<VRTK_ControllerMapper>();
                var isActual = controller.GetComponent<VRTK_ControllerMarker>();

                if (isAlias)
                {
                    isActual = GetActualController(controller);
                }

                if (isActual)
                {
                    return isActual.gameObject;
                }

                return controller;
            }
            return null;
        }

        /// <summary>
        /// The TrackedIndexIsController method is used to determine if a given tracked object index belongs to a tracked controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to find.</param>
        /// <returns>Returns true if the given index is a tracked object of type controller.</returns>
        public static bool TrackedIndexIsController(uint index)
        {
            return VRTK_SDK_Bridge.TrackedIndexIsController(index);
        }

        /// <summary>
        /// The GetControllerIndex method is used to find the index of a given controller object.
        /// </summary>
        /// <param name="controller">The controller object to check the index on.</param>
        /// <returns>The index of the given controller.</returns>
        public static uint GetControllerIndex(GameObject controller, bool forceRefresh = false)
        {
            uint returnIndex = uint.MaxValue;
            if (!controller)
            {
                return returnIndex;
            }

            VRTK_ControllerMarker isActual;
            VRTK_ControllerMapper isAlias = controller.GetComponent<VRTK_ControllerMapper>();

            if (isAlias)
            {
                isActual = GetActualController(controller);
            }
            else
            {
                isActual = controller.GetComponent<VRTK_ControllerMarker>();
            }

            if (isActual)
            {
                returnIndex = isActual.GetControllerIndex();
            }

            if (returnIndex == uint.MaxValue)
            {
                returnIndex = VRTK_SDK_Bridge.GetIndexOfTrackedObject(GetSDKController(controller));
            }

            return returnIndex;
        }

        /// <summary>
        /// The TrackedObjectByIndex method is used to find the GameObject of a tracked object by its generated index.
        /// </summary>
        /// <param name="index">The index of the tracked object to find.</param>
        /// <returns>The tracked object that matches the given index.</returns>
        public static GameObject TrackedObjectByIndex(uint index)
        {
            return VRTK_SDK_Bridge.GetTrackedObjectByIndex(index);
        }

        /// <summary>
        /// The TrackedObjectOrigin method is used to find the tracked object's origin.
        /// </summary>
        /// <param name="obj">The GameObject to get the origin for.</param>
        /// <returns>The transform of the tracked object's origin or if an origin is not set then the transform parent.</returns>
        public static Transform TrackedObjectOrigin(GameObject obj)
        {
            return VRTK_SDK_Bridge.GetTrackedObjectOrigin(GetSDKController(obj));
        }

        /// <summary>
        /// The TrackedObjectOfGameObject method is used to find the tracked object associated with the given game object and it can also return the index of the tracked object.
        /// </summary>
        /// <param name="obj">The game object to check for the presence of a tracked object on.</param>
        /// <param name="index">The variable to store the tracked object's index if one is found. It returns 0 if no index is found.</param>
        /// <returns>The GameObject of the tracked object.</returns>
        public static GameObject TrackedObjectOfGameObject(GameObject obj, out uint index)
        {
            return VRTK_SDK_Bridge.GetTrackedObject(GetSDKController(obj), out index);
        }

        /// <summary>
        /// The DeviceTransform method returns the transform for a given Devices enum.
        /// </summary>
        /// <param name="device">The Devices enum to get the transform for.</param>
        /// <returns>The transform for the given Devices enum.</returns>
        public static Transform DeviceTransform(Devices device)
        {
            switch (device)
            {
                case Devices.Headset:
                    return HeadsetTransform();
                case Devices.Left_Controller:
                    return GetControllerLeftHand().transform;
                case Devices.Right_Controller:
                    return GetControllerRightHand().transform;
            }
            return null;
        }

        /// <summary>
        /// The GetControllerHandType method is used for getting the enum representation of ControllerHand from a given string.
        /// </summary>
        /// <param name="hand">The string representation of the hand to retrieve the type of. `left` or `right`.</param>
        /// <returns>A ControllerHand representing either the Left or Right hand.</returns>
        public static ControllerHand GetControllerHandType(string hand)
        {
            switch (hand.ToLower())
            {
                case "left":
                    return ControllerHand.Left;
                case "right":
                    return ControllerHand.Right;
                default:
                    return ControllerHand.None;
            }
        }

        /// <summary>
        /// The GetControllerHand method is used for getting the enum representation of ControllerHand for the given controller game object.
        /// </summary>
        /// <param name="controller">The controller game object to check the hand of.</param>
        /// <returns>A ControllerHand representing either the Left or Right hand.</returns>
        public static ControllerHand GetControllerHand(GameObject controller)
        {
            controller = GetSDKController(controller);

            if (VRTK_SDK_Bridge.IsControllerLeftHand(controller))
            {
                return ControllerHand.Left;
            }
            else if (VRTK_SDK_Bridge.IsControllerRightHand(controller))
            {
                return ControllerHand.Right;
            }
            else
            {
                return ControllerHand.None;
            }
        }

        /// <summary>
        /// The GetActualController method will attempt to get the actual SDK controller object.
        /// </summary>
        /// <param name="givenController">The game object that contains a link to a marked controller.</param>
        /// <returns>The object that is the marked controller.</returns>
        public static VRTK_ControllerMarker GetActualController(GameObject controllerHand)
        {
            var markedControllerCheck = (controllerHand ? controllerHand.GetComponent<VRTK_ControllerMarker>() : null);
            if (markedControllerCheck)
            {
                return markedControllerCheck;
            }

            var controllerMapper = (controllerHand ? controllerHand.GetComponent<VRTK_ControllerMapper>() : null);
            if (controllerMapper && controllerMapper.markedController)
            {
                return controllerMapper.markedController;
            }
            return null;
        }

        /// <summary>
        /// The GetAliasController method will attempt to get the object that is mapped to the actual SDK controller object.
        /// </summary>
        /// <param name="controllerHand">The game object of a controller that contains a controller marker.</param>
        /// <returns>The object that is the mapped controller to the given controller.</returns>
        public static VRTK_ControllerMapper GetAliasController(GameObject controllerHand)
        {
            var mappedControllerCheck = (controllerHand ? controllerHand.GetComponent<VRTK_ControllerMapper>() : null);
            if (mappedControllerCheck)
            {
                return mappedControllerCheck;
            }

            var controllerMarker = (controllerHand ? controllerHand.GetComponent<VRTK_ControllerMarker>() : null);
            if (controllerMarker && controllerMarker.mappedController)
            {
                return controllerMarker.mappedController;
            }
            return null;
        }

        /// <summary>
        /// The GetControllerLeftHand method retrieves the game object for the left hand controller.
        /// </summary>
        /// <param name="getActual">An optional parameter that if true will return the game object that the SDK controller is attached to.</param>
        /// <returns>The left hand controller.</returns>
        public static GameObject GetControllerLeftHand(bool getActual = false)
        {
            if (getActual)
            {
                return VRTK_SDK_Bridge.GetControllerLeftHand();
            }

            var mappedController = GetAliasController(VRTK_SDK_Bridge.GetControllerLeftHand());
            return (mappedController ? mappedController.gameObject : null);
        }

        /// <summary>
        /// The GetControllerRightHand method retrieves the game object for the right hand controller.
        /// </summary>
        /// <param name="getActual">An optional parameter that if true will return the game object that the SDK controller is attached to.</param>
        /// <returns>The right hand controller.</returns>
        public static GameObject GetControllerRightHand(bool getActual = false)
        {
            if (getActual)
            {
                return VRTK_SDK_Bridge.GetControllerRightHand();
            }

            var mappedController = GetAliasController(VRTK_SDK_Bridge.GetControllerRightHand());
            return (mappedController ? mappedController.gameObject : null);
        }

        /// <summary>
        /// The IsControllerOfHand method is used to check if a given controller game object is of the hand type provided.
        /// </summary>
        /// <param name="checkController">The actual controller object that is being checked.</param>
        /// <param name="hand">The representation of a hand to check if the given controller matches.</param>
        /// <returns>Is true if the given controller matches the given hand.</returns>
        public static bool IsControllerOfHand(GameObject checkController, ControllerHand hand)
        {
            checkController = GetSDKController(checkController);

            if (hand == ControllerHand.Left && VRTK_SDK_Bridge.IsControllerLeftHand(checkController))
            {
                return true;
            }

            if (hand == ControllerHand.Right && VRTK_SDK_Bridge.IsControllerRightHand(checkController))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The IsControllerLeftHand method is used to check if a given controller game object is the left handed controller.
        /// </summary>
        /// <param name="checkController">The controller object that is being checked.</param>
        /// <returns>Is true if the given controller is the left controller.</returns>
        public static bool IsControllerLeftHand(GameObject checkController)
        {
            return IsControllerOfHand(checkController, ControllerHand.Left);
        }

        /// <summary>
        /// The IsControllerRightHand method is used to check if a given controller game object is the right handed controller.
        /// </summary>
        /// <param name="checkController">The controller object that is being checked.</param>
        /// <returns>Is true if the given controller is the right controller.</returns>
        public static bool IsControllerRightHand(GameObject checkController)
        {
            return IsControllerOfHand(checkController, ControllerHand.Right);
        }

        /// <summary>
        /// The HeadsetTransform method is used to retrieve the transform for the VR Headset in the scene. It can be useful to determine the position of the user's head in the game world.
        /// </summary>
        /// <returns>The transform of the VR Headset component.</returns>
        public static Transform HeadsetTransform()
        {
            return VRTK_SDK_Bridge.GetHeadset();
        }

        /// <summary>
        /// The HeadsetCamera method is used to retrieve the transform for the VR Camera in the scene.
        /// </summary>
        /// <returns>The transform of the VR Camera component.</returns>
        public static Transform HeadsetCamera()
        {
            return VRTK_SDK_Bridge.GetHeadsetCamera();
        }

        /// <summary>
        /// The PlayAreaTransform method is used to retrieve the transform for the play area in the scene.
        /// </summary>
        /// <returns>The transform of the VR Play Area component.</returns>
        public static Transform PlayAreaTransform()
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }
    }
}