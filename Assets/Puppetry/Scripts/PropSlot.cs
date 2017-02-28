using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using VRTK;

public class PropSlot : MonoBehaviour {
	[SerializeField] private Transform m_SpawnPoint;
	[SerializeField] private Text m_Label;

	private Prop m_Prop;
	private IPropCreator m_Creator;
	private BoxCollider m_PreviewBoundingBox;
	private VRTK_InteractableObject[] m_Interactables;
	private Transform m_Scaler;
	private PropConfig m_Config;

	private Vector3 PreviewSize { get { return m_PreviewBoundingBox.size; } }
	private Vector3 PreviewScale { get { return m_SpawnPoint.lossyScale; } }

	public void Init(IPropCreator creator, PropConfig config) {
		m_Config = config;
		m_Creator = creator;
		m_PreviewBoundingBox = m_SpawnPoint.GetComponent<BoxCollider>();
		m_PreviewBoundingBox.enabled = false;
		CreatePreview();
	}

	private void CreatePreview() {
		if (m_Prop) UnityUtils.Destroy(m_Prop.gameObject);

		float scale;
		var bounds = m_Creator.GetPreviewBounds();

		switch (m_Config.ScaleStyle) {
			case PropConfig.PreviewScaleStyle.BoundingBox:
				scale = GetPreviewScale(bounds);
				break;
			case PropConfig.PreviewScaleStyle.ActualSize:
				scale = m_Config.Scale;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		m_Scaler = GetPropScaler(scale);

		var position = m_Config.MountOnPivot ? m_SpawnPoint.position : -bounds.center * scale + m_SpawnPoint.position;
		var rotation = m_SpawnPoint.rotation;

		m_Prop = m_Creator.Create(p => {
			var instance = Instantiate(p, m_Scaler, false);
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			return instance.GetOrAddComponent<Prop>();
		});

		m_Label.text = m_Config.GetName(m_Creator.Name);
		m_Prop.StartPreview(m_Config, bounds);
		m_Interactables = m_Prop.GetComponentsInChildren<VRTK_InteractableObject>();
		m_Interactables.ForEach(i => i.InteractableObjectGrabbed += OnInstanceGrabbed);
	}

	private void OnInstanceGrabbed(object sender, InteractableObjectEventArgs e) {
		m_Prop.PreviewGrabbed();
		m_Interactables.ForEach(i => {
			i.InteractableObjectGrabbed -= OnInstanceGrabbed;
			i.InteractableObjectUngrabbed += OnInstanceUngrabbed;
		});
	}

	private void OnInstanceUngrabbed(object sender, InteractableObjectEventArgs e) {
		m_Prop.StopPreview();
		m_Prop = null;
		m_Interactables.ForEach(i => i.InteractableObjectUngrabbed -= OnInstanceUngrabbed);
		UnityUtils.Destroy(m_Scaler.gameObject);
		CreatePreview();
	}

	private float GetPreviewScale(Bounds bounds) {
		var xScale = PreviewSize.x * PreviewScale.x / bounds.size.x;
		var yScale = PreviewSize.y * PreviewScale.y / bounds.size.y;
		var zScale = PreviewSize.z * PreviewScale.z / bounds.size.z;

		var scale = Mathf.Min(xScale, yScale, zScale);
		return scale;
	}

	private Transform GetPropScaler(float scale) {
		var scaler = new GameObject(m_Creator.Name + " Scaler").transform;
		scaler.SetParent(m_SpawnPoint);
		scaler.ResetTransform();
		scaler.localScale = Vector3.one * scale;
		return scaler;
	}
}