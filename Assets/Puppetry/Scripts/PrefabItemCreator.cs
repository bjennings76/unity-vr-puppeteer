using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabItemCreator : IItemCreator {
	private readonly GameObject m_Prefab;

	public PrefabItemCreator(GameObject prefab) { m_Prefab = prefab; }

	public virtual GameObject Create(Func<GameObject, GameObject> instantiate) { return instantiate(m_Prefab); }
}

public interface IItemCreator {
	GameObject Create(Func<GameObject, GameObject> instantiate);
}

public interface IMultiItem {
	IEnumerable<IItemCreator> GetItemCreators();
}