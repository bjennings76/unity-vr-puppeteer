using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Utils {
	public class AverageTracker {
		public int MaxCount = 3;

		public float Value { get; private set; }

		private readonly List<float> m_List = new List<float>();

		private int m_Index;

		public void Add(float num) {
			if (m_List.Count < MaxCount) { m_List.Add(num); }
			else {
				m_List[m_Index] = num;
				m_Index = (m_Index + 1) % MaxCount;
			}
			Value = m_List.Average();
		}

		public void Reset() {
			m_List.Clear();
			m_Index = 0;
		}
	}

	[RequireComponent(typeof(TriggerCollisionTracker))]
	public class Spinner : MonoBehaviour {
		[SerializeField] private float m_MaxSpeed = 10f;
		[SerializeField] private float m_Push = 1.2f;
		[SerializeField] private Rigidbody m_Rigidbody;
		[SerializeField] private Collider m_Collider;
		[SerializeField, Range(2, 10)] private int m_FrameAverageCount = 3;

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
		private float m_Angle;

		private readonly AverageTracker m_FrameAverage = new AverageTracker();

		public float Angle { get { return m_Angle; } private set { m_Angle = MathUtils.ClampAngle(value); } }

		private Transform Target { get { return m_Targets.GetLast(); } }

		private void Awake() { m_Transform = transform; }

		private void Start() {
			m_Tracker = GetComponent<TriggerCollisionTracker>();
			m_Tracker.OnEnter += OnEnter;
			m_Tracker.OnExit += OnExit;
			m_FrameAverage.MaxCount = m_FrameAverageCount;
		}

		private void LateUpdate() {
			if (Target) UpdateTarget();
			else UpdateRotation();
		}

		private void UpdateRotation() {
			m_Transform.Rotate(m_Axis, m_RotationSpeed, Space.World);
			Angle += m_RotationSpeed;
		}

		private void UpdateTarget() {
			m_LookPos = ProjectPointOnPlane(m_Axis, m_Transform.position, Target.position);
			var lastLook = m_LookDir;
			m_LookDir = m_LookPos - m_Transform.position;
			var rotationAmount = SignedAngleBetween(lastLook, m_LookDir, m_Axis);
			m_Transform.Rotate(m_Axis, rotationAmount, Space.World);
			Angle += rotationAmount;

			m_FrameAverage.Add(Mathf.Clamp((m_Transform.rotation.eulerAngles.y - m_LastRotation) * m_Push, -m_MaxSpeed, m_MaxSpeed));
			m_RotationDelta = m_FrameAverage.Value;
			m_RotationTimeDelta = Time.time - m_LastRotationTime;
			m_LastRotation = m_Transform.rotation.eulerAngles.y;
			m_LastRotationTime = Time.time;
			m_RotationSpeed = m_RotationDelta * m_RotationTimeDelta * 100;
			XDebug.Log("Δ: " + m_RotationDelta.ToString("N2"));
		}

		private void OnEnter(Collider other) {
			m_Targets.Add(other.transform);

			if (!enabled) return;

			m_Axis = m_Transform.up;
			InitNewTarget();
		}

		private void OnExit(Collider other) {
			var lastTarget = Target;
			m_Targets.Remove(other.transform);

			if (!Target) StartRotation();
			else if (lastTarget != Target) InitNewTarget();
		}

		private void StartRotation() {
			this.DOKill();
			if (m_Collider) {
				m_Collider.enabled = false;
				UnityUtils.DelayAction(0.05f, () => m_Collider.enabled = true, this);
			}
			m_FrameAverage.Reset();
			XDebug.Log("Release at Δ " + m_RotationSpeed.ToString("N2"));
			var duration = Mathf.Clamp(Mathf.Abs(m_RotationSpeed) / (m_Rigidbody.angularDrag > 0 ? m_Rigidbody.angularDrag : 1), 0, 5);
			DOTween.To(() => m_RotationSpeed, s => m_RotationSpeed = s, 0f, duration).SetTarget(this).SetEase(Ease.OutQuad);
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