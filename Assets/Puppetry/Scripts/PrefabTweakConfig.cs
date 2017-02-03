using System;
using UnityEngine;

[Serializable]
public class PrefabTweakConfig {
	[SerializeField] private GameObject m_Prefab;

	public PrefabTweakConfig() { }

	public PrefabTweakConfig(GameObject prefab) { m_Prefab = prefab; }

	public GameObject Prefab { get { return m_Prefab; } }

	private void Start() { }

	private void Update() { }
}