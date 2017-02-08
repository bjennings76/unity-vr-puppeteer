using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class PrefabPropCreator : IPropCreator {
	private readonly GameObject m_Prefab;

	public virtual string Name { get { return m_Prefab ? m_Prefab.name : "<null> :("; } }

	public PrefabPropCreator(GameObject prefab) { m_Prefab = prefab; }

	public virtual GameObject Create(Func<GameObject, GameObject> instantiate) {
		var instance = instantiate(m_Prefab);
		instance.GetOrAddComponent<Prop>();
		return instance;
	}

	public Bounds GetPreviewBounds() {
		var hiders = m_Prefab.GetComponentsInChildren<HideInPropPreview>(true).Where(h => h.gameObject.activeSelf).ToArray();
		hiders.ForEach(h => h.gameObject.SetActive(false));
		var bounds = UnityUtils.GetBounds(m_Prefab.transform);
		hiders.ForEach(h => h.gameObject.SetActive(true));
		return bounds;
	}
}

public interface IPropCreator {
	string Name { get; }
	GameObject Create(Func<GameObject, GameObject> instantiate);
	Bounds GetPreviewBounds();
}

public interface IMultiProp {
	IEnumerable<IPropCreator> GetPropCreators();
}