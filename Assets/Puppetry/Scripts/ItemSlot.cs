using UnityEngine;
using UnityEngine.UI;
using Utils;
using VRTK;

public class ItemSlot : VRTK_InteractableObject {
	[SerializeField] private Transform m_SpawnPoint;
	[SerializeField] private Text m_Label;

	private GameObject m_Prefab;
	private GameObject m_Instance;

	public override void StartUsing(GameObject currentUsingObject) {
		base.StartUsing(currentUsingObject);
		Grab(currentUsingObject, m_Prefab);
	}

	public void Spawn(IItemCreator creator) {
		UnityUtils.Destroy(m_Instance);
		m_Instance = creator.Create(prefab => {
			m_Prefab = prefab;
			return Instantiate(prefab, m_SpawnPoint, false);
		});
		m_Instance.transform.ResetTransform();
		m_Label.text = creator.Name;
		DisableColliders(m_Instance);
	}

	private void DisableColliders(GameObject instance) { instance.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false); }

	private void Grab(GameObject controller, GameObject prefab) {
		var grabbableObject = prefab.GetComponent<VRTK_InteractableObject>();
		var controllerGrab = controller.GetComponent<VRTK_InteractGrab>();
		var controllerTouch = controller.GetComponent<VRTK_InteractTouch>();

		if (!grabbableObject) {
			Debug.LogError("Object cannot be grabbed.", prefab);
			return;
		}

		while (controllerGrab.controllerAttachPoint == null) return;

		if (controllerGrab.GetGrabbedObject()) return;

		grabbableObject = Instantiate(grabbableObject);

		if (!grabbableObject.isGrabbable || grabbableObject.IsGrabbed()) return;

		if (grabbableObject.grabAttachMechanicScript && grabbableObject.grabAttachMechanicScript.IsKinematic()) grabbableObject.isKinematic = true;

		grabbableObject.transform.position = transform.position;
		controllerTouch.ForceStopTouching();
		controllerTouch.ForceTouch(grabbableObject.gameObject);
		controllerGrab.AttemptGrab();
	}
}