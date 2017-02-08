using DG.Tweening;
using UnityEngine;
using Utils;
using VRTK;

public class TrashCan : VRTK_SnapDropZone {
	private const float kDownDuration = 0.5f;

	public override void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e) {
		base.OnObjectSnappedToDropZone(e);
		ForceUnsnap();
		var prop = e.snappedObject.GetComponentInParent<Prop>();
		if (prop) {
			prop.transform.DOMove(prop.transform.position + Vector3.down, kDownDuration).OnComplete(() => UnityUtils.Destroy(prop.gameObject));
			prop.transform.DOScale(0, kDownDuration);
		}
	}
}