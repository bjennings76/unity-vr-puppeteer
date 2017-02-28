using UnityEngine;
using VRTK;

public class ItemTypeSwitch : VRTK_InteractableObject {
	[Header("Type Options", order = 3)]

	[SerializeField] private PropConfig m_Config;
	[SerializeField] private PropDispenser m_Dispenser;

	public override void OnInteractableObjectUsed(InteractableObjectEventArgs e) {
		base.OnInteractableObjectUsed(e);
		m_Dispenser.PropConfig = m_Config;
	}
}