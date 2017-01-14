using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class ModelSwapperMultiItem : MonoBehaviour, IMultiItem {
	[SerializeField] private ModelSwapper m_ModelSwapper;
	[SerializeField] private string m_TrimRegex;

	public IEnumerable<IItemCreator> GetItemCreators() { return m_ModelSwapper.Models.Select((m, i) => new ModelSwapperItem(gameObject, i, CleanName(m.name)) as IItemCreator); }

	private string CleanName(string nam) {
		if (!m_TrimRegex.IsNullOrEmpty()) nam = nam.ReplaceRegex(m_TrimRegex, "");
		return nam.ToSpacedName();
	}

	private class ModelSwapperItem : PrefabItemCreator {
		private readonly int m_Index;
		private readonly string m_Name;

		public ModelSwapperItem(GameObject prefab, int index, string name) : base(prefab) {
			m_Index = index;
			m_Name = name;
		}

		public override string Name { get { return m_Name; } }

		public override GameObject Create(Func<GameObject, GameObject> instantiate) {
			var instance = base.Create(instantiate);
			var modelSwapper = instance.GetComponentInChildren<ModelSwapper>();
			modelSwapper.Index = m_Index;
			return instance;
		}
	}
}