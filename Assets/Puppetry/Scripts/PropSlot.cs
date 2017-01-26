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

	private PropDispenser Dispenser { get { return m_Dispenser ? m_Dispenser : (m_Dispenser = GetComponentInParent<PropDispenser>()); } }

	public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e) {
		var instance = m_Creator.Create(p => Instantiate(p, Dispenser.transform.position, Dispenser.transform.rotation, GetPropParent()));
		var defaultGrabObject = instance.GetComponent<DefaultGrabObject>();
		var grabbableObject = defaultGrabObject ? defaultGrabObject.GrabbableObject : instance.GetComponentInChildren<VRTK_InteractableObject>();
		if (grabbableObject) grabbableObject.OnInteractableObjectGrabbed(e);
	}

	public void CreatePreview(IPropCreator creator) {
		m_Creator = creator;
		UnityUtils.Destroy(m_PreviewInstance);

		m_PreviewInstance = m_Creator.Create(p => {
			var preview = Instantiate(p, Vector3.zero, Quaternion.identity, m_SpawnPoint);
			preview.GetComponentsInChildren<HideInPropPreview>().ForEach(c => c.Hide());
			return preview;
		});

		m_PreviewInstance.transform.ResetTransform();
		var previewBounds = UnityUtils.GetBounds(m_PreviewInstance.transform); //creator.GetBounds();
		var col = m_SpawnPoint.GetComponent<BoxCollider>();

		var xScale = col.size.x * col.transform.lossyScale.x / previewBounds.size.x;
		var yScale = col.size.y * col.transform.lossyScale.y / previewBounds.size.y;
		var zScale = col.size.z * col.transform.lossyScale.z / previewBounds.size.z;

		var scale = Mathf.Min(xScale, yScale, zScale);

		var center = m_PreviewInstance.transform.InverseTransformPoint(previewBounds.center);
		m_PreviewInstance.transform.localScale = new Vector3(scale, scale, scale);
		m_PreviewInstance.transform.localPosition = -center * scale;

		m_Label.text = GetName(m_Creator, Dispenser.PropType.TrimRegex);
		DisableColliders(m_PreviewInstance);
	}

	private static Transform m_PropRoot;

	private Transform GetPropParent() {
		m_PropRoot = m_PropRoot ? m_PropRoot : new GameObject("Props").transform;
		var parent = new GameObject(m_Creator.Name + " Scaler").transform;
		parent.localScale = Vector3.one * m_Dispenser.PropType.Scale;
		parent.SetParent(m_PropRoot);
		return parent;
	}

	private static string GetName(IPropCreator creator, string trimRegex) { return creator.Name.ReplaceRegex(trimRegex, "").ToSpacedName(); }

	private static void DisableColliders(GameObject instance) { instance.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false); }
}