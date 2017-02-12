using System;
using UnityEngine;
using Utils;

[Serializable]
public class PrefabTweakConfig {
	[SerializeField] private GameObject m_Prefab;

	public string Name { get { return StringUtils.CamelCaseSplitName(Prefab.name); } }

	public GameObject Prefab { get { return m_Prefab; } set { m_Prefab = value; } }

	public PrefabTweakConfig() { }

	public PrefabTweakConfig(GameObject prefab) { m_Prefab = prefab; }
}