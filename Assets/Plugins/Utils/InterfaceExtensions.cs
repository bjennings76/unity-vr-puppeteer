using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
  public static class InterfaceExtensions {
    private static InterfaceType GetInterface<InterfaceType>(Component[] components) where InterfaceType : class {
      if (components != null) {
        foreach (Component component in components) {
          InterfaceType i = component as InterfaceType;
          if (i != null) {
            return i;
          }
        }
      }
      return null;
    }

    private static object GetInterface(Component[] components, Type interfaceType) {
      if (components != null) {
        foreach (Component component in components) {
          if (interfaceType.IsAssignableFrom(component.GetType())) {
            return component;
          }
        }
      }
      return null;
    }

    private static InterfaceType[] GetInterfaces<InterfaceType>(Component[] components) where InterfaceType : class {
      List<InterfaceType> interfaces = new List<InterfaceType>();
      if (components != null) {
        foreach (Component component in components) {
          InterfaceType i = component as InterfaceType;
          if (i != null) {
            interfaces.Add(i);
          }
        }
      }
      return interfaces.ToArray();
    }

    private static object[] GetInterfaces(Component[] components, Type interfaceType) {
      List<object> interfaces = new List<object>();
      if (components != null) {
        foreach (Component component in components) {
          if (interfaceType.IsAssignableFrom(component.GetType())) {
            interfaces.Add(component);
          }
        }
      }
      return interfaces.ToArray();
    }


    //------------------------------------------------------------------------------------------
    public static InterfaceType GetInterface<InterfaceType>(this GameObject go) where InterfaceType : class {
      return GetInterface<InterfaceType>(go.GetComponents<Component>());
    }

    public static object GetInterface(this GameObject go, Type interfaceType) {
      return GetInterface(go.GetComponents(typeof(Component)), interfaceType);
    }

    public static InterfaceType[] GetInterfaces<InterfaceType>(this GameObject go) where InterfaceType : class {
      return GetInterfaces<InterfaceType>(go.GetComponents<Component>());
    }

    public static object[] GetInterfaces(this GameObject go, Type interfaceType) {
      return GetInterfaces(go.GetComponents(typeof(Component)), interfaceType);
    }

    //------------------------------------------------------------------------------------------


    public static InterfaceType GetInterfaceInChildren<InterfaceType>(this GameObject go) where InterfaceType : class {
      return GetInterface<InterfaceType>(go.GetComponentsInChildren<Component>());
    }

    public static object GetInterfaceInChildren(this GameObject go, Type interfaceType) {
      return GetInterface(go.GetComponentsInChildren(typeof(Component)), interfaceType);
    }

    public static InterfaceType[] GetInterfacesInChildren<InterfaceType>(this GameObject go) where InterfaceType : class {
      return GetInterfaces<InterfaceType>(go.GetComponentsInChildren<Component>());
    }

    public static object[] GetInterfacesInChildren(this GameObject go, Type interfaceType) {
      return GetInterfaces(go.GetComponentsInChildren(typeof(Component)), interfaceType);
    }

    //------------------------------------------------------------------------------------------
    public static InterfaceType GetInterface<InterfaceType>(this Component component) where InterfaceType : class {
      return GetInterface<InterfaceType>(component.GetComponents<Component>());
    }

    public static object GetInterface(this Component component, Type interfaceType) {
      return GetInterface(component.GetComponents(typeof(Component)), interfaceType);
    }

    public static InterfaceType[] GetInterfaces<InterfaceType>(this Component component) where InterfaceType : class {
      return GetInterfaces<InterfaceType>(component.GetComponents<Component>());
    }

    public static object[] GetInterfaces(this Component component, Type interfaceType) {
      return GetInterfaces(component.GetComponents(typeof(Component)), interfaceType);
    }

    //------------------------------------------------------------------------------------------


    public static InterfaceType GetInterfaceInChildren<InterfaceType>(this Component component) where InterfaceType : class {
      return GetInterface<InterfaceType>(component.GetComponentsInChildren<Component>());
    }

    public static object GetInterfaceInChildren(this Component component, Type interfaceType) {
      return GetInterface(component.GetComponentsInChildren(typeof(Component)), interfaceType);
    }

    public static InterfaceType[] GetInterfacesInChildren<InterfaceType>(this Component component) where InterfaceType : class {
      return GetInterfaces<InterfaceType>(component.GetComponentsInChildren<Component>());
    }

    public static object[] GetInterfacesInChildren(this Component component, Type interfaceType) {
      return GetInterfaces(component.GetComponentsInChildren(typeof(Component)), interfaceType);
    }

    //------------------------------------------------------------------------------------------
  }
}