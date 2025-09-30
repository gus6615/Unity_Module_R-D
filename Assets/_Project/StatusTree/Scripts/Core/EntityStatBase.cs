
namespace Status
{
    public abstract class EntityStatBase<Entity>
    {
        protected Entity _owner;
        protected INode _root;
        
        public INode Root => _root;
        public float Value => _root.Value;
        
        public void Setup(Entity owner)
        {
            _owner = owner;

            SetupInternal();
            MakeTree();
        }
        
        protected virtual INode FindNode(string key) => _root.FindChild(key);
        protected virtual void SetupInternal() { }
        protected abstract void MakeTree();
    }
}
