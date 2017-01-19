using UnityEngine;
using VRTK;

public class ItemTypeSwitch : VRTK_InteractableObject {
	[Header("Type Options", order = 3)]

	[SerializeField] private PropType m_Type;
	[SerializeField] private PropDispenser m_Dispenser;

	public override void OnInteractableObjectUsed(InteractableObjectEventArgs e) {
		base.OnInteractableObjectUsed(e);
		m_Dispenser.PropType = m_Type;
	}
}