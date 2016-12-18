using System;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable 0649 //Disable the "...  is never assigned to, and will always have its default value" warning
#pragma warning disable 0169 //Disable the "The field '...' is never used" warning

namespace Utils {

	public class InterfaceReference {
		public class XDebug : global::XDebug.Channel<XDebug> {
			public XDebug() : base("InterfaceReference", Colors.Extended.GoldenYellow) {}
		}

		[SerializeField]
		internal Object m_ObjectReference;
	}


	public class InterfaceReference<TInterface> : InterfaceReference
		where TInterface : class {
		// nothing here, we just use this type to access the generic parameter IInterface
	}

	public static class InterfaceReferenceExtensions {
		// We use an extension method to implement InterfaceReference.Value() so that we can deal wit hthe case where 
		// Unity hasn't even bothered to instantiate the InterfaceReference member.

		public static TInterface Value<TInterface>(this InterfaceReference<TInterface> reference)
			where TInterface : class {
			if (reference != null) {
				return reference.m_ObjectReference as TInterface;
			}
			return null;
		}
	}
}
