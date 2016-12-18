using System;
using UnityEngine;

namespace Utils {
	[Serializable]
	public class RandomIntegerRange {
		[SerializeField]
		[Tooltip("Inclusive minimum")]
		private int m_Min = 1;

		[SerializeField]
		[Tooltip("Inclusive maximum")]
		private int m_Max = 1;

		public int GetRandomValue() {
			// add 1 to change from Unity's exclusive range to the inclusive we want.
			return UnityEngine.Random.Range(m_Min, m_Max + 1);
		}
	}

	[Serializable]
	public class RandomFloatRange {
		[SerializeField]
		[Tooltip("Inclusive minimum")]
		private float m_Min = 0.0f;

		[SerializeField]
		[Tooltip("Inclusive maximum")]
		private float m_Max = 1.0f;

		public float GetRandomValue() {
			return UnityEngine.Random.Range(m_Min, m_Max);
		}
	}
}
