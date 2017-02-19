using System.Collections.Generic;
using DG.Tweening;
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
		private float m_RotationSpeed;
		private float m_LastRotationTime;
		private bool m_WasTracking;
		private float m_Angle;

		public float Angle { get { return m_Angle; } private set { m_Angle = MathUtils.ClampAngle(value); } }

		private Transform Target { get { return m_Targets.GetLast(); } }

		private void Awake() { m_Transform = transform; }

		private void Start() {
			m_Tracker = GetComponent<TriggerCollisionTracker>();
			m_Tracker.OnEnter += OnEnter;
			m_Tracker.OnExit += OnExit;
		}

		private void LateUpdate() {
			CheckTarget();
			CheckRotate();
		}

		private void CheckRotate() {
			if (Target) return;

			if (m_WasTracking) {
				m_WasTracking = false;
				this.DOKill();
				var duration = Mathf.Clamp(m_RotationSpeed / (m_Rigidbody.angularDrag > 0 ? m_Rigidbody.angularDrag : 1), 0, 5);
				DOTween.To(() => m_RotationSpeed, s => m_RotationSpeed = s, 0f, duration).SetTarget(this).SetEase(Ease.OutQuad);
			}

			LogRotation();
			m_Transform.Rotate(m_Axis, m_RotationSpeed, Space.World);
			Angle += m_RotationSpeed;
		}

		private void LogRotation() {
			m_RotationDelta = m_Transform.rotation.eulerAngles.y - m_LastRotation;
			m_RotationTimeDelta = Time.time - m_LastRotationTime;
			m_LastRotation = m_Transform.rotation.eulerAngles.y;
			m_LastRotationTime = Time.time;
		}

		private void CheckTarget() {
			if (!Target) return;
			m_LookPos = ProjectPointOnPlane(m_Axis, m_Transform.position, Target.position);
			var lastLook = m_LookDir;
			m_LookDir = m_LookPos - m_Transform.position;
			var rotationAmount = SignedAngleBetween(lastLook, m_LookDir, m_Axis);
			m_Transform.Rotate(m_Axis, rotationAmount, Space.World);
			Angle += rotationAmount;

			LogRotation();
			m_RotationSpeed = m_RotationDelta * m_Push * m_RotationTimeDelta * 100;
			m_WasTracking = true;
		}

		private void FixedUpdate() { }

		private void OnEnter(Collider other) {
			m_Targets.Add(other.transform);

			if (!enabled) return;

			m_Axis = m_Transform.up;
			InitNewTarget();
		}

		private void OnExit(Collider other) {
			var wasTarget = other.transform == Target;
			m_Targets.Remove(other.transform);

			if (!Target) m_Rigidbody.AddTorque(0, m_RotationDelta * m_Push * m_RotationTimeDelta * 100, 0, ForceMode.VelocityChange);
			else if (wasTarget) InitNewTarget();
		}

		private void InitNewTarget() {
			m_LookPos = ProjectPointOnPlane(m_Axis, m_Transform.position, Target.position);
			m_LookDir = m_LookPos - m_Transform.position;
		}

		private static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) { return planePoint + Vector3.ProjectOnPlane(point - planePoint, planeNormal); }

		private static float SignedAngleBetween(Vector3 startDirection, Vector3 endDirection, Vector3 axis) {
			var angle = Vector3.Angle(startDirection, endDirection);
			var sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(startDirection, endDirection)));
			return angle * sign;
		}

		private void OnDrawGizmos() { XDebug.DrawText(transform.position, string.Format("{0:N2}: Δ{1:N2}", Angle, m_RotationDelta)); }
	}
}