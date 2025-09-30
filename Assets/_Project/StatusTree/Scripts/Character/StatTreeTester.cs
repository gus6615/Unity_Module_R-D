using UnityEngine;

namespace Status
{
    /// <summary>
    /// StatTreeEditor로 생성한 트리 데이터를 테스트하는 컴포넌트
    /// </summary>
    public class StatTreeTester : MonoBehaviour
    {
        [Header("Tree Data")]
        [SerializeField] private StatTreeData _treeData;
        
        [Header("Test Settings")]
        [SerializeField] private bool _autoTest = true;
        [SerializeField] private float _testInterval = 2f;
        
        private DataDrivenStatTree _statTree;
        private CharacterController _character;
        private float _lastTestTime;
        
        private void Start()
        {
            if (_treeData == null)
            {
                Debug.LogError("TreeData가 설정되지 않았습니다. Inspector에서 StatTreeData를 할당하세요.");
                return;
            }
            
            // 캐릭터 컨트롤러 생성
            _character = new CharacterController();
            _character.Setup();
            
            // 데이터 기반 스탯 트리 생성
            _statTree = new DataDrivenStatTree();
            _statTree.SetTreeData(_treeData);
            _statTree.Setup(_character);
            
            Debug.Log("=== StatTreeTester 시작 ===");
            _statTree.PrintTreeDebugInfo();
        }
        
        private void Update()
        {
            if (_autoTest && _statTree != null && Time.time - _lastTestTime > _testInterval)
            {
                PerformRandomTest();
                _lastTestTime = Time.time;
            }
        }
        
        [ContextMenu("Manual Test")]
        public void PerformRandomTest()
        {
            if (_statTree == null || _treeData == null) return;
            
            // 랜덤하게 Value 노드를 찾아서 값을 변경
            var valueNodes = _treeData.Nodes.FindAll(node => node.nodeType == NodeType.Value);
            if (valueNodes.Count > 0)
            {
                var randomNode = valueNodes[Random.Range(0, valueNodes.Count)];
                var randomValue = Random.Range(-10f, 10f);
                
                Debug.Log($"=== 랜덤 테스트: {randomNode.key}에 {randomValue:F2} 추가 ===");
                _statTree.AddValueToNode(randomNode.key, randomValue);
                _statTree.PrintTreeDebugInfo();
            }
        }
        
        [ContextMenu("Reset All Values")]
        public void ResetAllValues()
        {
            if (_statTree == null || _treeData == null) return;
            
            Debug.Log("=== 모든 값 리셋 ===");
            var valueNodes = _treeData.Nodes.FindAll(node => node.nodeType == NodeType.Value);
            foreach (var node in valueNodes)
            {
                _statTree.SetValueToNode(node.key, node.value);
            }
            _statTree.PrintTreeDebugInfo();
        }
        
        [ContextMenu("Print Current Tree")]
        public void PrintCurrentTree()
        {
            if (_statTree != null)
            {
                _statTree.PrintTreeDebugInfo();
            }
            else
            {
                Debug.Log("StatTree가 초기화되지 않았습니다.");
            }
        }
        
        // GUI에서 실시간으로 값 조작
        private void OnGUI()
        {
            if (_statTree == null || _treeData == null) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 350, 500));
            
