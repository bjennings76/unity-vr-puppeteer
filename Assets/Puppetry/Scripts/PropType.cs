using PlayFab.ClientModels;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "PropType")]
public class PropType : ScriptableObject {
	public Sprite Icon;
	public PropSlot SlotPrefab;
	public float Scale = 1;
	public string TrimRegex;
	[Range(1, 20)] public int SlotCount = 8;
	[HideInInspector] public PrefabTweakConfig[] Props;

	public string GetName(string fileName) { return fileName.ReplaceRegex(TrimRegex, "").ToSpacedName(true, false); }
}