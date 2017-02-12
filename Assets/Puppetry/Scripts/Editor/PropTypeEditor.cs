using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;

[CustomEditor(typeof(PropType))]
public class PropTypeEditor : Editor {
	private DefaultAsset m_FolderReference;
	private DefaultAsset m_LastReference;
	private PropType m_Target;

	private PropType Target { get { return m_Target ? m_Target : (m_Target = target as PropType); } }

	private void OnEnable() { m_LastReference = m_FolderReference; }

	public override void OnInspectorGUI() {
		EditorGUILayout.LabelField("Editor Tools", EditorStyles.boldLabel);
		var newReference = EditorGUILayout.ObjectField("Population Folder", m_FolderReference, typeof(DefaultAsset), false) as DefaultAsset;

		GUI.enabled = newReference && m_LastReference != m_FolderReference;
		var populate = GUILayout.Button("Populate");
		m_FolderReference = newReference;
		GUI.enabled = true;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Object Inspector", EditorStyles.boldLabel);

		base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Props", EditorStyles.boldLabel);
		if (Target.Props.Any(OnPropGUI)) { EditorUtility.SetDirty(Target); }

		if (!populate) return;

		m_LastReference = m_FolderReference;
		var path = AssetDatabase.GetAssetPath(m_FolderReference);
		var assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] {path});
		var assetPaths = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath);
		var gameObjects = assetPaths.Select(p => AssetDatabase.LoadAssetAtPath(p, typeof(GameObject))).OfType<GameObject>();
		var propList = gameObjects.Select(go => new PrefabTweakConfig(go)).ToArray();
		Target.Props = propList;
		EditorUtility.SetDirty(Target);
	}

	private static bool OnPropGUI(PrefabTweakConfig config) {
		GUILayout.Space(5);
		//EditorGUILayout.LabelField(config.Name, EditorStyles.boldLabel);
		var newPrefab = (GameObject) EditorGUILayout.ObjectField(config.Name, config.Prefab, typeof(GameObject), false);

		if (newPrefab == config.Prefab) return false;
		config.Prefab = newPrefab;
		return true;
	}
}