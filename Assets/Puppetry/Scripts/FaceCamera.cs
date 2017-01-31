using UnityEngine;
using Utils;

public class FaceCamera : MonoBehaviour {
	private void LateUpdate() { transform.LookAt(UnityUtils.Camera.transform); }
}