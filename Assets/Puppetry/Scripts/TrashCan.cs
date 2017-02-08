using Utils;
using VRTK;

public class TrashCan : VRTK_SnapDropZone {
	public override void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e) {
		base.OnObjectSnappedToDropZone(e);
		ForceUnsnap();
		UnityUtils.Destroy(e.snappedObject);
	}
}