using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Status.Editor
{
    /// <summary>
    /// 노드 생성을 위한 검색 창 제공자
    /// </summary>
    public class StatNodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private StatGraphView _graphView;
        
        public StatNodeSearchWindowProvider(StatGraphView graphView)
        {
            _graphView = graphView;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                
                new SearchTreeGroupEntry(new GUIContent("Value Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Basic Value"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Value" }
                },
                new SearchTreeEntry(new GUIContent("Attack"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Attack" }
                },
                new SearchTreeEntry(new GUIContent("Defense"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Defense" }
                },
                new SearchTreeEntry(new GUIContent("Health"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Health" }
                },
                new SearchTreeEntry(new GUIContent("Mana"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Mana" }
                },
                new SearchTreeEntry(new GUIContent("Speed"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Speed" }
                },
                new SearchTreeEntry(new GUIContent("Level"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Level" }
                },
                new SearchTreeEntry(new GUIContent("Experience"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Value, name = "Experience" }
                },
                
                new SearchTreeGroupEntry(new GUIContent("Operator Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Add (+)"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "Add", operatorType = OperatorType.Add }
                },
                new SearchTreeEntry(new GUIContent("Subtract (-)"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "Subtract", operatorType = OperatorType.Subtract }
                },
                new SearchTreeEntry(new GUIContent("Multiply (×)"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "Multiply", operatorType = OperatorType.Multiply }
                },
                new SearchTreeEntry(new GUIContent("Divide (÷)"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "Divide", operatorType = OperatorType.Divide }
                },
                
                new SearchTreeGroupEntry(new GUIContent("Common Combinations"), 1),
                new SearchTreeEntry(new GUIContent("Buff System"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "BuffAll", operatorType = OperatorType.Subtract }
                },
                new SearchTreeEntry(new GUIContent("Equipment + Base"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "BaseAndEquipment", operatorType = OperatorType.Add }
                },
                new SearchTreeEntry(new GUIContent("Final Multiplier"))
                {
                    level = 2,
                    userData = new NodeCreationData { nodeType = NodeType.Operator, name = "Final", operatorType = OperatorType.Multiply }
                }
            };
            
            return tree;
        }
        
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (SearchTreeEntry.userData is NodeCreationData nodeData)
            {
                var windowRoot = _graphView.contentViewContainer;
                var windowMousePosition = windowRoot.WorldToLocal(context.screenMousePosition);
                
                var node = _graphView.CreateNode(nodeData.nodeType, windowMousePosition);
                if (node != null)
                {
                    node.NodeData.key = nodeData.name;
                    if (nodeData.nodeType == NodeType.Operator)
                    {
                        node.NodeData.operatorType = nodeData.operatorType;
                    }
                    node.UpdateFromData();
                }
                
                return true;
            }
            
            return false;
        }
        
        private struct NodeCreationData
        {
            public NodeType nodeType;
            public string name;
            public OperatorType operatorType;
        }
    }
}
