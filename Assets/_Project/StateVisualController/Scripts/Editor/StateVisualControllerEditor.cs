using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace StateSystem.Editor
{
    [CustomEditor(typeof(StateVisualController))]
    public class StateVisualControllerEditor : UnityEditor.Editor
    {
        private StateVisualController controller;
        private string newStateName = "NewState";

        private void OnEnable()
        {
            controller = (StateVisualController)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            DrawCurrentStateSection();
            
            EditorGUILayout.Space(10);
            DrawStateSection();
            
            EditorGUILayout.Space(20);
            DrawActorSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCurrentStateSection()
        {
            EditorGUILayout.LabelField("Current State", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            // 현재 상태 표시
            string currentState = controller.CurrentState;
            if (string.IsNullOrEmpty(currentState))
            {
                EditorGUILayout.LabelField("No state selected", EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.LabelField($"Current: {currentState}", EditorStyles.boldLabel);
            }
            
            EditorGUILayout.Space(5);
            
            // 상태 전환 버튼들
            if (controller.States.Count > 0)
            {
                EditorGUILayout.LabelField("Quick State Transition:", EditorStyles.miniLabel);
                
                EditorGUILayout.BeginHorizontal();
                
                // 각 상태에 대한 전환 버튼
                foreach (var state in controller.States)
                {
                    bool isCurrentState = state.StateName == currentState;
                    
                    // 현재 상태는 다른 색상으로 표시
                    GUI.backgroundColor = isCurrentState ? Color.green : Color.white;
                    
                    if (GUILayout.Button(state.StateName, GUILayout.Height(25)))
                    {
                        Undo.RecordObject(controller, $"Set State to {state.StateName}");
                        controller.SetState(state.StateName);
                        EditorUtility.SetDirty(controller);
                    }
                    
                    GUI.backgroundColor = Color.white;
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5);
                
                // 상태 초기화 버튼
                if (!string.IsNullOrEmpty(currentState))
                {
                    if (GUILayout.Button("Clear Current State", GUILayout.Height(20)))
                    {
                        Undo.RecordObject(controller, "Clear Current State");
                        controller.SetState(string.Empty);
                        EditorUtility.SetDirty(controller);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No states available. Add states first.", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawStateSection()
        {
            EditorGUILayout.LabelField("States", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            // 상태 추가 UI
            EditorGUILayout.BeginHorizontal();
            newStateName = EditorGUILayout.TextField("New State Name", newStateName);
            if (GUILayout.Button("Add State", GUILayout.Width(100)))
            {
                if (!string.IsNullOrEmpty(newStateName))
                {
                    Undo.RecordObject(controller, "Add State");
                    controller.AddState(newStateName);
                    newStateName = "NewState";
                    EditorUtility.SetDirty(controller);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 기존 상태 목록
            for (int i = controller.States.Count - 1; i >= 0; i--)
            {
                var state = controller.States[i];
                bool isCurrentState = state.StateName == controller.CurrentState;
                
                // 현재 상태는 다른 배경색으로 표시
                if (isCurrentState)
                {
                    GUI.backgroundColor = new Color(0.8f, 1f, 0.8f); // 연한 녹색
                }
                
                EditorGUILayout.BeginHorizontal("box");
                
                // 상태 이름과 현재 상태 표시
                string stateLabel = isCurrentState ? $"{state.StateName} (Current)" : state.StateName;
                EditorGUILayout.LabelField(stateLabel, GUILayout.Width(150));
                
                // 상태 전환 버튼
                if (!isCurrentState)
                {
                    if (GUILayout.Button("Set", GUILayout.Width(50)))
                    {
                        Undo.RecordObject(controller, $"Set State to {state.StateName}");
                        controller.SetState(state.StateName);
                        EditorUtility.SetDirty(controller);
                    }
                }
                
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    if (EditorUtility.DisplayDialog("Remove State", 
                        $"Are you sure you want to remove '{state.StateName}'?", "Yes", "No"))
                    {
                        Undo.RecordObject(controller, "Remove State");
                        controller.RemoveState(state.StateName);
                        EditorUtility.SetDirty(controller);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                GUI.backgroundColor = Color.white;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActorSection()
        {
            EditorGUILayout.LabelField("Actors", EditorStyles.boldLabel);
            
            if (GUILayout.Button("(+) Add Actor", GUILayout.Height(30)))
            {
                Undo.RecordObject(controller, "Add Actor");
                controller.AddActor();
                EditorUtility.SetDirty(controller);
            }

            EditorGUILayout.Space(10);

            for (int i = 0; i < controller.Actors.Count; i++)
            {
                DrawActor(controller.Actors[i], i);
                EditorGUILayout.Space(10);
            }
        }

        private void DrawActor(ActorData actor, int index)
        {
            EditorGUILayout.BeginVertical("box");
            
            // Actor 헤더
            EditorGUILayout.BeginHorizontal();
            actor.IsEnabled = EditorGUILayout.Toggle(actor.IsEnabled, GUILayout.Width(20));
            EditorGUILayout.LabelField($"Actor {index}", EditorStyles.boldLabel);
            
            GUI.enabled = actor.IsEnabled;
            if (GUILayout.Button("(-)", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Remove Actor", 
                    "Are you sure you want to remove this actor?", "Yes", "No"))
                {
                    Undo.RecordObject(controller, "Remove Actor");
                    controller.RemoveActor(index);
                    EditorUtility.SetDirty(controller);
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (!actor.IsEnabled)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUI.indentLevel++;

            // Target GameObject 선택
            EditorGUI.BeginChangeCheck();
            var newTarget = (GameObject)EditorGUILayout.ObjectField("Target", actor.Target, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Change Actor Target");
                actor.Target = newTarget;
                controller.OnActorTargetChanged(actor);
                EditorUtility.SetDirty(controller);
            }

            if (actor.Target != null)
            {
                DrawComponentSelection(actor);
                
                if (actor.SelectedComponent != null)
                {
                    DrawHandlerSelection(actor);
                }

                if (actor.Handler != null)
                {
                    DrawStateDataFields(actor);
                }
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawComponentSelection(ActorData actor)
        {
            var components = actor.Target.GetComponents<Component>();
            var componentNames = components.Select(c => c.GetType().Name).ToArray();
            
            int currentIndex = actor.SelectedComponent != null 
                ? Array.IndexOf(components, actor.SelectedComponent) 
                : -1;

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup("Component", currentIndex, componentNames);
            if (EditorGUI.EndChangeCheck() && newIndex >= 0)
            {
                Undo.RecordObject(controller, "Select Component");
                controller.OnComponentSelected(actor, components[newIndex]);
                EditorUtility.SetDirty(controller);
            }
        }

        private void DrawHandlerSelection(ActorData actor)
        {
            var componentType = actor.SelectedComponent.GetType();
            var availableHandlers = HandlerRegistry.GetHandlersForComponent(componentType);

            if (availableHandlers.Count == 0)
            {
                EditorGUILayout.HelpBox("No handlers available for this component type.", MessageType.Info);
                return;
            }

            var handlerNames = availableHandlers.Select(h => h.Name).ToArray();
            int currentIndex = -1;
            
            if (actor.Handler != null)
            {
                currentIndex = availableHandlers.FindIndex(h => h == actor.Handler.GetType());
            }

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup("Handler", currentIndex, handlerNames);
            if (EditorGUI.EndChangeCheck() && newIndex >= 0)
            {
                Undo.RecordObject(controller, "Select Handler");
                controller.OnHandlerSelected(actor, availableHandlers[newIndex]);
                EditorUtility.SetDirty(controller);
            }
        }

        private void DrawStateDataFields(ActorData actor)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("State Data", EditorStyles.boldLabel);

            foreach (var stateData in actor.StateDataList)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(stateData.StateName, EditorStyles.boldLabel);
                
                // Handler 타입에 따라 다른 필드 표시
                DrawHandlerSpecificFields(actor.Handler, stateData);
                
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawHandlerSpecificFields(BaseStateHandler handler, StateHandlerData stateData)
        {
            if (handler is ImageSpriteHandler)
            {
                if (stateData.Data == null)
                {
                    stateData.Data = ScriptableObject.CreateInstance<ImageSpriteData>();
                }
                
                var spriteData = stateData.Data as ImageSpriteData;
                EditorGUI.BeginChangeCheck();
                spriteData.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", spriteData.Sprite, typeof(Sprite), false);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(controller);
                }
            }
            else if (handler is ImageColorHandler)
            {
                if (stateData.Data == null)
                {
                    stateData.Data = ScriptableObject.CreateInstance<ImageColorData>();
                }
                
                var colorData = stateData.Data as ImageColorData;
                EditorGUI.BeginChangeCheck();
                colorData.Color = EditorGUILayout.ColorField("Color", colorData.Color);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(controller);
                }
            }
            else if (handler is TextContentHandler)
            {
                if (stateData.Data == null)
                {
                    stateData.Data = ScriptableObject.CreateInstance<TextContentData>();
                }
                
                var textData = stateData.Data as TextContentData;
                EditorGUI.BeginChangeCheck();
                textData.Content = EditorGUILayout.TextField("Text", textData.Content);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(controller);
                }
            }
            else if (handler is GameObjectActiveHandler)
            {
                if (stateData.Data == null)
                {
                    stateData.Data = ScriptableObject.CreateInstance<GameObjectActiveData>();
                }
                
                var activeData = stateData.Data as GameObjectActiveData;
                EditorGUI.BeginChangeCheck();
                activeData.IsActive = EditorGUILayout.Toggle("Active", activeData.IsActive);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(controller);
                }
            }
        }
    }
}