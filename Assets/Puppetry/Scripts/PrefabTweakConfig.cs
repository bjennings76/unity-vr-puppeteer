using System;
using UnityEngine;

[Serializable]
public class PrefabTweakConfig {
	[SerializeField] private GameObject m_Prefab;

	[SerializeField, HideInInspector] private string m_Name;

	public string Name { get { return m_Name; } set { m_Name = value; } }

	public GameObject Prefab { get { return m_Prefab; } set { m_Prefab = value; } }

	public PrefabTweakConfig() { }

	public PrefabTweakConfig(GameObject prefab, string name) {
		m_Prefab = prefab;
		m_Name = name;
	}
}