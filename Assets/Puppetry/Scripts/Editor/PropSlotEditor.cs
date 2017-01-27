using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropSlot))]
public class PropSlotEditor : Editor {
	private PropSlot m_Target;

	private PropSlot Target { get { return m_Target ? m_Target : (m_Target = target as PropSlot); } }

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Spawn")) { Target.Create(); }
	}
}