using UnityEngine;
using VRTK;

public class ItemTypeSwitch : VRTK_InteractableObject {
	[Header("Type Options", order = 3)]

	[SerializeField] private ItemType m_Type;
	[SerializeField] private ItemDispenser m_Dispenser;

	public override void OnInteractableObjectUsed(InteractableObjectEventArgs e) {
		base.OnInteractableObjectUsed(e);
		m_Dispenser.ItemType = m_Type;
	}
}