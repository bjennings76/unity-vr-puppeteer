using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;

[CustomEditor(typeof(PropConfig))]
public class PropTypeEditor : Editor {
	private PropConfig m_Target;

	private PropConfig Target { get { return m_Target ? m_Target : (m_Target = target as PropConfig); } }

	public override void OnInspectorGUI() {
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("PropType Config Settings", EditorStyles.boldLabel);

		base.OnInspectorGUI();
		
		EditorGUILayout.Space();

		GUI.enabled = Target.PopulationFolder;
		var populate = GUILayout.Button("Populate");
		GUI.enabled = true;

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Props", EditorStyles.boldLabel);
		if (Target.Props.Any(OnPropGUI)) { EditorUtility.SetDirty(Target); }

		if (Target.Props.Any(p => p.Name.IsNullOrEmpty())) { Target.Props.ForEach(p => p.Name = m_Target.GetName(p.Prefab.name)); }

		if (!populate) return;

		var path = AssetDatabase.GetAssetPath(Target.PopulationFolder);
		var assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] {path});
		var assetPaths = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath);
		var gameObjects = assetPaths.Select(p => AssetDatabase.LoadAssetAtPath(p, typeof(GameObject))).OfType<GameObject>();
		var propList = gameObjects.Select(go => new PrefabTweakConfig(go, m_Target.GetName(go.name))).ToArray();
		Target.Props = propList;
		EditorUtility.SetDirty(Target);
	}

	private static bool OnPropGUI(PrefabTweakConfig config) {
		var newPrefab = (GameObject) EditorGUILayout.ObjectField(config.Name, config.Prefab, typeof(GameObject), false);

		if (newPrefab == config.Prefab) return false;
		config.Prefab = newPrefab;
		return true;
	}
}