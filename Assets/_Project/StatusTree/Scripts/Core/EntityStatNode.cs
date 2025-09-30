
using System.Collections.Generic;
using UnityEngine;

namespace Status
{
    public enum OperatorType { Add, Subtract, Multiply, Divide }

    public interface INode
    {
        string Key { get; set; }
        float Value { get; set; }
        INode Parent { get; set; }
        INode FindChild(string key);
        void SetParent(INode newParent);
        void AddChild(INode newChild);
        void MarkDirty();
        void SetConstraint(float minValue, float maxValue);
    }

    public abstract class StatBase : INode
    {
        public delegate void OnChangedStat(StatBase node, float previousValue, float newValue);
        public event OnChangedStat onChangedStat;
        protected void InvokeChangedStatHandler(StatBase node, float previousValue, float newValue) 
            => onChangedStat?.Invoke(node, previousValue, newValue);

        protected float _minValue = float.MinValue;
        protected float _maxValue = float.MaxValue;
        
        public string Key { get; set; }
        public INode Parent { get; set; }
        public abstract float Value { get; set; }
        public abstract INode FindChild(string key);
        public abstract void SetParent(INode newParent);
        public abstract void AddChild(INode newChild);
        public abstract void MarkDirty();

        public void SetConstraint(float minValue, float maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
    }
    
    public class StatValue : StatBase
    {
        private float _value;

        public StatValue(string key, float value)
        {
            Key  = key;
            _value = value;
        }
        
        public override float Value
        {
            get => _value;
            set
            {
                var previousValue = _value;
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                
                InvokeChangedStatHandler(this, previousValue, value);
            }
        }

        public override INode FindChild(string key) => Key == key ? this : null;
        public override void SetParent(INode newParent) => Parent = newParent;
        public override void AddChild(INode newChild) => Debug.LogError("EntityStatValueNode cannot use AddChild method");
        public override void MarkDirty() => Parent?.MarkDirty();
        
        public void AddValue(float amount)
        {
            Value += amount;
            MarkDirty();
        }

        public void SetValue(float amount)
        {
            Value = amount;
            MarkDirty();
        }
    }
    
    public class StatOperator : StatBase
    {
        private OperatorType _operator;
        private List<INode> _children;

        private float _cachedValue;
        private bool _dirtyFlag;

        public StatOperator(string key, OperatorType @operator)
        {
            Key = key;
            _operator = @operator;
            
            _children = new List<INode>();
            _dirtyFlag = true;
        }
        
        public override float Value
        {
            get
            {
                if (_dirtyFlag)
                {
                    _dirtyFlag = false;
                    CalculateValue();
                }
                
                return _cachedValue;
            }
            set
            {
                var previousValue = _cachedValue;
                _cachedValue = value;
                
                InvokeChangedStatHandler(this, previousValue, value);
            }
        }

        public override INode FindChild(string key)
        {
            if (Key == key) return this;
            
            foreach (var child in _children)
            {
                var node = child.FindChild(key);
                if (node != null) return node;
            }

            return null;
        }

        public override void SetParent(INode newParent) => Parent = newParent;
        public override void AddChild(INode newChild)
        {
            _children.Add(newChild);
            newChild.SetParent(this);
            newChild.MarkDirty();
        }

        public override void MarkDirty()
        {
            _dirtyFlag = true;
            Parent?.MarkDirty();
        }
        
        private void CalculateValue()
        {
            if (_operator is OperatorType.Divide or OperatorType.Subtract && _children.Count != 2)
            {
                Debug.LogError($"{_operator} is not a supported at _children.Count {_children.Count}");
                _cachedValue = 0;
                return;
            }

            if (_children.Count == 0)
            {
                _cachedValue = 0;
                return;
            }
            
            var temp = _children[0].Value;
            for (var i = 1; i < _children.Count; i++)
            {
                var child = _children[i];
                switch (_operator)
                {
                    case OperatorType.Add: temp += child.Value; break;
                    case OperatorType.Subtract: temp -= child.Value; break;
                    case OperatorType.Multiply: temp *= child.Value; break;
                    case OperatorType.Divide: temp /= child.Value; break;
                }
            }
            
            temp = Mathf.Clamp(temp, _minValue, _maxValue);
            _cachedValue = temp;
        }
        
    }
}