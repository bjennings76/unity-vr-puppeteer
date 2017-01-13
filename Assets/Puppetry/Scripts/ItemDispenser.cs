using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ItemDispenser : MonoBehaviour {
	[SerializeField] private GameObject[] m_ItemPrefabs;
	[SerializeField] private ItemSlot[] m_ItemSlots;

	private List<IItemCreator> m_ItemList;

	private int Cursor {
		get { return m_Cursor; }
		set {
			m_Cursor = value;
			m_Cursor = Mathf.Clamp(m_Cursor, 0, m_ItemList == null ? 0 : m_ItemList.Count - 1);
		}
	}
	private int m_Cursor;

	private void Start() {
		m_ItemSlots = GetComponentsInChildren<ItemSlot>();
		PopulateItemList();
		UpdateItemSlots();
	}

	private void UpdateItemSlots() {
		m_ItemSlots.ForEach(slot => {
			slot.Spawn(m_ItemList[Cursor]);
			Cursor++;
		});
	}

	private void PopulateItemList() {
		var list = new List<IItemCreator>();

		foreach (var prefab in m_ItemPrefabs) {
			var multiItem = prefab.GetComponent<IMultiItem>();
			if (multiItem != null) list.AddRange(multiItem.GetItemCreators());
			else list.Add(new PrefabItemCreator(prefab));
		}

		m_ItemList = list;
	}
}
