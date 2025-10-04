
using UnityEngine;

namespace StateVisualController
{
    [System.Serializable]
    public struct StateVisualData
    {
        public static StateVisualData Empty => new StateVisualData(null, default);
        
        [SerializeField] public Object asset;
        [SerializeField] public Color color;
        
        public StateVisualData(Object asset, Color color = default)
        {
            this.asset = asset;
            this.color = color;
        }

        public bool HasAsset => asset != null;
        public bool HasColor => color != default;
        
        public static bool operator ==(StateVisualData a, StateVisualData b)
        {
            return a.asset == b.asset && a.color == b.color;
        }
        
        public static bool operator !=(StateVisualData a, StateVisualData b)
        {
            return !(a == b);
        }
    }
}