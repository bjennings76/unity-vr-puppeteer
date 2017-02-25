using UnityEngine;
using Utils;

[ExecuteInEditMode]
public class BackdropProp : Prop {
	[SerializeField, ReadOnly] private BackdropRoot m_Root;
	[SerializeField, ReadOnly] private Vector3 m_PositionOffset;
	[SerializeField, ReadOnly] private Quaternion m_RotationOffset;

	private Vector3 m_LastRootPosition;
	private Quaternion m_LastRootRotation;
	private Vector3 m_LastBackgroundPosition;
	private Quaternion m_LastBackgroundRotation;

	private BackdropRoot Root { get { return m_Root ? m_Root : (m_Root = FindObjectOfType<BackdropRoot>()); } }

	private void Start() {
		if (!Root) return;

		Root.SetBackdrop(this);
		MatchRoot();
	}

	private void Update() {
		if (!Root) return;
		TrackRootMovement();
		TrackPropMovement();
	}

	private void MatchRoot() {
		if (!Root) return;
		transform.position = Root.transform.TransformPoint(m_PositionOffset);
		transform.rotation = Root.transform.rotation * m_RotationOffset;
	}

	private void TrackPropMovement() {
		if (transform.position.Approximately(m_LastBackgroundPosition) && transform.rotation.Approximately(m_LastBackgroundRotation)) return;

		m_LastBackgroundPosition = transform.position;
		m_PositionOffset = Root.transform.InverseTransformPoint(m_LastBackgroundPosition);

		m_LastBackgroundRotation = transform.rotation;
		m_RotationOffset = Quaternion.Inverse(m_LastRootRotation) * m_LastBackgroundRotation;
	}

	private void TrackRootMovement() {
		if (Root.transform.position.Approximately(m_LastRootPosition) && Root.transform.rotation.Approximately(m_LastRootRotation)) return;

		m_LastRootPosition = Root.transform.position;
		m_LastRootRotation = Root.transform.rotation;
		MatchRoot();
	}
}