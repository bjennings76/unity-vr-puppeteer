using System;
using DG.Tweening;
using UnityEngine;
using Utils;

[ExecuteInEditMode]
public class BackdropProp : Prop {
	[SerializeField, ReadOnly] private BackdropRoot m_Root;
	[SerializeField, ReadOnly] private Vector3 m_PositionOffset;
	[SerializeField, ReadOnly] private Vector3 m_ScaleOffset;
	[SerializeField, ReadOnly] private Quaternion m_RotationOffset;

	private Vector3 m_LastRootPosition;
	private Vector3 m_LastRootScale;
	private Quaternion m_LastRootRotation;
	private Vector3 m_LastBackgroundPosition;
	private Vector3 m_LastBackgroundScale;
	private Quaternion m_LastBackgroundRotation;

	private BackdropRoot BackdropRoot { get { return m_Root ? m_Root : (m_Root = FindObjectOfType<BackdropRoot>()); } }

	protected void Start() {
		if (!Application.isPlaying) { StopPreview(); }
	}

	protected override void Update() {
		base.Update();
		if (BackdropRoot && !Application.isPlaying) {
			TrackPropMovement();
			TrackRootMovement();
		}
	}

	public override void PreviewGrabbed() {
		// Disable transition when the preview is grabbed by not calling the base function.
	}

	public override void StopPreview() {
		BackdropRoot.SetBackdrop(this);

		// Remove all interaction bits.
		GetComponentsInChildren<Collider>().ForEach(c => UnityUtils.Destroy(c));
		GetComponentsInChildren<Rigidbody>().ForEach(rb => UnityUtils.Destroy(rb));

		MatchRoot(() => InPreview = false);
	}

	private void TrackPropMovement() {
		if (Application.isPlaying) return;

		if (transform.position.Approximately(m_LastBackgroundPosition) && 
			transform.localScale.Approximately(m_LastBackgroundScale) &&
			transform.rotation.Approximately(m_LastBackgroundRotation)) return;

		m_LastBackgroundPosition = transform.position;
		m_PositionOffset = BackdropRoot.transform.InverseTransformPoint(m_LastBackgroundPosition);

		m_LastBackgroundScale = transform.lossyScale;
		m_ScaleOffset = transform.lossyScale - BackdropRoot.transform.lossyScale;

		m_LastBackgroundRotation = transform.rotation;
		m_RotationOffset = BackdropRoot.transform.InverseTransformRotation(m_LastBackgroundRotation);
	}

	private void TrackRootMovement() {

		if (BackdropRoot.transform.position.Approximately(m_LastRootPosition) && 
			BackdropRoot.transform.localScale.Approximately(m_LastRootScale) &&
			BackdropRoot.transform.rotation.Approximately(m_LastRootRotation)) return;

		m_LastRootPosition = BackdropRoot.transform.position;
			m_LastRootScale = BackdropRoot.transform.localScale;
			m_LastRootRotation = BackdropRoot.transform.rotation;

		MatchRoot(null, true);

	}

	private void MatchRoot(Action action = null, bool instant = false) {
		action = action == null ? () => { } : action;
		transform.DOKill();

		if (instant) {
			transform.localPosition = m_PositionOffset;
			transform.localScale = Vector3.one + m_ScaleOffset;
			transform.localRotation = m_RotationOffset;
		}
		else {
			DOTween.Sequence().SetTarget(transform)
				.Append(transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InSine))
				.Join(transform.DOLocalRotateQuaternion(m_RotationOffset, 0.5f).SetEase(Ease.InSine).OnComplete(() => action()))
				.Append(transform.DOScale(Vector3.one + m_ScaleOffset, 1f).SetEase(Ease.OutElastic))
				.Join(transform.DOLocalMove(m_PositionOffset, 1f).SetEase(Ease.OutElastic));
		}
	}
}