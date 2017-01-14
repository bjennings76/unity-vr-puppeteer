using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class ItemDispenser : MonoBehaviour {
	[SerializeField] private HingeJoint m_Hinge;
	[SerializeField] private ItemSlot m_SlotPrefab;
	[SerializeField, Range(1, 20)] private int m_SlotCount = 8;
	[SerializeField] private GameObject[] m_ItemPrefabs;
	[SerializeField] private List<ItemSlot> m_ItemSlots;

	private List<IItemCreator> m_ItemList;
	private float m_LastAngle;
	private int m_Cursor;
	private int m_LastSlotCount;

	private int ItemCount { get { return m_ItemList == null || m_ItemList.Count == 0 ? 0 : m_ItemList.Count; } }

	private int Cursor {
		get { return m_Cursor; }
		set {
			m_Cursor = value;
			m_Cursor = ItemCount > 0 ? MathUtils.Mod(m_Cursor, ItemCount) : 0;
		}
	}

	private int BackCursor { get { return ItemCount > 0 ? MathUtils.Mod(Cursor - m_SlotCount, ItemCount) : 0; } }

	private void Start() {
		PopulateItemList();
		PopulateItemSlots();
	}

	private void Update() {
		UpdateRotation();
		CheckForInspectorChanges();
	}

	private void CheckForInspectorChanges() {
		CheckForSlotCountChange();
	}

	private void CheckForSlotCountChange() {
		if (m_LastSlotCount != m_SlotCount) {
			m_LastSlotCount = m_SlotCount;
			Cursor = BackCursor + 1;
			PopulateItemSlots();
		}
	}

	private void UpdateRotation() {
		var lastAngle = m_LastAngle;
		var currentAngle = m_LastAngle = m_Hinge.angle;
		var currentSegment = GetSegment(currentAngle);
		var lastSegment = GetSegment(lastAngle);

		if (currentSegment == lastSegment) return;

		var forward = Mathf.DeltaAngle(lastAngle, currentAngle) > 0;
		Cursor = forward ? Cursor + 1 : Cursor - 1;
		var slotIndex = MathUtils.Mod(forward ? m_SlotCount - currentSegment : m_SlotCount - currentSegment - 1, m_SlotCount);
		var itemIndex = forward ? Cursor : BackCursor;

		XDebug.Log(this, "Segment {0} --> {1}. Spawning Item #{2} in slot {3}", lastSegment, currentSegment, itemIndex, slotIndex);
		m_ItemSlots[slotIndex].Spawn(m_ItemList[itemIndex]);
	}

	private int GetSegment(float hingeAngle) {
		var segmentAngle = 360f / m_SlotCount;
		var segment = MathUtils.Mod(Mathf.FloorToInt(hingeAngle / segmentAngle), m_SlotCount);
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
		XDebug.AssertRequiresComponent(m_SlotPrefab, this);
		XDebug.AssertRequiresComponent(m_Hinge, this);

		if (m_ItemSlots != null && m_ItemSlots.Any()) m_ItemSlots.ForEach(s => UnityUtils.Destroy(s.gameObject));

		m_ItemSlots = new List<ItemSlot>();

		var angleStep = 360 / m_SlotCount;

		for (var i = 0; i < m_SlotCount; i++) {
			var slot = Instantiate(m_SlotPrefab, m_Hinge.transform, false);
			slot.transform.ResetTransform();
			slot.transform.Rotate(0, angleStep * i, 0);
			m_ItemSlots.Add(slot);
			slot.Spawn(m_ItemList[Cursor]);
			Cursor++;
		}
	}
}