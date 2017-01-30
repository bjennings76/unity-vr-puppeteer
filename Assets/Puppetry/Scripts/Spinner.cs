using System.Collections.Generic;
using UnityEngine;

namespace Utils {
	[RequireComponent(typeof(TriggerCollisionTracker))]
	public class Spinner : MonoBehaviour {
		[SerializeField] private float m_Push = 1.2f;
		[SerializeField] private Rigidbody m_Rigidbody;

		private readonly List<Transform> m_Targets = new List<Transform>();
		private float m_LastRotation;
		private float m_RotationDelta;
		private float m_RotationTimeDelta;
		private Vector3 m_Axis;
		private TriggerCollisionTracker m_Tracker;

		private Vector3 m_LookPos;
		private Vector3 m_LookDir;
		private Transform m_Transform;

		private Transform Target { get { return m_Targets.GetLast(); } }

		private void Awake() { m_Transform = transform; }

		private void Start() {
			m_Tracker = GetComponent<TriggerCollisionTracker>();
			m_Tracker.OnEnter += OnEnter;
			m_Tracker.OnExit += OnExit;
		}

		private void LateUpdate() {
			m_RotationDelta = m_Transform.rotation.eulerAngles.y - m_LastRotation;
			m_RotationTimeDelta = Time.deltaTime;
			m_LastRotation = m_Transform.rotation.eulerAngles.y;

			if (!Target) return;

			m_LookPos = ProjectPointOnPlane(m_Axis, m_Transform.position, Target.position);
			var lastLook = m_LookDir;
			m_LookDir = m_LookPos - m_Transform.position;
			var angle = SignedAngleBetween(lastLook, m_LookDir, m_Axis);
			m_Transform.Rotate(m_Axis, angle, Space.World);
		}

		private void OnEnter(Collider other)
		{
			m_Targets.Add(other.transform);

			if (!enabled)
				return;

			m_Rigidbody.isKinematic = true;
			m_Axis = m_Transform.up;
			InitNewTarget();
		}

		private void OnExit(Collider other) {
			var wasTarget = other.transform == Target;
			m_Targets.Remove(other.transform);

			if (!Target) {
				m_Rigidbody.isKinematic = false;
				m_Rigidbody.AddTorque(0, m_RotationDelta * m_Push * m_RotationTimeDelta * 100, 0, ForceMode.VelocityChange);
			}
			else if (wasTarget) { InitNewTarget(); }
		}

		private void InitNewTarget()
		{
			m_LookPos = ProjectPointOnPlane(m_Axis, m_Transform.position, Target.position);
			m_LookDir = m_LookPos - m_Transform.position;
		}

		private static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) { return planePoint + Vector3.ProjectOnPlane(point - planePoint, planeNormal); }

		private static float SignedAngleBetween(Vector3 startDirection, Vector3 endDirection, Vector3 axis) {
			var angle = Vector3.Angle(startDirection, endDirection);
			var sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(startDirection, endDirection)));
			return angle * sign;
		}

		private void OnDrawGizmos() { XDebug.DrawText(transform.position, m_RotationDelta.ToString("N2")); }
	}
}