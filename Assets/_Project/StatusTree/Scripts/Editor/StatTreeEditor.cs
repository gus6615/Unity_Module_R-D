using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Status.Editor
{
    public class StatTreeEditor : EditorWindow
    {
        private StatTreeData _currentTreeData;
        private StatGraphView _graphView;
        private ObjectField _treeDataField;
        
        // 이벤트 선언
        private event System.Action<StatTreeData> TreeDataChanged;
        
        [MenuItem("Window/Status Tree Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<StatTreeEditor>("Stat Tree Editor");
            window.minSize = new Vector2(800, 600);
        }
        
        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            // 툴바 생성
            CreateToolbar(root);
            
            // 그래프 뷰 생성
            _graphView = new StatGraphView();
            _graphView.OnTreeDataChanged += OnTreeDataChanged;
            _graphView.StretchToParentSize();
            
            root.Add(_graphView);
            
            // 현재 트리 데이터 로드
            if (_currentTreeData != null)
            {
                _graphView.LoadTreeData(_currentTreeData);
            }
            else
            {
                // 초기 안내 메시지 추가
                ShowWelcomeMessage();
            }
        }
        
        private void ShowWelcomeMessage()
        {
            // 중앙에 큰 환영 메시지 컨테이너
            var welcomeContainer = new VisualElement()
            {
                style = {
                    position = Position.Absolute,
                    left = 200,
                    top = 150,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
                    borderTopLeftRadius = 10,
                    borderTopRightRadius = 10,
                    borderBottomLeftRadius = 10,
                    borderBottomRightRadius = 10,
                    paddingTop = 30,
                    paddingBottom = 30,
                    paddingLeft = 40,
                    paddingRight = 40,
                    width = 500,
                    height = 300
                }
            };
            
            // 제목
            var titleLabel = new Label("🌳 Status Tree Editor");
            titleLabel.style.fontSize = 24;
            titleLabel.style.color = Color.white;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 20;
            titleLabel.style.alignSelf = Align.Center;
            welcomeContainer.Add(titleLabel);
            
            // 설명
            var descriptionLabel = new Label("시각적 노드 기반 스탯 트리 에디터에 오신 것을 환영합니다!");
            descriptionLabel.style.fontSize = 14;
            descriptionLabel.style.color = new Color(0.8f, 0.8f, 0.8f);
            descriptionLabel.style.marginBottom = 25;
            descriptionLabel.style.whiteSpace = WhiteSpace.Normal;
            descriptionLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            welcomeContainer.Add(descriptionLabel);
            
            // 버튼 컨테이너
            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.justifyContent = Justify.Center;
            buttonContainer.style.marginBottom = 20;
            
            // Create New Tree 버튼
            var createButton = new Button(CreateNewTree);
            createButton.text = "🆕 Create New Tree";
            createButton.style.fontSize = 14;
            createButton.style.paddingTop = 12;
            createButton.style.paddingBottom = 12;
            createButton.style.paddingLeft = 20;
            createButton.style.paddingRight = 20;
            createButton.style.marginRight = 10;
            createButton.style.backgroundColor = new Color(0.2f, 0.7f, 0.2f);
            createButton.style.color = Color.white;
            createButton.style.borderTopLeftRadius = 5;
            createButton.style.borderTopRightRadius = 5;
            createButton.style.borderBottomLeftRadius = 5;
            createButton.style.borderBottomRightRadius = 5;
            buttonContainer.Add(createButton);
            
            // Load Tree 버튼
            var loadButton = new Button(LoadExistingTree);
            loadButton.text = "📂 Load Existing Tree";
            loadButton.style.fontSize = 14;
            loadButton.style.paddingTop = 12;
            loadButton.style.paddingBottom = 12;
            loadButton.style.paddingLeft = 20;
            loadButton.style.paddingRight = 20;
            loadButton.style.backgroundColor = new Color(0.2f, 0.5f, 0.7f);
            loadButton.style.color = Color.white;
            loadButton.style.borderTopLeftRadius = 5;
            loadButton.style.borderTopRightRadius = 5;
            loadButton.style.borderBottomLeftRadius = 5;
            loadButton.style.borderBottomRightRadius = 5;
            buttonContainer.Add(loadButton);
            
            welcomeContainer.Add(buttonContainer);
            
            // 추가 안내
            var instructionLabel = new Label("트리를 생성하거나 로드한 후, 우클릭으로 노드를 추가할 수 있습니다.");
            instructionLabel.style.fontSize = 12;
            instructionLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            instructionLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            instructionLabel.style.whiteSpace = WhiteSpace.Normal;
            welcomeContainer.Add(instructionLabel);
            
            _graphView.Add(welcomeContainer);
            
            // TreeData가 로드되면 메시지 제거
            TreeDataChanged += (data) =>
            {
                if (data != null && welcomeContainer.parent != null)
                {
                    welcomeContainer.RemoveFromHierarchy();
                }
            };
        }
        
        private void CreateToolbar(VisualElement root)
        {
            var toolbar = new Toolbar();
            toolbar.style.height = 35; // 툴바 높이 증가
            
            // 1. Create New Tree 버튼
            var createNewTreeButton = new ToolbarButton(CreateNewTree);
            createNewTreeButton.text = "🆕 Create New Tree";
            createNewTreeButton.style.fontSize = 12;
            createNewTreeButton.style.paddingLeft = 10;
            createNewTreeButton.style.paddingRight = 10;
            createNewTreeButton.style.marginRight = 5;
            toolbar.Add(createNewTreeButton);
            
            // 2. Load Tree 버튼  
            var loadTreeButton = new ToolbarButton(LoadExistingTree);
            loadTreeButton.text = "📂 Load Tree";
            loadTreeButton.style.fontSize = 12;
            loadTreeButton.style.paddingLeft = 10;
            loadTreeButton.style.paddingRight = 10;
            loadTreeButton.style.marginRight = 10;
            toolbar.Add(loadTreeButton);
            
            // 구분선
            var separator = new VisualElement();
            separator.style.width = 2;
            separator.style.backgroundColor = Color.gray;
            separator.style.marginLeft = 5;
            separator.style.marginRight = 10;
            toolbar.Add(separator);
            
            // 현재 트리 표시 (읽기 전용)
            var currentTreeLabel = new Label("Current Tree: None");
            currentTreeLabel.style.fontSize = 11;
            currentTreeLabel.style.color = Color.gray;
            currentTreeLabel.style.alignSelf = Align.Center;
            currentTreeLabel.style.marginRight = 10;
            toolbar.Add(currentTreeLabel);
            
            // TreeData 변경 시 라벨 업데이트
            System.Action updateLabel = () =>
            {
                currentTreeLabel.text = _currentTreeData != null 
                    ? $"Current Tree: {_currentTreeData.TreeName}" 
                    : "Current Tree: None";
                currentTreeLabel.style.color = _currentTreeData != null ? Color.white : Color.gray;
            };
            
            // 초기 업데이트
            updateLabel();
            
            // TreeData 변경 시 라벨 업데이트 등록
            TreeDataChanged += (data) => updateLabel();
            
            // 공간
            toolbar.Add(new ToolbarSpacer());
            
            // 자동 레이아웃 버튼
            var autoLayoutButton = new ToolbarButton(() => _graphView?.AutoLayout());
            autoLayoutButton.text = "📐 Auto Layout";
            autoLayoutButton.style.fontSize = 11;
            toolbar.Add(autoLayoutButton);
            
            // 테스트 버튼
            var testButton = new ToolbarButton(() => _graphView?.TestTree());
            testButton.text = "🧪 Test Tree";
            testButton.style.fontSize = 11;
            toolbar.Add(testButton);
            
            root.Add(toolbar);
        }
        
        private void OnTreeDataFieldChanged(ChangeEvent<Object> evt)
        {
            var newTreeData = evt.newValue as StatTreeData;
            if (newTreeData != _currentTreeData)
            {
                _currentTreeData = newTreeData;
                _graphView?.LoadTreeData(_currentTreeData);
            }
        }
        
        private void OnTreeDataChanged(StatTreeData treeData)
        {
            if (treeData != null)
            {
                EditorUtility.SetDirty(treeData);
            }
            
            // 이벤트 호출
            TreeDataChanged?.Invoke(treeData);
        }
        
        private void CreateNewTree()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create New Stat Tree", "NewStatTree", "asset", "새 스탯 트리를 저장할 위치를 선택하세요");
            if (!string.IsNullOrEmpty(path))
            {
                var newTreeData = CreateInstance<StatTreeData>();
                newTreeData.SetTreeName(System.IO.Path.GetFileNameWithoutExtension(path));
                AssetDatabase.CreateAsset(newTreeData, path);
                AssetDatabase.SaveAssets();
                
                _currentTreeData = newTreeData;
                _graphView?.LoadTreeData(_currentTreeData);
                
                Debug.Log($"✅ 새 스탯 트리가 생성되었습니다: {path}");
            }
        }
        
        private void LoadExistingTree()
        {
            // 기존 StatTreeData 에셋을 찾아서 선택할 수 있는 창 열기
            var path = EditorUtility.OpenFilePanel("Load Existing Stat Tree", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                // 절대 경로를 상대 경로로 변환
                var relativePath = path.Replace(Application.dataPath, "Assets");
                var treeData = AssetDatabase.LoadAssetAtPath<StatTreeData>(relativePath);
                
                if (treeData != null)
                {
                    _currentTreeData = treeData;
                    _graphView?.LoadTreeData(_currentTreeData);
                    Debug.Log($"✅ 스탯 트리가 로드되었습니다: {treeData.TreeName}");
                }
                else
                {
                    Debug.LogError($"❌ StatTreeData를 로드할 수 없습니다: {relativePath}");
                    EditorUtility.DisplayDialog("오류", 
                        "선택한 파일이 StatTreeData가 아니거나 손상되었습니다.\n올바른 StatTreeData 에셋을 선택해주세요.", 
                        "확인");
                }
            }
        }
    }
}