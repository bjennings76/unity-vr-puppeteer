using UnityEngine;

[CreateAssetMenu(fileName = "PropType")]
public class PropType : ScriptableObject {
	public Sprite Icon;
	public PropSlot SlotPrefab;
	public float Scale = 1;
	public string TrimRegex;
	[Range(1, 20)] public int SlotCount = 8;
	public PrefabTweakConfig[] Props;
}