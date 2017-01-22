using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropType))]
public class PropTypeEditor : Editor {
	private DefaultAsset m_FolderReference;
	private PropType m_Target;

	private PropType Target { get { return m_Target ? m_Target : (m_Target = target as PropType); } }

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUILayout.LabelField("Test");
		var newReference = EditorGUILayout.ObjectField("Use Objects from Folder", m_FolderReference, typeof(DefaultAsset), false) as DefaultAsset;

		if (newReference == null || newReference == m_FolderReference) return;

		m_FolderReference = newReference;
		Debug.Log(m_FolderReference.GetType().Name);
		var path = AssetDatabase.GetAssetPath(m_FolderReference);
		var assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] {path});
		var assetPaths = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath);
		var gameObjects = assetPaths.Select(p => AssetDatabase.LoadAssetAtPath(p, typeof(GameObject))).OfType<GameObject>();
		var propList = gameObjects.ToArray();
		Target.Props = propList;
	}
}