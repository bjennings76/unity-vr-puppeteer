using UnityEngine;
using Utils;
using VRTK;

public class PuppetPossession : MonoBehaviour {
	[SerializeField] private float m_PossessionScale = 0.3f;
	[SerializeField] private Rigidbody m_PuppetHead;
	[SerializeField] private Transform m_PuppetHeadPivot;
	[Space, SerializeField] private Rigidbody m_PuppetLeftHand;
	[SerializeField] private Transform m_PuppetLeftHandPivot;
	[Space, SerializeField] private Rigidbody m_PuppetRightHand;
	[SerializeField] private Transform m_PuppetRightHandPivot;

	private FixedJoint m_HeadJoint;
	private FixedJoint m_LeftHandJoint;
	private FixedJoint m_RightHandJoint;

	private VRTK_InteractableObject[] m_Interactables;
	private bool m_Touching;
	private bool m_Possessed;
	private VRTK_ControllerEvents m_RightControllerEvents;
	private VRTK_ControllerEvents m_LeftControllerEvents;
	private UnlockOnGrab m_Locker;
	private Vector3 m_StartPosition;

	private static Transform Boundaries { get { return VRTK_SDKManager.instance.actualBoundaries.transform; } }
	private static Transform Headset { get { return VRTK_SDKManager.instance.actualHeadset.transform; } }

	private void Start() {
		m_Interactables = GetComponentsInChildren<VRTK_InteractableObject>();
		m_Interactables.ForEach(i => {
			i.InteractableObjectTouched += OnTouch;
			i.InteractableObjectUntouched += OnUntouch;
		});

		m_Locker = GetComponentInChildren<UnlockOnGrab>();
		m_RightControllerEvents = VRTK_SDKManager.instance.scriptAliasRightController.GetComponent<VRTK_ControllerEvents>();
		m_LeftControllerEvents = VRTK_SDKManager.instance.scriptAliasLeftController.GetComponent<VRTK_ControllerEvents>();
		m_RightControllerEvents.GripPressed += OnGripPressed;
		m_LeftControllerEvents.GripPressed += OnGripPressed;
	}

	private void OnDestroy() {
		m_RightControllerEvents.GripPressed -= OnGripPressed;
		m_LeftControllerEvents.GripPressed -= OnGripPressed;
	}

	private void OnTouch(object sender, InteractableObjectEventArgs e) { m_Touching = true; }

	private void OnUntouch(object sender, InteractableObjectEventArgs e) { m_Touching = false; }

	private void OnGripPressed(object sender, ControllerInteractionEventArgs e) {
		if (!m_Possessed && !m_Touching) return;

		m_Possessed = !m_Possessed;

		if (m_Locker) m_Locker.SetLock(!m_Possessed);

		if (m_Possessed) {
			m_StartPosition = Boundaries.position;
			Boundaries.localScale = Vector3.one * m_PossessionScale;
			var offset = m_PuppetHead.position - Headset.position;
			Boundaries.position += offset;

			m_HeadJoint = AddJoint(m_PuppetHead, m_PuppetHeadPivot, Headset);
			if (m_LeftControllerEvents.isActiveAndEnabled) m_LeftHandJoint = AddJoint(m_PuppetLeftHand, m_PuppetLeftHandPivot, m_LeftControllerEvents);
			if (m_RightControllerEvents.isActiveAndEnabled) m_RightHandJoint = AddJoint(m_PuppetRightHand, m_PuppetRightHandPivot, m_RightControllerEvents);
		}
		else {
			Boundaries.localScale = Vector3.one;
			Boundaries.position = m_StartPosition;
			UnityUtils.Destroy(m_HeadJoint);
			UnityUtils.Destroy(m_LeftHandJoint);
			UnityUtils.Destroy(m_RightHandJoint);
		}
	}

	private static FixedJoint AddJoint(Rigidbody source, Transform sourcePivot, Component target) {
		sourcePivot = sourcePivot ? sourcePivot : target.transform;
		var targetRigidbody = target.GetOrAddComponent<Rigidbody>();
		targetRigidbody.isKinematic = true;
		var sourceRotationOffset = Quaternion.Inverse(sourcePivot.rotation) * source.transform.rotation;
		source.transform.rotation = target.transform.rotation * sourceRotationOffset;
		var sourcePositionOffset = source.transform.position - sourcePivot.position;
		source.transform.position = target.transform.position + sourcePositionOffset;
		var joint = source.GetOrAddComponent<FixedJoint>();
		joint.connectedBody = targetRigidbody;
		return joint;
	}
}