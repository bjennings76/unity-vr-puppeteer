using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class PropDispenser : MonoBehaviour {
	[SerializeField] private HingeJoint m_Hinge;
	[SerializeField] private PropType m_PropType;
	[SerializeField] private Transform m_SpawnPoint;

	private PropType m_LastPropType;

	public PropType PropType {
		get { return m_PropType; }
		set {
			m_PropType = value;
			RefreshPropCreators();
		}
	}

	public Transform SpawnPoint { get { return m_SpawnPoint; } }

	private List<PropSlot> m_PropSlots;
	private List<IPropCreator> m_PropCreators;
	private float m_LastAngle;
	private int m_Cursor;
	private int m_LastSlotCount;

	private int SlotCount { get { return PropType.SlotCount; } }

	private int CreatorCount { get { return m_PropCreators == null || m_PropCreators.Count == 0 ? 0 : m_PropCreators.Count; } }

	private int Cursor {
		get { return m_Cursor; }
		set {
			m_Cursor = value;
			WrapCursor();
		}
	}
	private void WrapCursor() { m_Cursor = CreatorCount > 0 ? MathUtils.Mod(m_Cursor, CreatorCount) : 0; }

	private int BackCursor { get { return CreatorCount > 0 ? MathUtils.Mod(Cursor - SlotCount, CreatorCount) : 0; } }

	private void Start() {
		RefreshPropCreators();
	}

	private void Update() {
		UpdateRotation();
		CheckForInspectorChanges();
	}

	private void CheckForInspectorChanges() {
		CheckForSlotCountChange();
		CheckForPropTypeChange();
	}

	private void CheckForPropTypeChange() {
		if (m_LastPropType == m_PropType) { return; }
		RefreshPropCreators();
	}

	private void CheckForSlotCountChange() {
		if (m_LastSlotCount == SlotCount) return;
		m_LastSlotCount = SlotCount;
		Cursor = BackCursor + 1;
		PopulatePropSlots();
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
		var propIndex = forward ? Cursor : BackCursor;

		XDebug.Log(this, "Segment {0} --> {1}. Spawning prop #{2} in slot {3}", lastSegment, currentSegment, propIndex, slotIndex);
		m_PropSlots[slotIndex].CreatePreview(m_PropCreators[propIndex]);
	}

	private int GetSegment(float hingeAngle) {
		var segmentAngle = 360f / SlotCount;
		var segment = MathUtils.Mod(Mathf.FloorToInt(hingeAngle / segmentAngle), SlotCount);
		return segment;
	}

	private void RefreshPropCreators() {
		var list = new List<IPropCreator>();
		m_LastPropType = m_PropType;

		foreach (var prefab in PropType.Props) {
			var multiProp = prefab.GetComponent<IMultiProp>();
			if (multiProp != null) list.AddRange(multiProp.GetPropCreators());
			else list.Add(new PrefabPropCreator(prefab));
		}

		m_PropCreators = list;
		WrapCursor();
		PopulatePropSlots();
	}

	private void PopulatePropSlots() {
		XDebug.AssertRequiresComponent(PropType.SlotPrefab, this);
		XDebug.AssertRequiresComponent(m_Hinge, this);

		if (m_PropSlots != null && m_PropSlots.Any()) m_PropSlots.ForEach(s => UnityUtils.Destroy(s.gameObject));

		m_LastSlotCount = SlotCount;
		m_PropSlots = new List<PropSlot>();
		var angleStep = 360 / SlotCount;

		for (var i = 0; i < SlotCount; i++) {
			var slot = Instantiate(PropType.SlotPrefab, m_Hinge.transform, false);
			slot.transform.ResetTransform();
			slot.transform.Rotate(0, angleStep * i, 0);
			m_PropSlots.Add(slot);
			slot.CreatePreview(m_PropCreators[Cursor]);
			Cursor++;
		}
	}
}