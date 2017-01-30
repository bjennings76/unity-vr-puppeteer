using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils {
	public class TriggerCollisionTracker : MonoBehaviour {
		private readonly List<CollisionInteraction> m_Interactions = new List<CollisionInteraction>();
		[SerializeField] private Collider m_Collider;

		public event Action<Collider> OnEnter;
		public event Action<Collider> OnExit;
		public event Action<Collider> OnChanged;

		protected void Update() { CleanIntersectingColliders(); }

		protected void OnTriggerEnter(Collider c) { AddIntersectingCollider(c, m_Collider); }

		protected void OnTriggerExit(Collider c) { RemoveIntersectingCollider(c); }

		private void RaiseOnEnter(Collider c) {
			if (OnEnter != null) {
				XDebug.AssertDelegate(OnEnter, this);
				OnEnter(c);
				XDebug.Log(this, "--> {0}", c);
			}

			RaiseOnChanged(c);
		}

		private void RaiseOnExit(Collider c) {
			if (OnExit != null) {
				XDebug.AssertDelegate(OnExit, this);
				OnExit(c);
				XDebug.Log(this, "<-- {0}", c);
			}

			RaiseOnChanged(c);
		}

		private void RaiseOnChanged(Collider c) {
			if (OnChanged != null) {
				XDebug.AssertDelegate(OnChanged, this);
				OnChanged(c);
			}
		}

		private void CleanIntersectingColliders() {
			// If there are no 'bad' colliders, leave the interacting collider list alone.
			if (m_Interactions.All(c => c.IsInteracting)) return;

			var bad = m_Interactions.Where(c => !c.IsInteracting).ToList();

			foreach (var ci in bad) RemoveIntersectingCollider(ci.TargetCollider);
		}

		private void AddIntersectingCollider(Collider targetCollider, Collider sourceCollider) {
			if (m_Interactions.All(ic => ic.TargetCollider != targetCollider)) {
				RaiseOnEnter(targetCollider);
				m_Interactions.Add(new CollisionInteraction(sourceCollider, targetCollider));
			}
		}

		private void RemoveIntersectingCollider(Collider targetCollider) {
			var collisionInteraction = m_Interactions.FirstOrDefault(ic => ic.TargetCollider == targetCollider);

			if (collisionInteraction != null) {
				if (targetCollider != null) RaiseOnExit(targetCollider);
				m_Interactions.Remove(collisionInteraction);
			}
		}

		public Collider GetInteractingCollider(Predicate<Collider> isValid = null) {
			isValid = isValid ?? (obj => obj);

			return m_Interactions.Where(i => i.TargetCollider && isValid(i.TargetCollider)).Select(i => i.TargetCollider).OrderBy(c => transform.position.SqrDistanceTo(c.transform.position)).FirstOrDefault();
		}

		public T GetInteractingComponent<T>(Func<T, bool> isValid = null, Func<T, float> getDistance = null) where T : Component {
			isValid = isValid ?? (c => c);
			getDistance = getDistance ?? (c => transform.position.SqrDistanceTo(c.transform.position));

			return m_Interactions.Where(i => i != null && i.TargetCollider).Select(i => i.TargetCollider.GetComponent<T>()).Where(isValid).OrderBy(getDistance).FirstOrDefault();
		}

		public T GetInteractingComponentInParents<T>(Func<T, bool> isValid = null, Func<T, float> getDistance = null) where T : Component {
			isValid = isValid ?? (c => c);
			getDistance = getDistance ?? (c => transform.position.SqrDistanceTo(c.transform.position));

			return m_Interactions.Select(i => i.TargetCollider.GetComponentInParent<T>()).Where(isValid).OrderBy(getDistance).FirstOrDefault();
		}
	}

	public sealed class CollisionInteraction {
		public Collider TargetCollider { get; private set; }
		private Collider SourceCollider { get; set; }

		public bool IsInteracting {
			get {
				if (!SourceCollider || !TargetCollider) return false;

				if (!TargetCollider.gameObject.activeInHierarchy) return false;

				var result = SourceCollider.bounds.Intersects(TargetCollider.bounds);
				return result;
			}
		}

		public CollisionInteraction(Collider source, Collider target) {
			SourceCollider = source;
			TargetCollider = target;
		}
	}
}