using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace StateSystem.Editor
{
    [CustomEditor(typeof(StateVisualController))]
    public class StateVisualControllerEditor : UnityEditor.Editor
    {
        private StateVisualController controller;
        private string newStateName = "NewState";
        
        // Target별 그룹화를 위한 변수들
        private Dictionary<GameObject, List<ActorData>> targetToActors;
        private GameObject selectedTarget;
        private Dictionary<GameObject, bool> targetFoldouts = new Dictionary<GameObject, bool>();
        private Dictionary<string, bool> actorFoldouts = new Dictionary<string, bool>();
        
        // Undo 감지를 위한 변수
        private int lastUndoGroup = -1;
        
        // Actor 백업을 위한 변수
        private List<StateVisualController.ActorBackupData> actorBackup;
        private List<StateData> stateBackup;
        
        // Add Actor를 위한 Target 선택 변수
        private GameObject newActorTarget;

        private void OnEnable()
        {
            controller = (StateVisualController)target;
            UpdateTargetGroups();
        }
        
        private void OnDisable()
        {
            // 핸들러가 일반 클래스 인스턴스이므로 자동 메모리 관리됨
            // 명시적 정리 불필요
        }
        
        /// <summary>
        /// StateVisualController가 있는 GameObject와 그 자식 오브젝트들을 가져오는 메서드
        /// </summary>
        /// <returns>사용 가능한 GameObject 목록</returns>
        private List<GameObject> GetAvailableTargets()
        {
            var availableTargets = new List<GameObject>();
            
            if (controller != null)
            {
                // 자기 자신 추가
                availableTargets.Add(controller.gameObject);
                
                // 모든 자식 오브젝트 추가 (재귀적으로)
                AddChildrenRecursively(controller.gameObject.transform, availableTargets);
            }
            
            return availableTargets;
        }
        
        /// <summary>
        /// 재귀적으로 자식 오브젝트들을 목록에 추가하는 메서드
        /// </summary>
        /// <param name="parent">부모 Transform</param>
        /// <param name="targets">추가할 목록</param>
        private void AddChildrenRecursively(Transform parent, List<GameObject> targets)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                targets.Add(child.gameObject);
                
                // 재귀적으로 자식의 자식들도 추가
                AddChildrenRecursively(child, targets);
            }
        }
        
        /// <summary>
        /// 커스텀 ObjectField를 그리는 메서드 (자식 오브젝트만 선택 가능)
        /// </summary>
        /// <param name="label">필드 라벨</param>
        /// <param name="currentValue">현재 선택된 값</param>
        /// <returns>새로 선택된 값</returns>
        private GameObject DrawChildObjectField(string label, GameObject currentValue)
        {
            var availableTargets = GetAvailableTargets();
            
            // 현재 값이 사용 가능한 목록에 있는지 확인
            if (currentValue != null && !availableTargets.Contains(currentValue))
            {
                currentValue = null; // 사용 불가능한 값이면 null로 설정
            }
            
            // Popup으로 표시할 이름들 생성
            var targetNames = new List<string> { "None" };
            var targetObjects = new List<GameObject> { null };
            
            foreach (var target in availableTargets)
            {
                string displayName = target.name;
                if (target == controller.gameObject)
                {
                    displayName += " (Self)";
                }
                else
                {
                    // 계층 구조를 보여주기 위해 들여쓰기 추가
                    int depth = GetDepthFromController(target);
                    displayName = new string(' ', depth * 2) + displayName;
                }
                
                targetNames.Add(displayName);
                targetObjects.Add(target);
            }
            
            // 현재 선택된 인덱스 찾기
            int currentIndex = 0;
            if (currentValue != null)
            {
                currentIndex = targetObjects.IndexOf(currentValue);
                if (currentIndex == -1) currentIndex = 0;
            }
            
            // Popup으로 선택
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup(label, currentIndex, targetNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                return targetObjects[newIndex];
            }
            
            return currentValue;
        }
        
        /// <summary>
        /// StateVisualController로부터의 깊이를 계산하는 메서드
        /// </summary>
        /// <param name="target">대상 GameObject</param>
        /// <returns>깊이 (0 = 자기 자신)</returns>
        private int GetDepthFromController(GameObject target)
        {
            if (target == controller.gameObject) return 0;
            
            int depth = 0;
            Transform current = target.transform;
            
            while (current.parent != null)
            {
                depth++;
                current = current.parent;
                if (current.gameObject == controller.gameObject)
                {
                    return depth;
                }
            }
            
            return depth; // 예상치 못한 경우
        }

        /// <summary>
        /// Actor의 고유 식별자를 생성하는 메서드
        /// </summary>
        /// <param name="actor">대상 Actor</param>
        /// <returns>고유 식별자</returns>
        private string GetActorUniqueId(ActorData actor)
        {
            if (actor == null) return "null";
            
            // Actor의 인덱스와 Target 정보를 조합하여 고유 ID 생성
            int actorIndex = controller.Actors.IndexOf(actor);
            string targetName = actor.Target != null ? actor.Target.name : "null";
            
            return $"actor_{actorIndex}_{targetName}";
        }

        /// <summary>
        /// Target별로 Actor들을 그룹화하는 메서드
        /// </summary>
        private void UpdateTargetGroups()
        {
            targetToActors = new Dictionary<GameObject, List<ActorData>>();
            
            foreach (var actor in controller.Actors)
            {
                if (actor.Target != null)
                {
                    if (!targetToActors.ContainsKey(actor.Target))
                    {
                        targetToActors[actor.Target] = new List<ActorData>();
                    }
                    targetToActors[actor.Target].Add(actor);
                }
            }
            
            // actorFoldouts에서 더 이상 사용되지 않는 Actor들 제거
            var keysToRemove = new List<string>();
            foreach (var key in actorFoldouts.Keys)
            {
                bool found = false;
                foreach (var actor in controller.Actors)
                {
                    if (GetActorUniqueId(actor) == key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    keysToRemove.Add(key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                actorFoldouts.Remove(key);
            }
            
            // 첫 번째 Target을 기본 선택으로 설정
            if (selectedTarget == null && targetToActors.Count > 0)
            {
                selectedTarget = targetToActors.Keys.First();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Undo 감지 및 핸들러 복구
            CheckForUndoAndRestoreHandlers();
            
            // Target 그룹 업데이트
            UpdateTargetGroups();

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
                        
                        // 핸들러가 누락된 경우 복구 시도
                        RestoreMissingHandlers();
                        
                        controller.SetState(state.StateName);
                        EditorUtility.SetDirty(controller);
                    }
                    
                    GUI.backgroundColor = Color.white;
                }
                
                EditorGUILayout.EndHorizontal();
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
                    
                    // Current State가 없을 때만 새로 추가한 State를 Current State로 설정
                    if (string.IsNullOrEmpty(controller.CurrentState))
                    {
                        controller.SetState(newStateName);
                    }
                    
                    newStateName = "NewState";
                    EditorUtility.SetDirty(controller);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 기존 상태 목록
            for (int i = 0; i < controller.States.Count; i++)
            {
                var state = controller.States[i];
                bool isCurrentState = state.StateName == controller.CurrentState;
                
                // 현재 상태는 다른 배경색으로 표시
                if (isCurrentState)
                {
                    GUI.backgroundColor = new Color(0.8f, 1f, 0.8f); // 연한 녹색
                }
                
                EditorGUILayout.BeginHorizontal("box");
                
                // 상태 이름 편집 필드
                EditorGUI.BeginChangeCheck();
                string newStateName = EditorGUILayout.TextField(state.StateName, GUILayout.Width(150));
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrEmpty(newStateName) && newStateName != state.StateName)
                    {
                        // 중복 이름 체크
                        if (!controller.States.Exists(s => s.StateName == newStateName))
                        {
                            Undo.RecordObject(controller, $"Rename State from {state.StateName} to {newStateName}");
                            
                            // 현재 상태가 변경되는 State라면 Current State도 업데이트
                            if (isCurrentState)
                            {
                                controller.CurrentState = newStateName;
                            }
                            
                            // 모든 Actor의 StateDataList에서 해당 상태 이름 업데이트
                            foreach (var actor in controller.Actors)
                            {
                                var stateData = actor.StateDataList.Find(d => d.StateName == state.StateName);
                                if (stateData != null)
                                {
                                    stateData.StateName = newStateName;
                                }
                            }
                            
                            // State 이름 업데이트
                            state.StateName = newStateName;
                            EditorUtility.SetDirty(controller);
                        }
                    }
                }
                
                // 현재 상태 표시
                if (isCurrentState)
                {
                    EditorGUILayout.LabelField("(Current)", EditorStyles.miniLabel, GUILayout.Width(60));
                }
                else
                {
                    EditorGUILayout.LabelField("", GUILayout.Width(60));
                }
                
                // 상태 전환 버튼
                if (!isCurrentState)
                {
                    if (GUILayout.Button("Set", GUILayout.Width(50)))
                    {
                        Undo.RecordObject(controller, $"Set State to {state.StateName}");
                        
                        // 핸들러가 누락된 경우 복구 시도
                        RestoreMissingHandlers();
                        
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
            
            EditorGUILayout.BeginVertical("box");
            
            // Add Actor 섹션 (Target 선택 포함)
            DrawAddActorSection();
            
            EditorGUILayout.Space(10);
            
            // Target이 없는 Actor들 표시
            DrawNoTargetActors();

            EditorGUILayout.Space(10);

            // Target별로 그룹화된 Actor들 표시
            foreach (var targetGroup in targetToActors)
            {
                DrawTargetGroup(targetGroup.Key, targetGroup.Value);
                EditorGUILayout.Space(10);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Add Actor 섹션을 그리는 메서드 (Target 선택 포함)
        /// </summary>
        private void DrawAddActorSection()
        {
            EditorGUILayout.LabelField("Add New Actor", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            
            // Target 선택 필드
            EditorGUI.BeginChangeCheck();
            newActorTarget = DrawChildObjectField("Target", newActorTarget);
            if (EditorGUI.EndChangeCheck())
            {
                // Target이 변경되면 해당 Target 그룹을 펼치도록 설정
                if (newActorTarget != null && targetFoldouts.ContainsKey(newActorTarget))
                {
                    targetFoldouts[newActorTarget] = true;
                }
            }
            
            // Add Actor 버튼
            GUI.enabled = newActorTarget != null;
            if (GUILayout.Button("Add Actor", GUILayout.Width(100), GUILayout.Height(30)))
            {
                Undo.RecordObject(controller, $"Add Actor to {newActorTarget.name}");
                int newActorIndex = controller.AddActor();
                var newActor = controller.Actors[newActorIndex];
                newActor.Target = newActorTarget;
                controller.OnActorTargetChanged(newActor);
                EditorUtility.SetDirty(controller);
                
                // Target 그룹을 펼치도록 설정
                if (targetFoldouts.ContainsKey(newActorTarget))
                {
                    targetFoldouts[newActorTarget] = true;
                }
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            // 도움말 텍스트
            if (newActorTarget == null)
            {
                EditorGUILayout.HelpBox("Select a Target GameObject from the hierarchy below to add an Actor.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"Actor will be added to '{newActorTarget.name}'.", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Target이 없는 Actor들을 표시하는 메서드
        /// </summary>
        private void DrawNoTargetActors()
        {
            var noTargetActors = controller.Actors.Where(actor => actor.Target == null).ToList();
            
            if (noTargetActors.Count > 0)
            {
                EditorGUILayout.LabelField("Actors without Target", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginVertical("box");
                
                foreach (var actor in noTargetActors)
                {
                    DrawNoTargetActor(actor);
                    EditorGUILayout.Space(5);
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        /// <summary>
        /// Target이 없는 개별 Actor를 표시하는 메서드
        /// </summary>
        private void DrawNoTargetActor(ActorData actor)
        {
            int actorIndex = controller.Actors.IndexOf(actor);
            
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            
            // 활성화 토글
            actor.IsEnabled = EditorGUILayout.Toggle(actor.IsEnabled, GUILayout.Width(20));
            
            // Actor 정보
            EditorGUILayout.LabelField($"Actor {actorIndex} (No Target)", EditorStyles.boldLabel);
            
            // 제거 버튼
            GUI.enabled = actor.IsEnabled;
            if (GUILayout.Button("(-)", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Remove Actor", 
                    "Are you sure you want to remove this actor?", "Yes", "No"))
                {
                    Undo.RecordObject(controller, "Remove Actor");
                    controller.RemoveActor(actorIndex);
                    EditorUtility.SetDirty(controller);
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            // Target 할당 필드
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            var newTarget = DrawChildObjectField("Assign Target", actor.Target);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Assign Target to Actor");
                actor.Target = newTarget;
                if (newTarget != null)
                {
                    controller.OnActorTargetChanged(actor);
                }
                EditorUtility.SetDirty(controller);
            }
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 특정 Target의 Actor 그룹을 그리는 메서드
        /// </summary>
        private void DrawTargetGroup(GameObject target, List<ActorData> actors)
        {
            if (target == null) return;
            
            // Target 폴드아웃 상태 초기화
            if (!targetFoldouts.ContainsKey(target))
            {
                targetFoldouts[target] = false;
            }
            
            // Target 헤더
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            
            // 폴드아웃 토글
            targetFoldouts[target] = EditorGUILayout.Foldout(targetFoldouts[target], 
                $"Target: {target.name} ({actors.Count} actors)", true);
            
            // Target 선택 버튼
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeGameObject = target;
                EditorGUIUtility.PingObject(target);
            }
            
            EditorGUILayout.EndHorizontal();

            // Actor들 표시
            if (targetFoldouts[target])
            {
                EditorGUI.indentLevel++;
                
                for (int i = 0; i < actors.Count; i++)
                {
                    DrawActorCompact(actors[i], i);
                    EditorGUILayout.Space(5);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 컴팩트한 Actor 표시 (토글 방식)
        /// </summary>
        private void DrawActorCompact(ActorData actor, int index)
        {
            string actorId = GetActorUniqueId(actor);
            
            // Actor 폴드아웃 상태 초기화
            if (!actorFoldouts.ContainsKey(actorId))
            {
                actorFoldouts[actorId] = false;
            }
            
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            
            // 활성화 토글
            actor.IsEnabled = EditorGUILayout.Toggle(actor.IsEnabled, GUILayout.Width(20));
            
            // Actor 폴드아웃
            string actorLabel = $"Actor {index}";
            if (actor.SelectedComponent != null)
            {
                actorLabel += $" ({actor.SelectedComponent.GetType().Name})";
            }
            if (actor.Handler != null)
            {
                actorLabel += $" [{actor.Handler.GetType().Name}]";
            }
            
            actorFoldouts[actorId] = EditorGUILayout.Foldout(actorFoldouts[actorId], actorLabel, true);
            
            // 제거 버튼
            GUI.enabled = actor.IsEnabled;
            if (GUILayout.Button("(-)", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Remove Actor", 
                    "Are you sure you want to remove this actor?", "Yes", "No"))
                {
                    Undo.RecordObject(controller, "Remove Actor");
                    controller.RemoveActor(controller.Actors.IndexOf(actor));
                    EditorUtility.SetDirty(controller);
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
                }
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            // Actor 상세 정보 표시
            if (actorFoldouts[actorId] && actor.IsEnabled)
            {
                EditorGUI.indentLevel++;
                DrawActorDetails(actor);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Actor의 상세 정보를 그리는 메서드
        /// </summary>
        private void DrawActorDetails(ActorData actor)
        {
            // Target GameObject 선택
            EditorGUI.BeginChangeCheck();
            var newTarget = DrawChildObjectField("Target", actor.Target);
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

            // 상태가 없는 경우 안내 메시지
            if (controller.States.Count == 0)
            {
                EditorGUILayout.HelpBox("No states available. Please add states first using the 'States' section above.", MessageType.Warning);
                return;
            }

            // StateData가 없는 경우 안내 메시지
            if (actor.StateDataList == null || actor.StateDataList.Count == 0)
            {
                EditorGUILayout.HelpBox($"No state data found for this actor. States count: {controller.States.Count}, Handler: {actor.Handler?.GetType().Name}", MessageType.Warning);
                return;
            }

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
        
        /// <summary>
        /// Actor와 상태 백업을 위한 정보를 기록하는 메서드
        /// </summary>
        private void RecordActorAndStateBackup()
        {
            // Actor 백업
            actorBackup = controller.BackupActors();
            
            // 상태 백업
            stateBackup = new List<StateData>(controller.States);
        }
        
        /// <summary>
        /// 백업된 Actor와 상태를 복구하는 메서드
        /// </summary>
        private void RestoreActorAndStateBackup()
        {
            if (actorBackup != null && stateBackup != null)
            {
                // 상태 복구
                controller.States.Clear();
                controller.States.AddRange(stateBackup);
                
                // Actor 복구
                controller.RestoreActors(actorBackup);
            }
        }
        
        /// <summary>
        /// Undo 감지 및 핸들러 복구 메서드
        /// </summary>
        private void CheckForUndoAndRestoreHandlers()
        {
            int currentUndoGroup = Undo.GetCurrentGroup();
            
            // Undo가 발생했는지 확인
            if (lastUndoGroup != -1 && currentUndoGroup != lastUndoGroup)
            {
                // Actor와 상태가 모두 비어있다면 전체 복구 시도
                if (controller.Actors.Count == 0 && controller.States.Count == 0)
                {
                    RestoreActorAndStateBackup();
                }
                else
                {
                    // 핸들러가 누락된 Actor들을 복구
                    RestoreMissingHandlers();
                }
            }
            
            lastUndoGroup = currentUndoGroup;
        }
        
        /// <summary>
        /// 누락된 핸들러들을 복구하는 메서드
        /// </summary>
        private void RestoreMissingHandlers()
        {
            bool restoredAny = false;
            
            foreach (var actor in controller.Actors)
            {
                if (actor.Handler == null && actor.Target != null && !string.IsNullOrEmpty(actor.HandlerType))
                {
                    // 핸들러 인스턴스 생성 (컴포넌트로 추가하지 않음)
                    var handler = HandlerRegistry.CreateHandler(actor.HandlerType);
                    if (handler != null)
                    {
                        actor.Handler = handler;
                        
                        // 핸들러에 타겟 컴포넌트 설정
                        if (actor.SelectedComponent != null)
                        {
                            actor.Handler.SetTargetComponent(actor.SelectedComponent);
                        }
                        
                        restoredAny = true;
                    }
                }
            }
            
            if (restoredAny)
            {
                EditorUtility.SetDirty(controller);
            }
        }
    }
}