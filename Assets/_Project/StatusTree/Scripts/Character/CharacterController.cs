
namespace Status
{
    public class CharacterController
    {
        private CharacterStatTree _stat;
        private CharacterView _view;
        
        public CharacterStatTree Stat => _stat;
        
        public void Setup()
        {
            _stat = new CharacterStatTree();
            _stat.Setup(this);
            //_view.Setup();
        }
    }
}