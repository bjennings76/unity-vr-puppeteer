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
		transform.SetParent(PropHolder.Instance.transform);
		BackdropRoot.SetBackdrop(this);

		// Remove all interaction bits.
		GetComponentsInChildren<Collider>().ForEach(c => UnityUtils.Destroy(c));
		GetComponentsInChildren<Rigidbody>().ForEach(rb => UnityUtils.Destroy(rb));

		MatchRoot(base.StopPreview);
	}

	private void MatchRoot(Action action = null, bool instant = false) {
		action = action == null ? () => { } : action;

		if (instant) {
			transform.position = BackdropRoot.transform.TransformPoint(m_PositionOffset);
			transform.localScale = BackdropRoot.transform.localScale + m_ScaleOffset;
			transform.rotation = BackdropRoot.transform.rotation * m_RotationOffset;
		}
		else {
			transform.DOKill();
			DOTween.Sequence().SetTarget(transform)
				.Append(transform.DOMove(BackdropRoot.transform.TransformPoint(m_PositionOffset), 0.5f).SetEase(Ease.InSine))
				.Join(transform.DORotateQuaternion(BackdropRoot.transform.rotation * m_RotationOffset, 0.5f).SetEase(Ease.InSine).OnComplete(() => action()))
				.Append(transform.DOScale(BackdropRoot.transform.localScale + m_ScaleOffset, 1f).SetEase(Ease.OutElastic));
		}
	}

	private void TrackPropMovement() {
		if (transform.position.Approximately(m_LastBackgroundPosition) && 
			transform.localScale.Approximately(m_LastBackgroundScale) &&
			transform.rotation.Approximately(m_LastBackgroundRotation)) return;

		m_LastBackgroundPosition = transform.position;
		m_PositionOffset = BackdropRoot.transform.InverseTransformPoint(m_LastBackgroundPosition);

		m_LastBackgroundScale = transform.localScale;
		m_ScaleOffset = transform.localScale - BackdropRoot.transform.localScale;

		m_LastBackgroundRotation = transform.rotation;
		m_RotationOffset = Quaternion.Inverse(m_LastRootRotation) * m_LastBackgroundRotation;
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
}