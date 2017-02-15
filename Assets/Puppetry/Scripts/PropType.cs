using UnityEngine;
using Utils;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PropType")]
public class PropType : ScriptableObject {
	public Sprite Icon;
	public PropSlot SlotPrefab;
	public float Scale = 1;
	public string TrimRegex;
	[Range(1, 20)] public int SlotCount = 8;

#if UNITY_EDITOR
	public DefaultAsset PopulationFolder;
#endif

	[HideInInspector] public PrefabTweakConfig[] Props;

	public string GetName(string fileName) { return fileName.ReplaceRegex(TrimRegex, "").ToSpacedName(true, false); }
}