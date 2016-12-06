// Controller Marker|Utilities|90020
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the marked controller.</param>
    /// <param name="previousIndex">The previous index of the marked controller if it has been updated.</param>
    public struct ControllerMarkerEventArgs
    {
        public uint controllerIndex;
        public uint previousIndex;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerMarkerEventArgs"/></param>
    public delegate void ControllerMarkerEventHandler(object sender, ControllerMarkerEventArgs e);

    /// <summary>
    /// The Controller Marker is attached to a GameObject containing the VR Controller scripts to identify the controller and perform specific actions based around that specific controller.
    /// </summary>
    public class VRTK_ControllerMarker : MonoBehaviour
    {
        /// <summary>
        /// The script on another GameObject that is mapped to this marked controller.
        /// </summary>
        [HideInInspector]
        public VRTK_ControllerMapper mappedController;

        private uint controllerIndex = uint.MaxValue;
        private Coroutine attemptCacheControllerCoroutine = null;
        private VRTK_DeviceFinder.ControllerHand controllerHand;

        /// <summary>
        /// Emitted when the marked controller is enabled.
        /// </summary>
        public event ControllerMarkerEventHandler ControllerEnabled;
        /// <summary>
        /// Emitted when the marked controller is disabled.
        /// </summary>
        public event ControllerMarkerEventHandler ControllerDisabled;
        /// <summary>
        /// Emitted when the marked controller index is changed.
        /// </summary>
        public event ControllerMarkerEventHandler ControllerIndexChanged;

        public virtual void OnControllerEnabled(ControllerMarkerEventArgs e)
        {
            if (controllerIndex < uint.MaxValue && ControllerEnabled != null)
            {
                ControllerEnabled(this, e);
            }
        }

        public virtual void OnControllerDisabled(ControllerMarkerEventArgs e)
        {
            if (controllerIndex < uint.MaxValue && ControllerDisabled != null)
            {
                ControllerDisabled(this, e);
            }
        }

        public virtual void OnControllerIndexChanged(ControllerMarkerEventArgs e)
        {
            if (controllerIndex < uint.MaxValue && ControllerIndexChanged != null)
            {
                ControllerIndexChanged(this, e);
            }
        }

        /// <summary>
        /// The GetControllerIndex method returns the current index of the marked controller.
        /// </summary>
        /// <returns>The current index of the controller.</returns>
        public uint GetControllerIndex()
        {
            return controllerIndex;
        }

        /// <summary>
        /// The GetControllerHand method returns a ControllerHand enum to represent which hand the controller is being used by.
        /// </summary>
        /// <returns>The ControllerHand enum of either None, Left or Right.</returns>
        public VRTK_DeviceFinder.ControllerHand GetControllerHand()
        {
            return controllerHand;
        }

        /// <summary>
        /// The RefreshCache method attempts to update the controller index cache if the index has changed.
        /// </summary>
        public void RefreshCache()
        {
            var currentIndex = controllerIndex;
            CacheControllerIndex();
            if (currentIndex < uint.MaxValue && currentIndex != controllerIndex)
            {
                OnControllerIndexChanged(SetEventPayload(currentIndex));
            }
        }

        /// <summary>
        /// The ClearCache method removes the current marked controller from the cache.
        /// </summary>
        public void ClearCache()
        {
            RemoveControllerIndexFromCache();
        }

        private ControllerMarkerEventArgs SetEventPayload(uint previousIndex = uint.MaxValue)
        {
            ControllerMarkerEventArgs e;
            e.controllerIndex = controllerIndex;
            e.previousIndex = previousIndex;
            return e;
        }

        private IEnumerator AttemptCacheController()
        {
            yield return new WaitForEndOfFrame();

            while (!gameObject.activeInHierarchy)
            {
                yield return null;
            }

            CacheControllerIndex();
            OnControllerEnabled(SetEventPayload());
        }

        private void CacheControllerIndex()
        {
            uint tmpControllerIndex = uint.MaxValue;
            var trackedObject = VRTK_DeviceFinder.TrackedObjectOfGameObject(gameObject, out tmpControllerIndex);
            if (tmpControllerIndex < uint.MaxValue && tmpControllerIndex != controllerIndex)
            {
                RemoveControllerIndexFromCache();
                if (!VRTK_ObjectCache.trackedControllers.ContainsKey(tmpControllerIndex))
                {
                    VRTK_ObjectCache.trackedControllers.Add(tmpControllerIndex, trackedObject);
                }
                controllerIndex = tmpControllerIndex;
                controllerHand = VRTK_DeviceFinder.GetControllerHand(gameObject);
            }
        }

        private void RemoveControllerIndexFromCache()
        {
            if (VRTK_ObjectCache.trackedControllers.ContainsKey(controllerIndex))
            {
                VRTK_ObjectCache.trackedControllers.Remove(controllerIndex);
            }
        }

        private void OnEnable()
        {
            if (attemptCacheControllerCoroutine != null)
            {
                StopCoroutine(attemptCacheControllerCoroutine);
            }
            attemptCacheControllerCoroutine = StartCoroutine(AttemptCacheController());
        }

        private void OnDisable()
        {
            Invoke("Disable", 0f);
        }

        private void Disable()
        {
            if (attemptCacheControllerCoroutine != null)
            {
                StopCoroutine(attemptCacheControllerCoroutine);
            }

            OnControllerDisabled(SetEventPayload());
            RemoveControllerIndexFromCache();
        }
    }
}