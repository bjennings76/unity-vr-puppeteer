using System;
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
	private bool m_Init;
	private Transform m_Scaler;
	private static Transform m_PropRoot;

	private Vector3 PreviewSize { get { return m_Box.size; } }
	private Vector3 PreviewScale { get { return m_Box.transform.lossyScale; } }

	private PropDispenser Dispenser { get { return m_Dispenser ? m_Dispenser : (m_Dispenser = GetComponentInParent<PropDispenser>()); } }

	private PropType PropType { get { return Dispenser ? Dispenser.PropType : null; } }

	private static Transform PropRoot { get { return m_PropRoot ? m_PropRoot : (m_PropRoot = new GameObject("Props").transform); } }

	private void Init() {
		if (m_Init) return;
		m_Init = true;
		m_Box = m_SpawnPoint.GetComponent<BoxCollider>();
		m_Box.enabled = false;
	}

	public void Create(IPropCreator creator) {
		m_Creator = creator;
		Create();
	}

	public GameObject Create() {
		Init();

		UnityUtils.Destroy(m_Instance);
		var scale = 1f;
		var bounds = m_Creator.GetPreviewBounds();

		switch (PropType.ScaleStyle) {
			case PropType.PreviewScaleStyle.BoundingBox:
				scale = GetPreviewScale(bounds);
				break;
			case PropType.PreviewScaleStyle.ActualSize:
				scale = PropType.Scale;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		m_Scaler = GetPropScaler(scale);

		if (PropType.ScaleStyle == PropType.PreviewScaleStyle.BoundingBox) {
		}

		var position = PropType.MountOnPivot ? m_SpawnPoint.position : -bounds.center * scale + m_SpawnPoint.position;
		var rotation = m_SpawnPoint.rotation;

		m_Instance = m_Creator.Create(p => {
			var instance = Instantiate(p, position, rotation, m_Scaler);
			instance.GetComponentsInChildren<HideInPropPreview>().ForEach(c => c.Hide());
			return instance;
		});

		GetCollideable(m_Instance, bounds);
		m_Interactable = GetInteractable(m_Instance);
		m_Interactable.InteractableObjectGrabbed += OnInstanceGrabbed;
		m_Label.text = PropType.GetName(m_Creator.Name);

		return m_Instance;
	}

	private void OnInstanceGrabbed(object sender, InteractableObjectEventArgs e) {
		if (PropType.ScaleStyle != PropType.PreviewScaleStyle.ActualSize) m_Instance.transform.DOScale(Vector3.one * PropType.Scale, 1).SetEase(Ease.OutElastic);
		m_Interactable.InteractableObjectGrabbed -= OnInstanceGrabbed;
		m_Interactable.InteractableObjectUngrabbed += OnInstanceUngrabbed;
	}

	private void OnInstanceUngrabbed(object sender, InteractableObjectEventArgs e) {
		m_Interactable.InteractableObjectUngrabbed -= OnInstanceUngrabbed;
		m_Instance.transform.SetParent(PropRoot);
		m_Instance.GetComponentsInChildren<HideInPropPreview>().ForEach(c => c.Show());
		UnityUtils.Destroy(m_Scaler);
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

	private Transform GetPropScaler(float scale) {
		var scaler = new GameObject(m_Creator.Name + " Scaler").transform;
		scaler.SetParent(m_SpawnPoint);
		scaler.ResetTransform();
		scaler.localScale = Vector3.one * scale;
		return scaler;
	}
}