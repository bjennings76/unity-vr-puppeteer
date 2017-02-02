using UnityEngine;

public class HideInPropPreview : MonoBehaviour {
	public void Hide() { gameObject.SetActive(false); }
	public void Show() { gameObject.SetActive(true); }
}