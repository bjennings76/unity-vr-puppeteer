using UnityEngine;
using UnityEngine.UI;
using Utils;
using VRTK;

public class PropSlot : VRTK_InteractableObject {
	[SerializeField] private Transform m_SpawnPoint;
	[SerializeField] private Text m_Label;

	private GameObject m_PreviewInstance;
	private IPropCreator m_Creator;
	private PropDispenser m_Dispenser;

	private PropDispenser Dispenser {  get { return m_Dispenser ? m_Dispenser : (m_Dispenser = GetComponentInParent<PropDispenser>()); } }

	public override void StartUsing(GameObject currentUsingObject) {
		base.StartUsing(currentUsingObject);
		StopUsing(currentUsingObject);

		m_Creator.Create(p => Instantiate(p, Dispenser.transform.position, Dispenser.transform.rotation));

		//var instance = m_Creator.Create(p => Instantiate(p, currentUsingObject.transform.position, transform.rotation));
		//var controllerGrab = currentUsingObject.GetComponent<VRTK_InteractGrab>();
		//var defaultGrabObject = instance.GetComponent<DefaultGrabObject>();
		//var grabbableObject = defaultGrabObject ? defaultGrabObject.GrabbableObject : instance.GetComponentInChildren<VRTK_InteractableObject>();

		//if (!grabbableObject || !grabbableObject.isGrabbable || grabbableObject.IsGrabbed()) {
		//	Debug.LogError("Object cannot be grabbed.", this);
		//	UnityUtils.Destroy(instance);
		//	return;
		//}

		//var controllerTouch = currentUsingObject.GetComponent<VRTK_InteractTouch>();
		//if (controllerTouch) controllerTouch.ForceTouch(grabbableObject ? grabbableObject.gameObject : null);
		//if (controllerGrab) controllerGrab.AttemptGrab();
	}

	public void Spawn(IPropCreator creator) {
		m_Creator = creator;
		UnityUtils.Destroy(m_PreviewInstance);
		m_PreviewInstance = m_Creator.Create(p => {
			var preview = Instantiate(p, m_SpawnPoint, false);
			preview.GetComponentsInChildren<HideInPropPreview>().ForEach(c => c.Hide());
			return preview;
		});
		m_PreviewInstance.transform.ResetTransform();
		m_Label.text = m_Creator.Name;
		DisableColliders(m_PreviewInstance);
	}

	private static void DisableColliders(GameObject instance) { instance.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false); }
}