using UnityEngine;
using Utils;
using VRTK;

public class InteractableSpotlight : VRTK_InteractableObject {
	private Light[] m_Lights;

	private bool m_Using;

	private void Start() { m_Lights = GetComponentsInChildren<Light>(); }

	public override void StartUsing(GameObject previousUsingObject) {
		base.StartUsing(previousUsingObject);
		m_Lights.ForEach(l => l.enabled = !l.enabled);
	}

	public override void StopUsing(GameObject previousUsingObject) {
		base.StopUsing(previousUsingObject);
		m_Lights.ForEach(l => l.enabled = !l.enabled);
	}
}