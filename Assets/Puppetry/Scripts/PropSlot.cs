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

	private static Transform m_CreationParent;

	private static Transform CreationParent { get { return m_CreationParent ? m_CreationParent : (m_CreationParent = GetPropParent()); } }

	private static Transform GetPropParent() {
		var parent = new GameObject("Props").transform;
		parent.localScale = new Vector3(0.15f, 0.15f, 0.15f);
		return parent;
	}

	private PropDispenser Dispenser { get { return m_Dispenser ? m_Dispenser : (m_Dispenser = GetComponentInParent<PropDispenser>()); } }

	public override void OnInteractableObjectUsed(InteractableObjectEventArgs e) {
		base.OnInteractableObjectUsed(e);
		m_Creator.Create(p => Instantiate(p, Dispenser.transform.position, Dispenser.transform.rotation, CreationParent));

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
			var preview = Instantiate(p, Vector3.zero, Quaternion.identity, m_SpawnPoint);
			preview.GetComponentsInChildren<HideInPropPreview>().ForEach(c => c.Hide());
			return preview;
		});

		m_PreviewInstance.transform.ResetTransform();
		var previewBounds = UnityUtils.GetBounds(m_PreviewInstance.transform);
		var col = m_SpawnPoint.GetComponent<BoxCollider>();

		var xScale = col.size.x * col.transform.lossyScale.x / previewBounds.size.x;
		var yScale = col.size.y * col.transform.lossyScale.y / previewBounds.size.y;
		var zScale = col.size.z * col.transform.lossyScale.z / previewBounds.size.z;

		var scale = Mathf.Min(xScale, yScale, zScale);

		var center = m_PreviewInstance.transform.InverseTransformPoint(previewBounds.center);
		m_PreviewInstance.transform.localScale = new Vector3(scale, scale, scale);
		m_PreviewInstance.transform.localPosition = -center*scale;

		m_Label.text = m_Creator.Name;
		DisableColliders(m_PreviewInstance);
	}

	private static void DisableColliders(GameObject instance) { instance.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false); }
}