using UnityEngine;

[CreateAssetMenu(fileName = "PropType")]
public class PropType : ScriptableObject {
	public Sprite Icon;
	public PropSlot SlotPrefab;
	public GameObject[] Props;
	[Range(1, 20)] public int SlotCount = 8;
}