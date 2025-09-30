
using UnityEngine;

namespace Status
{
    public class CharacterStatTree : EntityStatBase<CharacterController>
    {
        public float OutGameValue => FindNode("OutGame").Value;
        public float InGameValue => (FindNode("BuffAll").Value - 1f) * _specificValue;
        
        private float _specificValue;
        
        public void AddBuff(float value)
        {
            if (FindNode("Buff") is StatValue buffNode)
                buffNode.AddValue(value);
        }
        
        public void AddDebuff(float value)
        {
            if (FindNode("Debuff") is StatValue debuffNode)
                debuffNode.AddValue(value);
        }
        
        public void AddLevel(float value)
        {
            if (FindNode("Level") is StatValue levelNode)
                levelNode.AddValue(value);
        }

        public void Print()
        {
            Debug.Log("=============== [ Print Status ] ===============");
            Debug.Log("TotalValue: " + Value);
            Debug.Log("OutGameValue: " + OutGameValue);
            Debug.Log("InGameValue: " + InGameValue);
        }

        protected override void SetupInternal()
        {
            _specificValue = 100;
        }

        protected override void MakeTree()
        {
            // ====================================================================
            // _root = InGame + OutGame
            _root = new StatOperator("Total", OperatorType.Multiply);
            var inGameStatus = new StatOperator("InGame", OperatorType.Multiply);
            var outGameStatus = new StatOperator("OutGame", OperatorType.Multiply);
            
            _root.AddChild(inGameStatus);
            _root.AddChild(outGameStatus);
            _root.SetConstraint(0f, float.MaxValue);
            
            // ====================================================================
            // InGame = Basic * (Buff - Debuff)
            
            var buffAllStatus = new StatOperator("BuffAll", OperatorType.Subtract);
            var buffStatus = new StatValue("Buff", 1f);
            var debuffStatus = new StatValue("Debuff", 0f);
            
            inGameStatus.AddChild(buffAllStatus);
            
            buffAllStatus.AddChild(buffStatus);
            buffAllStatus.AddChild(debuffStatus);
            buffAllStatus.SetConstraint(0.1f, float.MaxValue);
            
            // ====================================================================
            // OutGame = (Basic + Equipment) * Level
            
            var basicAndEquipmentStatus = new StatOperator("basicAndEquipment", OperatorType.Add);
            basicAndEquipmentStatus.AddChild(new StatValue("Basic", _specificValue));
            basicAndEquipmentStatus.AddChild(new StatValue("Equipment", 0f));
            
            var levelStatus = new StatValue("Level", 1f);
            
            outGameStatus.AddChild(basicAndEquipmentStatus);
            outGameStatus.AddChild(levelStatus);
            
            // ====================================================================
        }
    }
}