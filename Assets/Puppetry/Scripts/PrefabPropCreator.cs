using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PrefabPropCreator : IPropCreator {
	private readonly GameObject m_Prefab;

	public virtual string Name { get { return m_Prefab ? m_Prefab.name : "<null> :("; } }

	public PrefabPropCreator(GameObject prefab) { m_Prefab = prefab; }

	public virtual GameObject Create(Func<GameObject, GameObject> instantiate) { return instantiate(m_Prefab); }

	public Bounds GetBounds() {
		return UnityUtils.GetBounds(m_Prefab.transform);
	}
}

public interface IPropCreator {
	string Name { get; }
	GameObject Create(Func<GameObject, GameObject> instantiate);
	Bounds GetBounds();
}

public interface IMultiProp {
	IEnumerable<IPropCreator> GetPropCreators();
}