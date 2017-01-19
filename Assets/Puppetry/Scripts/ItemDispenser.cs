using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class ItemDispenser : MonoBehaviour {
	[SerializeField] private HingeJoint m_Hinge;
	[SerializeField] private ItemType m_ItemType;

	public ItemType ItemType {
		private get { return m_ItemType; }
		set {
			m_ItemType = value;
			Repopulate();
		}
	}

	private List<ItemSlot> m_ItemSlots;
	private List<IItemCreator> m_ItemList;
	private float m_LastAngle;
	private int m_Cursor;
	private int m_LastSlotCount;

	private int SlotCount { get { return ItemType.SlotCount; } }

	private int ItemCount { get { return m_ItemList == null || m_ItemList.Count == 0 ? 0 : m_ItemList.Count; } }

	private int Cursor {
		get { return m_Cursor; }
		set {
			m_Cursor = value;
			UpdateCursor();
		}
	}
	private void UpdateCursor() { m_Cursor = ItemCount > 0 ? MathUtils.Mod(m_Cursor, ItemCount) : 0; }

	private int BackCursor { get { return ItemCount > 0 ? MathUtils.Mod(Cursor - SlotCount, ItemCount) : 0; } }

	private void Start() {
		Repopulate();
	}

	private void Update() {
		UpdateRotation();
		CheckForInspectorChanges();
	}

	private void Repopulate() {
		PopulateItemList();
		UpdateCursor();
		PopulateItemSlots();
	}

	private void CheckForInspectorChanges() { CheckForSlotCountChange(); }

	private void CheckForSlotCountChange() {
		if (m_LastSlotCount != SlotCount) {
			m_LastSlotCount = SlotCount;
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

		foreach (var prefab in ItemType.Items) {
			var multiItem = prefab.GetComponent<IMultiItem>();
			if (multiItem != null) list.AddRange(multiItem.GetItemCreators());
			else list.Add(new PrefabItemCreator(prefab));
		}

		m_ItemList = list;
	}

	private void PopulateItemSlots() {
		XDebug.AssertRequiresComponent(ItemType.SlotPrefab, this);
		XDebug.AssertRequiresComponent(m_Hinge, this);

		if (m_ItemSlots != null && m_ItemSlots.Any()) m_ItemSlots.ForEach(s => UnityUtils.Destroy(s.gameObject));

		m_ItemSlots = new List<ItemSlot>();

		var angleStep = 360 / SlotCount;

		for (var i = 0; i < SlotCount; i++) {
			var slot = Instantiate(ItemType.SlotPrefab, m_Hinge.transform, false);
			slot.transform.ResetTransform();
			slot.transform.Rotate(0, angleStep * i, 0);
			m_ItemSlots.Add(slot);
			slot.Spawn(m_ItemList[Cursor]);
			Cursor++;
		}
	}
}