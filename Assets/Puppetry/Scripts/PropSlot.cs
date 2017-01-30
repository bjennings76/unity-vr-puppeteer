using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

public class PropSlot : MonoBehaviour {
	[SerializeField] private Transform m_SpawnPoint;
	[SerializeField] private Text m_Label;

	private GameObject m_Instance;
	private IPropCreator m_Creator;
	private PropDispenser m_Dispenser;
	private BoxCollider m_Box;
	private VRTK_InteractableObject m_Interactable;

	private Vector3 PreviewSize { get { return m_Box.size; } }
	private Vector3 PreviewScale { get { return m_Box.transform.lossyScale; } }

	private PropDispenser Dispenser { get { return m_Dispenser ? m_Dispenser : (m_Dispenser = GetComponentInParent<PropDispenser>()); } }

	private void Start() {
		m_Box = m_SpawnPoint.GetComponent<BoxCollider>();
		m_Box.enabled = false;
	}

	public GameObject Create() {
		UnityUtils.Destroy(m_Instance);
		var bounds = m_Creator.GetBounds();
		var scale = GetPreviewScale(bounds);

		m_Instance = m_Creator.Create(p => {
			var instance = Instantiate(p, Vector3.zero, Quaternion.identity, GetPropScaler(scale));
			instance.GetComponentsInChildren<HideInPropPreview>().ForEach(c => c.Hide());
			return instance;
		});

		GetCollideable(m_Instance, bounds);
		m_Interactable = GetInteractable(m_Instance);
		m_Interactable.InteractableObjectGrabbed += OnInstanceGrabbed;

		var center = m_Instance.transform.InverseTransformPoint(bounds.center);
		m_Instance.transform.localScale = new Vector3(scale, scale, scale);
		m_Instance.transform.localPosition = -center * scale;

		m_Label.text = GetName(m_Creator, Dispenser.PropType.TrimRegex);
		DisableColliders(m_Instance);

		return m_Instance;

		//var prefabBounds = default(Bounds);

		//var instance = m_Creator.Create(p =>
		//{
		//	prefabBounds = UnityUtils.GetBounds(p.transform);
		//	return Instantiate(p, Dispenser.SpawnPoint.position, Dispenser.SpawnPoint.rotation, GetPropParent());
		//});

		//CheckCollideable(instance);
		//CheckGrabbable(instance);

		//return instance;
	}

	private void OnInstanceGrabbed(object sender, InteractableObjectEventArgs e) {
		m_Instance.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutElastic);
		m_Interactable.InteractableObjectGrabbed -= OnInstanceGrabbed;
		m_Interactable.InteractableObjectUngrabbed += OnInstanceUngrabbed;
	}

	private void OnInstanceUngrabbed(object sender, InteractableObjectEventArgs e) {
		m_Interactable.InteractableObjectUngrabbed -= OnInstanceUngrabbed;
		m_Instance = null;
		Create();
	}

	private float GetPreviewScale(Bounds bounds) {
		var xScale = PreviewSize.x * PreviewScale.x / bounds.size.x;
		var yScale = PreviewSize.y * PreviewScale.y / bounds.size.y;
		var zScale = PreviewSize.z * PreviewScale.z / bounds.size.z;

		var scale = Mathf.Min(xScale, yScale, zScale);
		return scale;
	}

	public void Create(IPropCreator creator) {
		m_Creator = creator;
		Create();
	}

	private static Collider GetCollideable(GameObject instance, Bounds bounds) {
		var collider = instance.GetComponentInChildren<Collider>();
		if (collider) return collider;
		var boxCollider = instance.AddComponent<BoxCollider>();
		boxCollider.size = bounds.size;
		boxCollider.center = bounds.center;
		return boxCollider;
	}

	private static VRTK_InteractableObject GetInteractable(GameObject instance) {
		var interactable = instance.GetComponentInChildren<VRTK_InteractableObject>();
		if (interactable) return interactable;
		interactable = instance.AddComponent<VRTK_InteractableObject>();
		interactable.isGrabbable = true;
		interactable.touchHighlightColor = Color.yellow;
		interactable.grabAttachMechanicScript = interactable.GetOrAddComponent<VRTK_ChildOfControllerGrabAttach>();
		interactable.grabAttachMechanicScript.precisionGrab = true;
		interactable.secondaryGrabActionScript = interactable.GetOrAddComponent<VRTK_SwapControllerGrabAction>();
		return interactable;
	}

	//private static Transform m_PropRoot;

	private Transform GetPropScaler(float scale) {
		//m_PropRoot = m_PropRoot ? m_PropRoot : Dispenser.SpawnPoint ? Dispenser.SpawnPoint : new GameObject("Props").transform;
		var scaler = new GameObject(m_Creator.Name + " Scaler").transform;
		scaler.SetParent(m_SpawnPoint);
		scaler.ResetTransform();
		scaler.localScale = Vector3.one * scale; //m_Dispenser.PropType.Scale;
		return scaler;
	}

	private static string GetName(IPropCreator creator, string trimRegex) { return creator.Name.ReplaceRegex(trimRegex, "").ToSpacedName(); }

	private static void DisableColliders(GameObject instance) { instance.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false); }
}