            // 헤더
            var headerStyle = new GUIStyle(GUI.skin.label) 
            { 
                fontSize = 18, 
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            GUILayout.Label("🎮 Stat Tree Tester", headerStyle);
            
            // 트리 정보
            GUILayout.Space(5);
            var boldStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            GUILayout.Label($"📊 Tree: {_treeData.TreeName}", boldStyle);
            GUILayout.Label($"🌳 Root Value: {_statTree.Value:F2}", boldStyle);
            GUILayout.Label($"📈 Node Count: {_treeData.Nodes.Count}");
            
            GUILayout.Space(10);
            
            // Value 노드들만 표시하고 조작 가능하게 함
            var valueNodes = _treeData.Nodes.FindAll(node => node.nodeType == NodeType.Value);
            if (valueNodes.Count > 0)
            {
                GUILayout.Label("💎 Value Nodes:", boldStyle);
                
                foreach (var node in valueNodes)
                {
                    GUILayout.BeginHorizontal();
                    
                    // 아이콘과 이름
                    var nodeIcon = GetNodeIcon(node.key);
                    GUILayout.Label($"{nodeIcon} {node.key}:", GUILayout.Width(100));
                    
                    var currentValue = _statTree.GetNodeValue(node.key);
                    var originalValue = node.value;
                    
                    // 현재 값 (색상으로 변화 표시)
                    var valueColor = Mathf.Approximately(currentValue, originalValue) ? Color.white : 
                                   currentValue > originalValue ? Color.green : Color.red;
                    var valueStyle = new GUIStyle(GUI.skin.label) { normal = { textColor = valueColor } };
                    GUILayout.Label($"{currentValue:F2}", valueStyle, GUILayout.Width(60));
                    
                    // 조작 버튼들
                    if (GUILayout.Button("+10", GUILayout.Width(35)))
                        _statTree.AddValueToNode(node.key, 10f);
                    if (GUILayout.Button("+1", GUILayout.Width(30)))
                        _statTree.AddValueToNode(node.key, 1f);
                    if (GUILayout.Button("-1", GUILayout.Width(30)))
                        _statTree.AddValueToNode(node.key, -1f);
                    if (GUILayout.Button("-10", GUILayout.Width(35)))
                        _statTree.AddValueToNode(node.key, -10f);
                    if (GUILayout.Button("↺", GUILayout.Width(25)))
                        _statTree.SetValueToNode(node.key, node.value);
                    
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.Space(15);
            
            // 액션 버튼들
            GUILayout.Label("🎯 Actions:", boldStyle);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🎲 Random Test", GUILayout.Height(30)))
            {
                PerformRandomTest();
            }
            if (GUILayout.Button("🔄 Reset All", GUILayout.Height(30)))
            {
                ResetAllValues();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🖨️ Print Debug", GUILayout.Height(30)))
            {
                PrintCurrentTree();
            }
            if (GUILayout.Button("⚡ Performance Test", GUILayout.Height(30)))
            {
                PerformanceTest();
            }
            GUILayout.EndHorizontal();
            
            // 자동 테스트 설정
            GUILayout.Space(10);
            GUILayout.Label("⚙️ Settings:", boldStyle);
            _autoTest = GUILayout.Toggle(_autoTest, "Auto Test");
            if (_autoTest)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Interval:", GUILayout.Width(60));
                _testInterval = GUILayout.HorizontalSlider(_testInterval, 0.5f, 5f);
                GUILayout.Label($"{_testInterval:F1}s", GUILayout.Width(40));
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndArea();
        }
        
        private string GetNodeIcon(string nodeKey)
        {
            return nodeKey.ToLower() switch
            {
                var key when key.Contains("attack") => "⚔️",
                var key when key.Contains("defense") => "🛡️",
                var key when key.Contains("health") || key.Contains("hp") => "❤️",
                var key when key.Contains("mana") || key.Contains("mp") => "💙",
                var key when key.Contains("speed") => "💨",
                var key when key.Contains("level") => "📊",
                var key when key.Contains("exp") => "⭐",
                var key when key.Contains("buff") => "✨",
                var key when key.Contains("debuff") => "💥",
                var key when key.Contains("basic") => "🔹",
                var key when key.Contains("equipment") => "⚙️",
                _ => "💎"
            };
        }
        
        private void PerformanceTest()
        {
            if (_statTree == null) return;
            
            Debug.Log("🚀 성능 테스트 시작...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            const int iterations = 10000;
            
            for (int i = 0; i < iterations; i++)
            {
                var value = _statTree.Value;
            }
            
            stopwatch.Stop();
            var avgTime = stopwatch.ElapsedMilliseconds / (float)iterations;
            
            Debug.Log($"📈 성능 테스트 결과:");
            Debug.Log($"  - 반복 횟수: {iterations:N0}");
            Debug.Log($"  - 총 시간: {stopwatch.ElapsedMilliseconds}ms");
            Debug.Log($"  - 평균 시간: {avgTime:F4}ms per calculation");
            Debug.Log($"  - 초당 계산 횟수: {1000f / avgTime:F0} calculations/sec");
        }
    }
}
