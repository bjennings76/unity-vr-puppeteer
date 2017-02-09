using System;
using System.Linq;
using UnityEngine;
using Utils;

public class JointScaler : MonoBehaviour {
	public ScalableJoint[] Joints;

	private void Start() { Joints = GetComponentsInChildren<Joint>().Select(j => new ScalableJoint(j)).ToArray(); }

	private void FixedUpdate() { Joints.ForEach(j => j.FixedUpdate()); }

	[Serializable]
	public class ScalableJoint {
		private readonly Joint m_Joint;
		private readonly Vector3 m_InitialAnchor;
		private readonly Vector3 m_InitialConnectedAnchor;
		private readonly float m_InitialScale;

		private float m_LastScale;

		private float Ratio { get { return m_LastScale / m_InitialScale; } }

		public ScalableJoint(Joint joint) {
			m_Joint = joint;
			m_InitialAnchor = joint.anchor;
			m_InitialConnectedAnchor = joint.connectedAnchor;
			m_InitialScale = joint.transform.lossyScale.x;
		}

		public void FixedUpdate() {
			if (m_LastScale.Approximately(m_Joint.transform.lossyScale.x)) return;
			m_LastScale = m_Joint.transform.lossyScale.x;
			m_Joint.anchor = m_InitialAnchor * Ratio;
			m_Joint.connectedAnchor = m_InitialConnectedAnchor * Ratio;
		}
	}
}