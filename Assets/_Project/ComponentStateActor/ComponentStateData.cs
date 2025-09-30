
using UnityEngine;

namespace ComponentStateActor
{
    [System.Serializable]

    public struct ComponentStateData
    {
        public static ComponentStateData Empty => new ComponentStateData(null, default);
        
        [SerializeField] public Object asset;
        [SerializeField] public Color color;
        
        public ComponentStateData(Object asset, Color color = default)
        {
            this.asset = asset;
            this.color = color;
        }

        public bool HasAsset => asset != null;
        public bool HasColor => color != default;
    }
}