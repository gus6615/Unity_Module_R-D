using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace Status.Editor
{
    /// <summary>
    /// GraphView에서 사용되는 스탯 노드
    /// </summary>
    public class StatNode : Node
    {
        public SerializableNode NodeData { get; private set; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }
        
        private TextField _keyField;
        private FloatField _valueField;
        private EnumField _operatorField;
        private FloatField _minValueField;
        private FloatField _maxValueField;
        
        public event Action<StatNode> OnNodeChanged;
        
        public StatNode(SerializableNode nodeData)
        {
            NodeData = nodeData;
            
            title = nodeData.key;
            viewDataKey = nodeData.key;
            
            style.left = nodeData.position.x;
            style.top = nodeData.position.y;
            
            CreatePorts();
            CreateUI();
            UpdateNodeColor();
            
            // 위치 변경 이벤트 등록
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        private void CreatePorts()
        {
            // 입력 포트 (부모로부터 받는 포트)
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
            InputPort.portName = "Input";
            inputContainer.Add(InputPort);
            
            // 출력 포트 (자식에게 보내는 포트)
            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            OutputPort.portName = "Output";
            outputContainer.Add(OutputPort);
        }
        
        private void CreateUI()
        {
            var container = new VisualElement();
            
            // 키 필드
            _keyField = new TextField("Key")
            {
                value = NodeData.key
            };
            _keyField.RegisterValueChangedCallback(evt =>
            {
                NodeData.key = evt.newValue;
                title = evt.newValue;
                OnNodeChanged?.Invoke(this);
            });
            container.Add(_keyField);
            
            // 노드 타입에 따른 UI
            if (NodeData.nodeType == NodeType.Value)
            {
                CreateValueNodeUI(container);
            }
            else
            {
                CreateOperatorNodeUI(container);
            }
            
            // 제약 조건 UI
            CreateConstraintUI(container);
            
            extensionContainer.Add(container);
        }
        
        private void CreateValueNodeUI(VisualElement container)
        {
            _valueField = new FloatField("Value")
            {
                value = NodeData.value
            };
            _valueField.RegisterValueChangedCallback(evt =>
            {
                NodeData.value = evt.newValue;
                OnNodeChanged?.Invoke(this);
            });
            container.Add(_valueField);
        }
        
        private void CreateOperatorNodeUI(VisualElement container)
        {
            _operatorField = new EnumField("Operator", NodeData.operatorType);
            _operatorField.RegisterValueChangedCallback(evt =>
            {
                NodeData.operatorType = (OperatorType)evt.newValue;
                OnNodeChanged?.Invoke(this);
            });
            container.Add(_operatorField);
        }
        
        private void CreateConstraintUI(VisualElement container)
        {
            var constraintContainer = new VisualElement();
            constraintContainer.style.marginTop = 5;
            
            var constraintLabel = new Label("Constraints");
            constraintLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            constraintContainer.Add(constraintLabel);
            
            _minValueField = new FloatField("Min")
            {
                value = NodeData.minValue
            };
            _minValueField.RegisterValueChangedCallback(evt =>
            {
                NodeData.minValue = evt.newValue;
                OnNodeChanged?.Invoke(this);
            });
            constraintContainer.Add(_minValueField);
            
            _maxValueField = new FloatField("Max")
            {
                value = NodeData.maxValue
            };
            _maxValueField.RegisterValueChangedCallback(evt =>
            {
                NodeData.maxValue = evt.newValue;
                OnNodeChanged?.Invoke(this);
            });
            constraintContainer.Add(_maxValueField);
            
            container.Add(constraintContainer);
        }
        
        private void UpdateNodeColor()
        {
            var color = NodeData.nodeType == NodeType.Value 
                ? new Color(0.7f, 0.9f, 0.7f) 
                : new Color(0.9f, 0.7f, 0.7f);
                
            titleContainer.style.backgroundColor = color;
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            NodeData.position = new Vector2(style.left.value.value, style.top.value.value);
            OnNodeChanged?.Invoke(this);
        }
        
        public void UpdateFromData()
        {
            title = NodeData.key;
            _keyField.value = NodeData.key;
            
            if (NodeData.nodeType == NodeType.Value)
            {
                if (_valueField != null)
                    _valueField.value = NodeData.value;
            }
            else
            {
                if (_operatorField != null)
                    _operatorField.value = NodeData.operatorType;
            }
            
            if (_minValueField != null)
                _minValueField.value = NodeData.minValue;
            if (_maxValueField != null)
                _maxValueField.value = NodeData.maxValue;
                
            style.left = NodeData.position.x;
            style.top = NodeData.position.y;
            
            UpdateNodeColor();
        }
        
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.position = new Vector2(newPos.x, newPos.y);
            OnNodeChanged?.Invoke(this);
        }
    }
}
