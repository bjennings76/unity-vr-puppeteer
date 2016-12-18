using UnityEngine;
using System.Collections;


// This class is a bit of a weird one.
// XDebug relies on #defines to strip out logging in release builds, there are a few cases 
// where we need to know the state of these #defines from within the XDebug.dll, however 
// because the .dll is pre-compiled we can't easily know what the state of the #defines in Unity
// currently are.  This probe class is an attempt to hack around that.  It lives as a raw script 
// in the Unity project, and will be compiled by Unity using the currently active #defines.  
// XDebug.dll can find this class via reflection and query it to find out which #defines the game
// was compiled with.

public class XDebugHashDefineProbe : XDebug.IHashDefineProbe {

	public bool InfoDefined {
		get {
			bool result = false;
#if XDEBUG
			result = true;
#endif
			return result;
		}
	}

	public bool WarningsDefined {
		get {
			bool result = false;
#if XDEBUG_WARNINGS
			result = true;
#endif
			return result;
		}

	}
	public bool ErrorsDefined {
		get {
			bool result = false;
#if XDEBUG_ERRORS
			result = true;
#endif
			return result;
		}
	}
}