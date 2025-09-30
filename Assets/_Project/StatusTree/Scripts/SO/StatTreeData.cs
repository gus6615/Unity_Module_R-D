using System;
using System.Collections.Generic;
using UnityEngine;

namespace Status
{
    [Serializable]
    public class SerializableNode
    {
        public string key;
        public NodeType nodeType;
        public float value;
        public OperatorType operatorType;
        public float minValue = float.MinValue;
        public float maxValue = float.MaxValue;
        public Vector2 position;
        public List<int> childIndices = new List<int>();
        public int parentIndex = -1;
        
        public SerializableNode()
        {
            key = "NewNode";
            nodeType = NodeType.Value;
            value = 0f;
            operatorType = OperatorType.Add;
            position = Vector2.zero;
        }
        
        public SerializableNode(string nodeKey, NodeType type)
        {
            key = nodeKey;
            nodeType = type;
            value = type == NodeType.Value ? 0f : 0f;
            operatorType = OperatorType.Add;
            position = Vector2.zero;
        }
    }
    
    public enum NodeType
    {
        Value,
        Operator
    }
    
    [CreateAssetMenu(fileName = "New Stat Tree", menuName = "Status/Stat Tree Data")]
    public class StatTreeData : ScriptableObject
    {
        [SerializeField] private List<SerializableNode> nodes = new List<SerializableNode>();
        [SerializeField] private int rootIndex = -1;
        [SerializeField] private string treeName = "New Stat Tree";
        
        public List<SerializableNode> Nodes => nodes;
        public int RootIndex => rootIndex;
        public string TreeName => treeName;
        
        public void SetTreeName(string name)
        {
            treeName = name;
        }
        
        public void SetRootIndex(int index)
        {
            rootIndex = index;
        }
        
        public int AddNode(SerializableNode node)
        {
            nodes.Add(node);
            return nodes.Count - 1;
        }
        
        public void RemoveNode(int index)
        {
            if (index < 0 || index >= nodes.Count) return;
            
            // 자식 노드들의 부모 인덱스 업데이트
            var nodeToRemove = nodes[index];
            foreach (var childIndex in nodeToRemove.childIndices)
            {
                if (childIndex >= 0 && childIndex < nodes.Count)
                {
                    nodes[childIndex].parentIndex = -1;
                }
            }
            
            // 부모 노드에서 이 노드를 자식 목록에서 제거
            if (nodeToRemove.parentIndex >= 0 && nodeToRemove.parentIndex < nodes.Count)
            {
                nodes[nodeToRemove.parentIndex].childIndices.Remove(index);
            }
            
            nodes.RemoveAt(index);
            
            // 인덱스 재조정
            UpdateIndicesAfterRemoval(index);
        }
        
        public void ClearNodes()
        {
            nodes.Clear();
            rootIndex = -1;
        }
        
        public void AddChildToNode(int parentIndex, int childIndex)
        {
            if (parentIndex < 0 || parentIndex >= nodes.Count || 
                childIndex < 0 || childIndex >= nodes.Count) return;
                
            if (!nodes[parentIndex].childIndices.Contains(childIndex))
            {
                nodes[parentIndex].childIndices.Add(childIndex);
                nodes[childIndex].parentIndex = parentIndex;
            }
        }
        
        public void RemoveChildFromNode(int parentIndex, int childIndex)
        {
            if (parentIndex < 0 || parentIndex >= nodes.Count) return;
            
            nodes[parentIndex].childIndices.Remove(childIndex);
            if (childIndex >= 0 && childIndex < nodes.Count)
            {
                nodes[childIndex].parentIndex = -1;
            }
        }
        
        private void UpdateIndicesAfterRemoval(int removedIndex)
        {
            // 모든 노드의 인덱스 참조를 업데이트
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                
                // 부모 인덱스 업데이트
                if (node.parentIndex > removedIndex)
                {
                    node.parentIndex--;
                }
                else if (node.parentIndex == removedIndex)
                {
                    node.parentIndex = -1;
                }
                
                // 자식 인덱스들 업데이트
                for (int j = 0; j < node.childIndices.Count; j++)
                {
                    if (node.childIndices[j] > removedIndex)
                    {
                        node.childIndices[j]--;
                    }
                }
                
                // 제거된 인덱스와 같은 자식 인덱스 제거
                node.childIndices.RemoveAll(childIndex => childIndex == removedIndex);
            }
            
            // 루트 인덱스 업데이트
            if (rootIndex > removedIndex)
            {
                rootIndex--;
            }
            else if (rootIndex == removedIndex)
            {
                rootIndex = -1;
            }
        }
        
        public INode BuildRuntimeTree()
        {
            if (rootIndex < 0 || rootIndex >= nodes.Count) return null;
            
            var runtimeNodes = new Dictionary<int, INode>();
            
            // 모든 노드를 런타임 노드로 변환
            for (int i = 0; i < nodes.Count; i++)
            {
                var serializableNode = nodes[i];
                INode runtimeNode;
                
                if (serializableNode.nodeType == NodeType.Value)
                {
                    runtimeNode = new StatValue(serializableNode.key, serializableNode.value);
                }
                else
                {
                    runtimeNode = new StatOperator(serializableNode.key, serializableNode.operatorType);
                }
                
                runtimeNode.SetConstraint(serializableNode.minValue, serializableNode.maxValue);
                runtimeNodes[i] = runtimeNode;
            }
            
            // 부모-자식 관계 설정
            for (int i = 0; i < nodes.Count; i++)
            {
                var serializableNode = nodes[i];
                var runtimeNode = runtimeNodes[i];
                
                foreach (var childIndex in serializableNode.childIndices)
                {
                    if (runtimeNodes.ContainsKey(childIndex))
                    {
                        runtimeNode.AddChild(runtimeNodes[childIndex]);
                    }
                }
            }
            
            return runtimeNodes[rootIndex];
        }
    }
}
