using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelSwapperMultiItem : MonoBehaviour, IMultiItem {
	[SerializeField] private ModelSwapper m_ModelSwapper;

	public IEnumerable<IItemCreator> GetItemCreators() { return m_ModelSwapper.Models.Select((model, index) => new ModelSwapperItem(gameObject, index) as IItemCreator); }

	private class ModelSwapperItem : PrefabItemCreator
	{
		private readonly int m_Index;

		public ModelSwapperItem(GameObject prefab, int index) : base(prefab) {
			m_Index = index;
		}

		public override GameObject Create(Func<GameObject, GameObject> instantiate) {
			var instance = base.Create(instantiate);
			var modelSwapper = instance.GetComponentInChildren<ModelSwapper>();
			modelSwapper.Index = m_Index;
			return instance;
		}
	}
}