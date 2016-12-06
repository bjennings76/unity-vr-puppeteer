// Controller Mapper|Utilities|90030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Controller Mapper maps the VR Controller that is marked with a Controller Marker to enable access to the controller's data.
    /// </summary>
    /// <remarks>
    /// This script is required to be attached to the same GameObject as any other script that requires data from a VR Controller (e.g. VRTK_ControllerEvents).
    /// </remarks>
    public class VRTK_ControllerMapper : MonoBehaviour
    {
        [Tooltip("A Controller Marker that is attached to the specific VR Controller to utilise.")]
        public VRTK_ControllerMarker markedController;

        private void Awake()
        {
            Setup();
            if (markedController)
            {
                markedController.ControllerEnabled += MarkedController_ControllerEnabled;
                markedController.ControllerDisabled += MarkedController_ControllerDisabled;
            }
        }

        private void Destroy()
        {
            if (markedController)
            {
                markedController.ControllerEnabled -= MarkedController_ControllerEnabled;
                markedController.ControllerDisabled -= MarkedController_ControllerDisabled;
            }
        }

        private void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            if (!markedController)
            {
                Debug.LogError("The VRTK_ControllerMapper script requires a valid VRTK_ControllerMarker to be attached (which is in turn attached to a valid VR Controller GameObject)");
            }

            if (markedController)
            {
                markedController.mappedController = this;
            }
        }

        private void MarkedController_ControllerEnabled(object sender, ControllerMarkerEventArgs e)
        {
            gameObject.SetActive(true);
        }

        private void MarkedController_ControllerDisabled(object sender, ControllerMarkerEventArgs e)
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            markedController.mappedController = null;
        }

        private void FixedUpdate()
        {
            if (markedController)
            {
                transform.position = markedController.transform.position;
                transform.rotation = markedController.transform.rotation;
                transform.localScale = markedController.transform.lossyScale;
            }
        }
    }
}