using System;
using UnityEngine;
using Utils;

[Serializable]
public class PrefabTweakConfig {
	[SerializeField] private GameObject m_Prefab;
	[SerializeField] private Vector3 m_HandlePosition;
	[SerializeField] private Vector3 m_HandleRotation;

	public string Name {  get { return StringUtils.CamelCaseSplitName(Prefab.name); } }

	public GameObject Prefab { get { return m_Prefab; } set { m_Prefab = value; } }

	public Vector3 HandlePosition { get { return m_HandlePosition; } set { m_HandlePosition = value; } }
	public Vector3 HandleRotation { get { return m_HandleRotation; } set { m_HandleRotation = value; } }

	public PrefabTweakConfig() { }

	public PrefabTweakConfig(GameObject prefab) { m_Prefab = prefab; }

	private void Start() { }

	private void Update() { }
}