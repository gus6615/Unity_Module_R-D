using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Status.Editor
{
    /// <summary>
    /// 스탯 트리를 위한 GraphView
    /// </summary>
    public class StatGraphView : GraphView
    {
        public StatTreeData TreeData { get; private set; }
        
        private Dictionary<int, StatNode> _nodeMap = new Dictionary<int, StatNode>();
        
        public event Action<StatTreeData> OnTreeDataChanged;
        
        public StatGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            
            // 노드 생성 메뉴 추가
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), 
                new StatNodeSearchWindowProvider(this));
                
            // GraphView 변경 사항 감지
            graphViewChanged += OnGraphViewChangedCallback;
        }
        
        public void LoadTreeData(StatTreeData treeData)
        {
            TreeData = treeData;
            ClearGraph();
            
            if (treeData == null) return;
            
            // 노드들 생성
            for (int i = 0; i < treeData.Nodes.Count; i++)
            {
                CreateNodeFromData(i, treeData.Nodes[i]);
            }
            
            // 연결 생성
            CreateConnections();
        }
        
        private void ClearGraph()
        {
            DeleteElements(graphElements.ToList());
            _nodeMap.Clear();
        }
        
        private void CreateNodeFromData(int index, SerializableNode nodeData)
        {
            var node = new StatNode(nodeData);
            node.OnNodeChanged += OnNodeChanged;
            
            _nodeMap[index] = node;
            AddElement(node);
        }
        
        private void CreateConnections()
        {
            if (TreeData == null) return;
            
            for (int i = 0; i < TreeData.Nodes.Count; i++)
            {
                var nodeData = TreeData.Nodes[i];
                var parentNode = _nodeMap[i];
                
                foreach (var childIndex in nodeData.childIndices)
                {
                    if (_nodeMap.ContainsKey(childIndex))
                    {
                        var childNode = _nodeMap[childIndex];
                        var edge = parentNode.OutputPort.ConnectTo(childNode.InputPort);
                        AddElement(edge);
                    }
                }
            }
        }
        
        public StatNode CreateNode(NodeType nodeType, Vector2 position)
        {
            // TreeData가 없으면 임시로 생성
            if (TreeData == null)
            {
                Debug.LogWarning("TreeData가 없습니다. 먼저 'New Tree'를 클릭하여 새 트리를 생성하거나 기존 트리를 로드하세요.");
                return null;
            }
            
            var nodeData = new SerializableNode($"Node_{TreeData.Nodes.Count}", nodeType)
            {
                position = position
            };
            
            var nodeIndex = TreeData.AddNode(nodeData);
            var node = new StatNode(nodeData);
            node.OnNodeChanged += OnNodeChanged;
            
            _nodeMap[nodeIndex] = node;
            AddElement(node);
            
            // 첫 번째 노드면 루트로 설정
            if (TreeData.Nodes.Count == 1)
            {
                TreeData.SetRootIndex(nodeIndex);
                MarkRootNode(node);
            }
            
            OnTreeDataChanged?.Invoke(TreeData);
            return node;
        }
        
        public void DeleteNode(StatNode node)
        {
            if (TreeData == null || node?.NodeData == null) return;
            
            // 노드 인덱스 찾기
            int nodeIndex = -1;
            for (int i = 0; i < TreeData.Nodes.Count; i++)
            {
                if (TreeData.Nodes[i] == node.NodeData)
                {
                    nodeIndex = i;
                    break;
                }
            }
            
            if (nodeIndex >= 0)
            {
                TreeData.RemoveNode(nodeIndex);
                
                // 노드 맵 재구성
                RebuildNodeMap();
                
                OnTreeDataChanged?.Invoke(TreeData);
            }
        }
        
        private void RebuildNodeMap()
        {
            var oldNodeMap = new Dictionary<int, StatNode>(_nodeMap);
            _nodeMap.Clear();
            
            // 기존 노드들을 모두 제거
            foreach (var kvp in oldNodeMap)
            {
                RemoveElement(kvp.Value);
            }
            
            // 새로운 인덱스로 다시 생성
            for (int i = 0; i < TreeData.Nodes.Count; i++)
            {
                CreateNodeFromData(i, TreeData.Nodes[i]);
            }
            
            CreateConnections();
        }
        
        public void SetRootNode(StatNode node)
        {
            if (TreeData == null || node?.NodeData == null) return;
            
            // 노드 인덱스 찾기
            for (int i = 0; i < TreeData.Nodes.Count; i++)
            {
                if (TreeData.Nodes[i] == node.NodeData)
                {
                    TreeData.SetRootIndex(i);
                    break;
                }
            }
            
            UpdateRootNodeVisuals();
            OnTreeDataChanged?.Invoke(TreeData);
        }
        
        private void UpdateRootNodeVisuals()
        {
            foreach (var kvp in _nodeMap)
            {
                var node = kvp.Value;
                var isRoot = kvp.Key == TreeData.RootIndex;
                MarkRootNode(node, isRoot);
            }
        }
        
        private void MarkRootNode(StatNode node, bool isRoot = true)
        {
            if (isRoot)
            {
                node.titleContainer.style.borderTopColor = Color.red;
                node.titleContainer.style.borderTopWidth = 3;
            }
            else
            {
                node.titleContainer.style.borderTopColor = Color.clear;
                node.titleContainer.style.borderTopWidth = 0;
            }
        }
        
        private void OnNodeChanged(StatNode node)
        {
            OnTreeDataChanged?.Invoke(TreeData);
        }
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            
            ports.ForEach(port =>
            {
                if (startPort != port && 
                    startPort.node != port.node && 
                    startPort.direction != port.direction)
                {
                    compatiblePorts.Add(port);
                }
            });
            
            return compatiblePorts;
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            
            if (evt.target is StatNode node)
            {
                evt.menu.AppendAction("Set as Root", (a) => SetRootNode(node));
                evt.menu.AppendAction("Delete", (a) => DeleteNode(node));
            }
            else if (evt.target is GraphView || evt.target is GridBackground)
            {
                var mousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                
                if (TreeData == null)
                {
                    evt.menu.AppendAction("⚠️ Create New Tree First", null, DropdownMenuAction.Status.Disabled);
                    evt.menu.AppendSeparator();
                }
                
                var createValueStatus = TreeData != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
                var createOperatorStatus = TreeData != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
                
                evt.menu.AppendAction("Create Value Node", 
                    (a) => CreateNode(NodeType.Value, mousePosition), 
                    createValueStatus);
                evt.menu.AppendAction("Create Operator Node", 
                    (a) => CreateNode(NodeType.Operator, mousePosition), 
                    createOperatorStatus);
            }
        }
        
        private GraphViewChange OnGraphViewChangedCallback(GraphViewChange graphViewChange)
        {
            UpdateTreeDataConnections();
            return graphViewChange;
        }
        
        private void UpdateTreeDataConnections()
        {
            if (TreeData == null) return;
            
            // 모든 연결 정보 초기화
            foreach (var nodeData in TreeData.Nodes)
            {
                nodeData.childIndices.Clear();
                nodeData.parentIndex = -1;
            }
            
            // 현재 연결된 엣지들을 기반으로 연결 정보 업데이트
            edges.ForEach(edge =>
            {
                var outputNode = edge.output.node as StatNode;
                var inputNode = edge.input.node as StatNode;
                
                if (outputNode != null && inputNode != null)
                {
                    var parentIndex = GetNodeIndex(outputNode);
                    var childIndex = GetNodeIndex(inputNode);
                    
                    if (parentIndex >= 0 && childIndex >= 0)
                    {
                        TreeData.AddChildToNode(parentIndex, childIndex);
                    }
                }
            });
            
            OnTreeDataChanged?.Invoke(TreeData);
        }
        
        private int GetNodeIndex(StatNode node)
        {
            foreach (var kvp in _nodeMap)
            {
                if (kvp.Value == node)
                    return kvp.Key;
            }
            return -1;
        }
        
        public void AutoLayout()
        {
            if (TreeData == null || TreeData.Nodes.Count == 0) return;
            
            var rootIndex = TreeData.RootIndex;
            if (rootIndex < 0) return;
            
            var positions = new Dictionary<int, Vector2>();
            var levels = new Dictionary<int, int>();
            
            // BFS로 레벨 계산
            var queue = new Queue<(int index, int level)>();
            var visited = new HashSet<int>();
            
            queue.Enqueue((rootIndex, 0));
            visited.Add(rootIndex);
            
            while (queue.Count > 0)
            {
                var (currentIndex, level) = queue.Dequeue();
                levels[currentIndex] = level;
                
                var node = TreeData.Nodes[currentIndex];
                foreach (var childIndex in node.childIndices)
                {
                    if (!visited.Contains(childIndex))
                    {
                        visited.Add(childIndex);
                        queue.Enqueue((childIndex, level + 1));
                    }
                }
            }
            
            // 각 레벨별 노드 배치
            var levelNodes = levels.GroupBy(kvp => kvp.Value)
                                  .ToDictionary(g => g.Key, g => g.Select(kvp => kvp.Key).ToList());
            
            const float NODE_SPACING = 200f;
            const float LEVEL_SPACING = 150f;
            
            foreach (var levelGroup in levelNodes)
            {
                var level = levelGroup.Key;
                var nodesInLevel = levelGroup.Value;
                var levelY = level * LEVEL_SPACING;
                
                for (int i = 0; i < nodesInLevel.Count; i++)
                {
                    var nodeIndex = nodesInLevel[i];
                    var levelX = (i - (nodesInLevel.Count - 1) / 2f) * NODE_SPACING;
                    
                    TreeData.Nodes[nodeIndex].position = new Vector2(levelX, levelY);
                    
                    if (_nodeMap.ContainsKey(nodeIndex))
                    {
                        _nodeMap[nodeIndex].SetPosition(new Rect(levelX, levelY, 0, 0));
                    }
                }
            }
            
            OnTreeDataChanged?.Invoke(TreeData);
        }
        
        public void TestTree()
        {
            if (TreeData == null) return;
            
            var runtimeTree = TreeData.BuildRuntimeTree();
            if (runtimeTree != null)
            {
                Debug.Log($"🧪 트리 테스트 결과: {runtimeTree.Value:F2}");
                Debug.Log("✅ 트리가 성공적으로 빌드되었습니다!");
                
                // 각 노드의 현재 값도 출력
                for (int i = 0; i < TreeData.Nodes.Count; i++)
                {
                    var node = TreeData.Nodes[i];
                    var runtimeNode = runtimeTree.FindChild(node.key);
                    if (runtimeNode != null)
                    {
                        Debug.Log($"  📊 {node.key}: {runtimeNode.Value:F2}");
                    }
                }
            }
            else
            {
                Debug.LogError("❌ 트리 빌드에 실패했습니다. 루트 노드를 설정했는지 확인하세요.");
            }
        }
    }
}
