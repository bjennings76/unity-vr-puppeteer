// ---------------------------------------------------------------------
// Copyright (c) 2016 Magic Leap. All Rights Reserved.
// Magic Leap Confidential and Proprietary
// ---------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace Invaders.Editor {
	[InitializeOnLoad]
	public class AutoPlay : ScriptableObject {
		private static AutoPlay m_Instance;

		[SerializeField] private bool m_SerialisedField;
		[SerializeField] private float m_CompilationStartTime;
		[SerializeField] private bool m_AutoPlay;

		[NonSerialized] private bool m_EphemeralField;
		private bool m_IsCompiling;

		private const string kAutoPlayPref = "ScriptReloadTime_AutoPlay";

		static AutoPlay() {
			EditorApplication.update -= OneTimeUpdate;
			EditorApplication.update += OneTimeUpdate;
		}

		private static void OneTimeUpdate() {
			EditorApplication.update -= OneTimeUpdate;
			if (m_Instance == null) {
				m_Instance = FindObjectOfType<AutoPlay>();
				if (m_Instance == null) m_Instance = CreateInstance<AutoPlay>();
			}
		}

		public void OnEnable() {
			hideFlags = HideFlags.HideAndDontSave;
			m_Instance = this;
			m_IsCompiling = false;
			m_AutoPlay = EditorPrefs.GetBool(kAutoPlayPref, false);
		}


		public AutoPlay() {
			EditorApplication.update -= Update;
			EditorApplication.update += Update;
		}

		[PreferenceItem("Autoplay")]
		private static void OnPreferenceGUI() {
			var content = new GUIContent("Restore Play After Recompile", "If the editor starts recompiling while in 'Play' mode, then automatically start 'Play' mode again when the recompile completes.");
			var autoPlay = EditorGUILayout.Toggle(content, m_Instance.m_AutoPlay);
			if (m_Instance.m_AutoPlay != autoPlay) {
				m_Instance.m_AutoPlay = autoPlay;
				EditorPrefs.SetBool(kAutoPlayPref, m_Instance.m_AutoPlay);
			}
		}

		private void Update() {
			CheckForEscape();

			if (m_Instance != this) {
				EditorApplication.update -= Update;
				if (Application.isPlaying) Destroy(this);
				else DestroyImmediate(this);
			}
			else {
				if (m_SerialisedField && !m_EphemeralField)
					if (m_CompilationStartTime > 0.0f) {
						var elapsedTime = Time.realtimeSinceStartup - m_CompilationStartTime;
						if (elapsedTime > 0.0f) Debug.Log(string.Format("Script Reload detected, duration {0} secs", elapsedTime));
						m_CompilationStartTime = float.NegativeInfinity;
					}

				m_SerialisedField = true;
				m_EphemeralField = true;

				if (!m_IsCompiling && EditorApplication.isCompiling) {
					m_IsCompiling = true;
					m_CompilationStartTime = Time.realtimeSinceStartup;
				}

				if (m_IsCompiling && !EditorApplication.isCompiling) {
					m_IsCompiling = false;
					m_CompilationStartTime = float.NegativeInfinity;
				}

				if (EditorApplication.isPlaying && EditorApplication.isCompiling) {
					Debug.LogWarning("Exiting play mode due to script recompilation.");
					EditorApplication.isPlaying = false;
					if (m_AutoPlay) EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
				}
			}
		}

		private static void CheckForEscape() {
			if (Input.GetAxis("Cancel") <= 0) return;
			XDebug.Log("Stopping play mode.");
			EditorApplication.isPlaying = false;
		}
	}
}