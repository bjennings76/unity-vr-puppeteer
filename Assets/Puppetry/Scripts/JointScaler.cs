using System.Linq;
using UnityEngine;
using Utils;

public class JointScaler : MonoBehaviour {
	[Range(0.05f, 2)] public float Scale = 1;

	private ScalableJoint[] m_Joints;
	private float m_LastScale;
	private Vector3 m_OriginalScale;

	private void Start() {
		m_LastScale = Scale = 1;
		m_OriginalScale = transform.localScale;
		m_Joints = GetComponentsInChildren<Joint>().Select(GetScaleableJoint).ToArray();
	}

	private static ScalableJoint GetScaleableJoint(Joint joint) {
		var springJoint = joint as SpringJoint;
		if (springJoint != null) return new ScalableSprintJoint(springJoint);

		var characterJoint = joint as CharacterJoint;
		if (characterJoint != null) return new ScalableCharacterJoint(characterJoint);

		Debug.LogWarning("No custom joint handling for " + joint.name, joint);
		return new ScalableJoint(joint);
	}

	private void FixedUpdate() {
		if (Scale.Approximately(m_LastScale)) return;

		m_LastScale = Scale;
		var disconnected = DisconnectJoints();
		transform.localScale = m_OriginalScale * Scale;
		ScaleJoints();
		ReconnectJoints(disconnected);
	}

	private void ScaleJoints() { m_Joints.ForEach(j => j.Scale(Scale)); }

	private Rigidbody[] DisconnectJoints() {
		var disconnected = new Rigidbody[m_Joints.Length];
		for (var i = 0; i < m_Joints.Length; i++) disconnected[i] = m_Joints[i].Disconnect();
		return disconnected;
	}

	private void ReconnectJoints(Rigidbody[] disconnected) {
		for (var i = 0; i < m_Joints.Length; i++) m_Joints[i].Reconnect(disconnected[i]);
	}

	private class ScalableJoint {
		private readonly Joint m_Joint;
		private readonly Vector3 m_OriginalAnchor;
		private readonly Vector3 m_OriginalConnectedAnchor;
		private readonly Vector3 m_OriginalScale;

		public ScalableJoint(Joint joint) {
			m_Joint = joint;
			m_OriginalAnchor = joint.anchor;
			m_OriginalConnectedAnchor = joint.connectedAnchor;
			m_OriginalScale = joint.transform.lossyScale;
		}

		public virtual void Scale(float scale) {
			//var ratio = m_Joint.transform.lossyScale.x / m_OriginalScale.x;
			//m_Joint.anchor = m_OriginalAnchor * scale;
			//m_Joint.connectedAnchor = m_OriginalConnectedAnchor * scale;
		}

		public Rigidbody Disconnect() {
			var rb = m_Joint.connectedBody;
			m_Joint.connectedBody = null;
			return rb;
		}

		public void Reconnect(Rigidbody rb) {
			m_Joint.connectedBody = rb;
		}
	}

	private class ScalableSprintJoint : ScalableJoint {
		private readonly float m_OriginalMinDistance;
		private readonly float m_OriginalMaxDistance;
		private readonly SpringJoint m_SpringJoint;

		public ScalableSprintJoint(SpringJoint joint) : base(joint) {
			m_SpringJoint = joint;
			m_OriginalMinDistance = m_SpringJoint.minDistance;
			m_OriginalMaxDistance = m_SpringJoint.maxDistance;
		}

		public override void Scale(float scale) {
			base.Scale(scale);
			m_SpringJoint.minDistance = scale * m_OriginalMinDistance;
			m_SpringJoint.maxDistance = scale * m_OriginalMaxDistance;
		}
	}

	private class ScalableCharacterJoint : ScalableJoint {
		private readonly CharacterJoint m_CharacterJoint;

		public ScalableCharacterJoint(CharacterJoint joint) : base(joint) { m_CharacterJoint = joint; }
	}
}