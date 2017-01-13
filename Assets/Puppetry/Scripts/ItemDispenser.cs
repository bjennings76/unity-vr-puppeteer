using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ItemDispenser : MonoBehaviour {
	[SerializeField] private GameObject[] m_ItemPrefabs;
	[SerializeField] private ItemSlot[] m_ItemSlots;
	[SerializeField] private HingeJoint m_Hinge;

	private List<IItemCreator> m_ItemList;
	private float m_LastAngle;
	private int m_Cursor;

	private int SlotCount {  get { return m_ItemSlots == null || m_ItemSlots.Length == 0 ? 0 : m_ItemSlots.Length; } }
	private int ItemCount { get { return m_ItemList == null || m_ItemList.Count == 0 ? 0 : m_ItemList.Count; } }

	private int Cursor {
		get { return m_Cursor; }
		set {
			m_Cursor = value;
			m_Cursor = ItemCount > 0 ? MathUtils.Mod(m_Cursor, ItemCount) : 0;
		}
	}

	private int BackCursor { get { return ItemCount > 0 ? MathUtils.Mod(Cursor - SlotCount, ItemCount) : 0; } }

	private void Start() {
		m_ItemSlots = GetComponentsInChildren<ItemSlot>();
		PopulateItemList();
		PopulateItemSlots();
	}

	private void Update() {
		var lastAngle = m_LastAngle;
		var currentAngle = m_LastAngle = m_Hinge.angle;
		var currentSegment = GetSegment(currentAngle);
		var lastSegment = GetSegment(lastAngle);

		if (currentSegment == lastSegment) return;


		var forward = Mathf.DeltaAngle(lastAngle, currentAngle) > 0;
		Cursor = forward ? Cursor + 1 : Cursor - 1;
		var slotIndex = MathUtils.Mod(forward ? SlotCount - currentSegment : SlotCount - currentSegment - 1, SlotCount);
		var itemIndex = forward ? Cursor : BackCursor;

		XDebug.Log(this, "Segment {0} --> {1}. Spawning Item #{2} in slot {3}", lastSegment, currentSegment, itemIndex, slotIndex);
		m_ItemSlots[slotIndex].Spawn(m_ItemList[itemIndex]);
	}

	private int GetSegment(float hingeAngle) {
		var segmentAngle = 360f / SlotCount;
		var segment = MathUtils.Mod(Mathf.FloorToInt(hingeAngle / segmentAngle), SlotCount);
		return segment;
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

	private void PopulateItemSlots() {
		m_ItemSlots.ForEach(slot => {
			slot.Spawn(m_ItemList[Cursor]);
			Cursor++;
		});
	}
}