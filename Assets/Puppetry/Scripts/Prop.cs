using DG.Tweening;
using UnityEngine;
using Utils;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

public class Prop : MonoBehaviour {
	[SerializeField] private bool m_InPreview;

	private HideInPropPreview[] m_HideInPreviewBits;
	private bool m_LastInPreview;
	private PropConfig m_Config;

	private bool InPreview {
		get { return m_InPreview; }
		set {
			if (m_InPreview == value) return;
			m_InPreview = value;
			PreviewChanged();
		}
	}

	private HideInPropPreview[] HideInPreviewBits { get { return m_HideInPreviewBits != null ? m_HideInPreviewBits : (m_HideInPreviewBits = GetComponentsInChildren<HideInPropPreview>()); } }


	private void PreviewChanged() {
		m_LastInPreview = m_InPreview;
		HideInPreviewBits.ForEach(h => h.SetActive(!InPreview));
	}

	protected virtual void Update() {
		if (m_InPreview != m_LastInPreview) PreviewChanged();
	}

	internal void StartPreview(PropConfig config, Bounds colliderBounds) {
		m_Config = config;
		InPreview = true;
		SetUpCollider(gameObject, colliderBounds);
		SetUpInteractable(gameObject);
	}

	public virtual void PreviewGrabbed() {
		if (m_Config.ScaleStyle != PropConfig.PreviewScaleStyle.ActualSize) transform.DOScale(Vector3.one * m_Config.Scale, 1).SetEase(Ease.OutElastic);
	}

	public virtual void StopPreview() {
		transform.SetParent(PropHolder.Instance.transform);
		InPreview = false;
	}

	private static void SetUpCollider(GameObject instance, Bounds bounds) {
		if (instance.GetComponentInChildren<Collider>()) return;
		var boxCollider = instance.AddComponent<BoxCollider>();
		boxCollider.size = bounds.size;
		boxCollider.center = bounds.center;
	}

	private static void SetUpInteractable(GameObject instance) {
		if (instance.GetComponentInChildren<VRTK_InteractableObject>()) return;
		var interactable = instance.AddComponent<VRTK_InteractableObject>();
		interactable.isGrabbable = true;
		interactable.touchHighlightColor = Color.yellow;
		interactable.grabAttachMechanicScript = interactable.GetOrAddComponent<VRTK_ChildOfControllerGrabAttach>();
		interactable.grabAttachMechanicScript.precisionGrab = true;
		interactable.secondaryGrabActionScript = interactable.GetOrAddComponent<VRTK_SwapControllerGrabAction>();
	}
}