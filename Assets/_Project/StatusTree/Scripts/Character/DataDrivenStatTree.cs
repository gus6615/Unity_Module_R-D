using UnityEngine;

namespace Status
{
    /// <summary>
    /// StatTreeData를 사용하여 데이터 기반으로 스탯 트리를 구성하는 클래스
    /// </summary>
    public class DataDrivenStatTree : EntityStatBase<CharacterController>
    {
        [SerializeField] private StatTreeData _treeData;
        
        public StatTreeData TreeData => _treeData;
        
        public void SetTreeData(StatTreeData treeData)
        {
            _treeData = treeData;
            if (_owner != null)
            {
                MakeTree();
            }
        }
        
        protected override void MakeTree()
        {
            if (_treeData == null)
            {
                Debug.LogError("TreeData가 설정되지 않았습니다.");
                return;
            }
            
            _root = _treeData.BuildRuntimeTree();
            
            if (_root == null)
            {
                Debug.LogError("트리 빌드에 실패했습니다. 루트 노드가 설정되었는지 확인하세요.");
            }
        }
        
        /// <summary>
        /// 특정 키를 가진 StatValue 노드에 값을 추가합니다.
        /// </summary>
        public void AddValueToNode(string key, float value)
        {
            if (FindNode(key) is StatValue statValue)
            {
                statValue.AddValue(value);
            }
            else
            {
                Debug.LogWarning($"키 '{key}'를 가진 StatValue 노드를 찾을 수 없습니다.");
            }
        }
        
        /// <summary>
        /// 특정 키를 가진 StatValue 노드의 값을 설정합니다.
        /// </summary>
        public void SetValueToNode(string key, float value)
        {
            if (FindNode(key) is StatValue statValue)
            {
                statValue.SetValue(value);
            }
            else
            {
                Debug.LogWarning($"키 '{key}'를 가진 StatValue 노드를 찾을 수 없습니다.");
            }
        }
        
        /// <summary>
        /// 특정 키를 가진 노드의 현재 값을 반환합니다.
        /// </summary>
        public float GetNodeValue(string key)
        {
            var node = FindNode(key);
            return node?.Value ?? 0f;
        }
        
        /// <summary>
        /// 현재 트리의 모든 노드 정보를 디버그로 출력합니다.
        /// </summary>
        public void PrintTreeDebugInfo()
        {
            if (_root == null)
            {
                Debug.Log("트리가 초기화되지 않았습니다.");
                return;
            }
            
            Debug.Log("=== Stat Tree Debug Info ===");
            Debug.Log($"Root Value: {_root.Value}");
            
            if (_treeData != null)
            {
                Debug.Log($"Tree Name: {_treeData.TreeName}");
                Debug.Log($"Node Count: {_treeData.Nodes.Count}");
                
                for (int i = 0; i < _treeData.Nodes.Count; i++)
                {
                    var node = _treeData.Nodes[i];
                    var runtimeNode = FindNode(node.key);
                    var currentValue = runtimeNode?.Value ?? 0f;
                    
                    Debug.Log($"Node[{i}] {node.key}: {node.nodeType} = {currentValue} " +
                             $"(Children: {node.childIndices.Count})");
                }
            }
        }
    }
}
