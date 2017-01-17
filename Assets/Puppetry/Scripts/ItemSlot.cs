using UnityEngine;
using UnityEngine.UI;
using Utils;
using VRTK;

public class ItemSlot : VRTK_InteractableObject {
	[SerializeField] private Transform m_SpawnPoint;
	[SerializeField] private Text m_Label;

	private GameObject m_Prefab;
	private GameObject m_Instance;
	private IItemCreator m_Creator;

	public override void StartUsing(GameObject currentUsingObject) {
		base.StartUsing(currentUsingObject);
		Grab(currentUsingObject, m_Prefab);
	}

	public void Spawn(IItemCreator creator) {
		m_Creator = creator;
		UnityUtils.Destroy(m_Instance);
		m_Instance = m_Creator.Create(prefab => {
			m_Prefab = prefab;
			return Instantiate(prefab, m_SpawnPoint, false);
		});
		m_Instance.transform.ResetTransform();
		m_Label.text = m_Creator.Name;
		DisableColliders(m_Instance);
	}

	private static void DisableColliders(GameObject instance) { instance.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false); }

	private void Grab(GameObject controller, GameObject prefab) {
		var controllerGrab = controller.GetComponent<VRTK_InteractGrab>();
		var controllerTouch = controller.GetComponent<VRTK_InteractTouch>();

		if (controllerGrab.controllerAttachPoint == null || controllerGrab.GetGrabbedObject()) return;

		var instance = m_Creator.Create(p => Instantiate(p, controller.transform.position, transform.rotation));

		var defaultGrabObject = instance.GetComponent<DefaultGrabObject>();

		var grabbableObject = defaultGrabObject ? defaultGrabObject.GrabbableObject : instance.GetComponentInChildren<VRTK_InteractableObject>();

		if (!grabbableObject) {
			Debug.LogError("Object cannot be grabbed.", prefab);
			UnityUtils.Destroy(instance);
			return;
		}

		if (!grabbableObject.isGrabbable || grabbableObject.IsGrabbed()) return;

		if (grabbableObject.grabAttachMechanicScript && grabbableObject.grabAttachMechanicScript.IsKinematic()) grabbableObject.isKinematic = true;

		controllerTouch.ForceStopTouching();
		controllerTouch.ForceTouch(grabbableObject.gameObject);
		controllerGrab.AttemptGrab();
	}
}