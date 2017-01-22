using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelSwapper))]
public class ModelSwapperEditor : Editor {
	private ModelSwapper Swapper { get { return m_Swapper ? m_Swapper : (m_Swapper = target as ModelSwapper); } }
	private ModelSwapper m_Swapper;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Previous")) {
			Swapper.Index--;
			;
		}
		if (GUILayout.Button("Next")) Swapper.Index++;
		GUILayout.EndHorizontal();
	}
}