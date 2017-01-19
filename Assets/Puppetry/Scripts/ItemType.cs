using UnityEngine;

[CreateAssetMenu(fileName = "ItemType")]
public class ItemType : ScriptableObject {
	public Sprite Icon;
	public ItemSlot SlotPrefab;
	public GameObject[] Items;
	[Range(1, 20)] public int SlotCount = 8;
}