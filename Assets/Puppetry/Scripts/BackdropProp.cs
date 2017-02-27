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

	private BackdropRoot Root { get { return m_Root ? m_Root : (m_Root = FindObjectOfType<BackdropRoot>()); } }

	protected override void Update() {
		base.Update();
		if (Root && !Application.isPlaying) {
			TrackPropMovement();
			TrackRootMovement();
		}
	}

	protected override void PreviewChanged() {
		base.PreviewChanged();
		if (!InPreview) MatchRoot();
	}

	public override void PreviewGrabbed() { }

	private void MatchRoot() {
		Root.SetBackdrop(this);
		transform.position = Root.transform.TransformPoint(m_PositionOffset);
		transform.localScale = Root.transform.localScale + m_ScaleOffset;
		transform.rotation = Root.transform.rotation * m_RotationOffset;
	}

	public override void PreviewReleased() {
		base.PreviewReleased();

		// Remove all interaction bits.
		GetComponentsInChildren<Collider>().ForEach(c => UnityUtils.Destroy(c));
		GetComponentsInChildren<Rigidbody>().ForEach(rb => UnityUtils.Destroy(rb));
	}

	private void TrackPropMovement() {
		if (transform.position.Approximately(m_LastBackgroundPosition) && 
			transform.localScale.Approximately(m_LastBackgroundScale) &&
			transform.rotation.Approximately(m_LastBackgroundRotation)) return;

		m_LastBackgroundPosition = transform.position;
		m_PositionOffset = Root.transform.InverseTransformPoint(m_LastBackgroundPosition);

		m_LastBackgroundScale = transform.localScale;
		m_ScaleOffset = transform.localScale - Root.transform.localScale;

		m_LastBackgroundRotation = transform.rotation;
		m_RotationOffset = Quaternion.Inverse(m_LastRootRotation) * m_LastBackgroundRotation;
	}

	private void TrackRootMovement() {
		if (Root.transform.position.Approximately(m_LastRootPosition) && 
			Root.transform.localScale.Approximately(m_LastRootScale) &&
			Root.transform.rotation.Approximately(m_LastRootRotation)) return;

		m_LastRootPosition = Root.transform.position;
		m_LastRootScale = Root.transform.localScale;
		m_LastRootRotation = Root.transform.rotation;
		MatchRoot();
	}
}