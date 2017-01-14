using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ItemSlot : MonoBehaviour {
	[SerializeField] private Transform m_SpawnPoint;
	[SerializeField] private Text m_Label;

	private GameObject m_Instance;

	public void Spawn(IItemCreator creator) {
		UnityUtils.Destroy(m_Instance);
		m_Instance = creator.Create(prefab => Instantiate(prefab, m_SpawnPoint, false));
		m_Instance.transform.ResetTransform();
		m_Label.text = creator.Name;
	}
}