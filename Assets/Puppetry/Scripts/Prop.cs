using UnityEngine;

public class Prop : MonoBehaviour {
	private bool m_InPreview = true;

	public bool InPreview { get { return m_InPreview; } set { m_InPreview = value; } }
}