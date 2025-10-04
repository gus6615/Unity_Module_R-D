using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using System.Linq;
using AYellowpaper.SerializedCollections.Editor.Data;
using UnityEngine;
using System.Collections;

namespace AYellowpaper.SerializedCollections.Editor
{
    internal static class SCEditorUtility
    {
        public const string EditorPrefsPrefix = "SC_";
        public const bool KeyFlag = true;
        public const bool ValueFlag = false;

        public static bool GetPersistentBool(string path, bool defaultValue)
        {
            return EditorPrefs.GetBool(EditorPrefsPrefix + path, defaultValue);
        }

        public static bool HasKey(string path)
        {
            return EditorPrefs.HasKey( EditorPrefsPrefix + path );
        }

        public static void SetPersistentBool(string path, bool value)
        {
            EditorPrefs.SetBool(EditorPrefsPrefix + path, value);
        }

        public static float CalculateHeight(SerializedProperty property, DisplayType displayType)
        {
            return CalculateHeight(property, displayType == DisplayType.List ? true : false);
        }

        public static float CalculateHeight(SerializedProperty property, bool drawAsList)
        {
            if (drawAsList)
            {
                float height = 0;
                foreach (SerializedProperty child in GetChildren(property))
                    height += EditorGUI.GetPropertyHeight(child, true);
                return height;
            }

            return EditorGUI.GetPropertyHeight(property, true);
        }

        public static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property, bool recursive = false)
        {
            if (!property.hasVisibleChildren)
            {
                yield return property;
                yield break;
            }

            SerializedProperty end = property.GetEndProperty();
            property.NextVisible(true);
            do
            {
                yield return property;
            } while (property.NextVisible(recursive) && !SerializedProperty.EqualContents(property, end));
        }

        public static int GetActualArraySize(SerializedProperty arrayProperty)
        {
            return GetChildren(arrayProperty).Count() - 1;
        }

        public static PropertyData GetPropertyData(SerializedProperty property)
        {
            var data = new PropertyData();
            var json = EditorPrefs.GetString(EditorPrefsPrefix + property.propertyPath, null);
            if (json != null)
                EditorJsonUtility.FromJsonOverwrite(json, data);
            return data;
        }

        public static void SavePropertyData(SerializedProperty property, PropertyData propertyData)
        {
            var json = EditorJsonUtility.ToJson(propertyData);
            EditorPrefs.SetString(EditorPrefsPrefix + property.propertyPath, json);
        }

        public static bool ShouldShowSearch(int pages)
        {
            var settings = EditorUserSettings.Get();
            return settings.AlwaysShowSearch ? true : pages >= settings.PageCountForSearch;
        }

        public static bool HasDrawerForType(Type type)
        {
            var attributeUtilityType = typeof(SerializedProperty).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
            if (attributeUtilityType == null)
                return false;

            // Unity 버전에 따라 GetDrawerTypeForType 시그니처가 다를 수 있어 모든 오버로드를 시도한다.
            var methods = attributeUtilityType
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(m => m.Name == "GetDrawerTypeForType")
                .ToArray();

            if (methods.Length == 0)
                return false;

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                try
                {
                    object result = null;
                    if (parameters.Length == 1)
                    {
                        result = method.Invoke(null, new object[] { type });
                    }
                    else if (parameters.Length == 2)
                    {
                        // 일반적으로 두 번째 인자는 isDecorator(혹은 관련 플래그)로 알려져 있음
                        result = method.Invoke(null, new object[] { type, false });
                    }
                    else
                    {
                        continue;
                    }

                    if (result != null)
                        return true;
                }
                catch (TargetParameterCountException)
                {
                    // 다른 오버로드를 시도한다.
                    continue;
                }
                catch
                {
                    // 무시하고 다음 오버로드 시도
                }
            }
            return false;
        }

        internal static void AddGenericMenuItem(GenericMenu genericMenu, bool isOn, bool isEnabled, GUIContent content, GenericMenu.MenuFunction action)
        {
            if (isEnabled)
                genericMenu.AddItem(content, isOn, action);
            else
                genericMenu.AddDisabledItem(content);
        }

        internal static void AddGenericMenuItem(GenericMenu genericMenu, bool isOn, bool isEnabled, GUIContent content, GenericMenu.MenuFunction2 action, object userData)
        {
            if (isEnabled)
                genericMenu.AddItem(content, isOn, action, userData);
            else
                genericMenu.AddDisabledItem(content);
        }

        internal static bool TryGetTypeFromProperty(SerializedProperty property, out Type type)
        {
            try
            {
                var classType = typeof(EditorGUI).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                var methodInfo = classType.GetMethod("GetFieldInfoFromProperty", BindingFlags.Static | BindingFlags.NonPublic);
                var parameters = new object[] { property, null };
                methodInfo.Invoke(null, parameters);
                type = (Type) parameters[1];
                return true;
            }
            catch
            {
                type = null;
                return false;
            }
        }




        public static object GetPropertyValue(SerializedProperty prop, object target)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    target = GetValue(target, elementName, index);
                }
                else
                {
                    target = GetValue(target, element);
                }
            }
            return target;
        }

        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}