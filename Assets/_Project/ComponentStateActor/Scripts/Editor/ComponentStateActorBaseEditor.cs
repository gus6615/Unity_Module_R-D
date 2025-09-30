using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ComponentStateActor.Editor
{
    [CustomEditor(typeof(ComponentStateActorBase), true)]
    [CanEditMultipleObjects]
    public class ComponentStateActorBaseEditor : UnityEditor.Editor
    {
        private SerializedProperty targetProp;
        private SerializedProperty stateDataDictProp;

        private ComponentStateActorBase actor;
        private string newStateKey = "";

        protected virtual void OnEnable()
        {
            actor = (ComponentStateActorBase)target;
            targetProp = serializedObject.FindProperty("target");
            stateDataDictProp = serializedObject.FindProperty("stateDataDict");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTargetField();
            EditorGUILayout.PropertyField(stateDataDictProp, true);

            EditorGUILayout.Space();
            DrawWarnings();

            EditorGUILayout.Space();
            DrawDuplicateKeyWarning();

            EditorGUILayout.Space();
            DrawCurrentStateInfo();

            EditorGUILayout.Space();
            DrawStateDropdownAndApply();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawTargetField()
        {
            EditorGUILayout.PropertyField(targetProp);
        }

        private void DrawWarnings()
        {
            if (targetProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "Target이 설정되지 않았습니다. 상태에 따라 변경할 타겟을 지정하세요.",
                    MessageType.Warning);
            }
        }

        private void DrawDuplicateKeyWarning()
        {
            var keys = actor.StateKeys;
            if (keys == null || keys.Count == 0) return;

            var duplicate = keys
                .GroupBy(k => k)
                .FirstOrDefault(g => !string.IsNullOrEmpty(g.Key) && g.Count() > 1);

            if (duplicate != null)
            {
                EditorGUILayout.HelpBox($"중복되는 Key '{duplicate.Key}' 가 존재합니다.", MessageType.Warning);
            }
        }

        private void DrawCurrentStateInfo()
        {
            var currentStateKey = GetCurrentStateKey(actor);
            var label = string.IsNullOrEmpty(currentStateKey) ? "(선택 없음)" : currentStateKey;
            EditorGUILayout.LabelField("현재 선택된 상태", label);
        }

        private void DrawStateDropdownAndApply()
        {
            var keys = actor.StateKeys;
            if (keys == null || keys.Count == 0)
            {
                EditorGUILayout.HelpBox("stateDataDict에 등록된 Key가 없습니다.", MessageType.Info);
                return;
            }

            var keyArray = new string[keys.Count];
            for (var i = 0; i < keys.Count; i++) keyArray[i] = keys[i];

            var currentKey = GetCurrentStateKey(actor);
            var currentIndex = Mathf.Max(0, Array.IndexOf(keyArray, currentKey));

            EditorGUI.BeginChangeCheck();
            var newIndex = EditorGUILayout.Popup("상태 선택", currentIndex, keyArray);
            if (EditorGUI.EndChangeCheck())
            {
                var selectedKey = keyArray[newIndex];

                // 준비 작업 (필요 시 Setup 호출)
                TryInvokeSetup(actor);

                Undo.RecordObject(actor, "Change Component State");
                actor.ChangeState(selectedKey);
                EditorUtility.SetDirty(actor);
            }
        }

        private static string GetCurrentStateKey(ComponentStateActorBase instance)
        {
            if (instance == null) return string.Empty;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var field = typeof(ComponentStateActorBase).GetField("currentStateKey", flags);
            return field != null ? (string)field.GetValue(instance) : string.Empty;
        }

        protected static void TryInvokeSetup(ComponentStateActorBase instance)
        {
            if (instance == null) return;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var method = typeof(ComponentStateActorBase).GetMethod("Setup", flags);
            method?.Invoke(instance, null);
        }
    }
}


