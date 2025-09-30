
using UnityEngine;

namespace Status
{
    public class StatusTester : MonoBehaviour
    {
        private CharacterController _character;
        
        private void Start()
        {
            Debug.Log("Start StatusTester");
            
            TestCase01();
            TestCase02();
            TestCase03();
            TestCase04();
            TestCase05();
            TestCase06();

            _character = new CharacterController();
            _character.Setup();

            _character.Stat.Print();
            _character.Stat.AddBuff(0.5f);
            _character.Stat.Print();
            _character.Stat.AddLevel(3.0f);
            _character.Stat.Print();
        }

        private void TestCase01()
        {
            var playerStatus = new StatOperator("Final", OperatorType.Add);
            var inGameStatus = new StatValue("InGame", 1f);
            var outGameStatus = new StatValue("OutGame", 3f);
            
            playerStatus.AddChild(inGameStatus);
            playerStatus.AddChild(outGameStatus);

            var result = 4f;
            var isSuccess = Mathf.Approximately(playerStatus.Value, result);

            Debug.Log(isSuccess
                ? "Test Case 1 Success"
                : $"Test Case 1 Fail : Your answer '{playerStatus.Value}' is not equal to {result}");
        }
        
        private void TestCase02()
        {
            var playerStatus = new StatOperator("Final", OperatorType.Multiply);
            var inGameStatus = new StatValue("InGame", 1f);
            var outGameStatus = new StatValue("OutGame", 3f);
            
            playerStatus.AddChild(inGameStatus);
            playerStatus.AddChild(outGameStatus);
            
            var result = 3f;
            var isSuccess = Mathf.Approximately(playerStatus.Value, result);

            Debug.Log(isSuccess
                ? "Test Case 2 Success"
                : $"Test Case 2 Fail : Your answer '{playerStatus.Value}' is not equal to {result}");
        }
        
        private void TestCase03()
        {
            var playerStatus = new StatOperator("Final", OperatorType.Multiply);
            var inGameStatus = new StatValue("InGame", 2f);
            var outGameStatus = new StatOperator("OutGame", OperatorType.Multiply);
            
            playerStatus.AddChild(inGameStatus);
            playerStatus.AddChild(outGameStatus);
            
            var basicStatus = new StatValue("Basic", 100f);
            var levelStatus = new StatValue("Level", 1f);
            
            outGameStatus.AddChild(basicStatus);
            outGameStatus.AddChild(levelStatus);
            
            levelStatus.AddValue(0.05f);
            levelStatus.AddValue(0.05f);
            levelStatus.AddValue(0.05f);

            var result = 230f;
            var isSuccess = Mathf.Approximately(playerStatus.Value, result);

            Debug.Log(isSuccess
                ? "Test Case 3 Success"
                : $"Test Case 3 Fail : Your answer '{playerStatus.Value}' is not equal to {result}");
        }

        private void TestCase04()
        {
            var playerStatus = new StatOperator("Final", OperatorType.Multiply);
            var inGameStatus = new StatOperator("InGame", OperatorType.Add);
            var outGameStatus = new StatOperator("OutGame", OperatorType.Multiply);
            
            playerStatus.AddChild(inGameStatus);
            playerStatus.AddChild(outGameStatus);
            
            var buffStatus = new StatValue("Buff", 0f);
            inGameStatus.AddChild(new StatValue("BuffBasic", 1f));
            inGameStatus.AddChild(buffStatus);
            
            buffStatus.AddValue(0.5f);
            
            var basicAndEquipmentStatus = new StatOperator("basicAndEquipment", OperatorType.Add);
            basicAndEquipmentStatus.AddChild(new StatValue("Basic", 100f));
            basicAndEquipmentStatus.AddChild(new StatValue("Equipment", 20f));
            
            var levelStatus = new StatValue("Level", 1f);
            
            outGameStatus.AddChild(basicAndEquipmentStatus);
            outGameStatus.AddChild(levelStatus);
            
            levelStatus.AddValue(0.1f);
            levelStatus.AddValue(0.1f);
            levelStatus.AddValue(0.1f);

            var result = 234f;
            var isSuccess = Mathf.Approximately(playerStatus.Value, result);

            Debug.Log(isSuccess
                ? "Test Case 4 Success"
                : $"Test Case 4 Fail : Your answer '{playerStatus.Value}' is not equal to {result}");
        }

        private void TestCase05()
        {
            var playerStatus = new StatOperator("Final", OperatorType.Divide);
            
            var playerTotalStatus = new StatOperator("Total", OperatorType.Multiply);
            var weightStatus = new StatValue("Weight", 1f);
            
            playerStatus.AddChild(playerTotalStatus);
            playerStatus.AddChild(weightStatus);
            
            weightStatus.SetValue(2.0f);
            
            var inGameStatus = new StatOperator("InGame", OperatorType.Add);
            var outGameStatus = new StatOperator("OutGame", OperatorType.Multiply);
            
            playerTotalStatus.AddChild(inGameStatus);
            playerTotalStatus.AddChild(outGameStatus);
            
            var buffAllStatus = new StatOperator("BuffAll", OperatorType.Subtract);
            var buffStatus = new StatValue("Buff", 1f);
            var debuffStatus = new StatValue("Debuff", 0f);
            
            inGameStatus.AddChild(buffAllStatus);
            
            buffAllStatus.AddChild(buffStatus);
            buffAllStatus.AddChild(debuffStatus);
            
            buffStatus.AddValue(1f);
            debuffStatus.AddValue(0.5f);
            
            var basicAndEquipmentStatus = new StatOperator("basicAndEquipment", OperatorType.Add);
            basicAndEquipmentStatus.AddChild(new StatValue("Basic", 100f));
            basicAndEquipmentStatus.AddChild(new StatValue("Equipment", 100f));
            
            var levelStatus = new StatValue("Level", 1f);
            
            outGameStatus.AddChild(basicAndEquipmentStatus);
            outGameStatus.AddChild(levelStatus);
            
            levelStatus.AddValue(0.2f);
            levelStatus.SetValue(1.0f);
            levelStatus.AddValue(1.0f);

            var result = 300f;
            var isSuccess = Mathf.Approximately(playerStatus.Value, result);

            Debug.Log(isSuccess
                ? "Test Case 5 Success"
                : $"Test Case 5 Fail : Your answer '{playerStatus.Value}' is not equal to {result}");
        }

        private void TestCase06()
        { 
            var playerStatus = new StatOperator("Final", OperatorType.Divide);
            
            var playerTotalStatus = new StatOperator("Total", OperatorType.Multiply);
            var weightStatus = new StatValue("Weight", 1f);
            
            playerStatus.AddChild(playerTotalStatus);
            playerStatus.AddChild(weightStatus);
            
            weightStatus.SetValue(2.0f);
            
            var inGameStatus = new StatOperator("InGame", OperatorType.Add);
            var outGameStatus = new StatOperator("OutGame", OperatorType.Multiply);
            
            playerTotalStatus.AddChild(inGameStatus);
            playerTotalStatus.AddChild(outGameStatus);
            
            var buffAllStatus = new StatOperator("BuffAll", OperatorType.Subtract);
            var buffStatus = new StatValue("Buff", 1f);
            var debuffStatus = new StatValue("Debuff", 0f);
            
            inGameStatus.AddChild(buffAllStatus);
            
            buffAllStatus.AddChild(buffStatus);
            buffAllStatus.AddChild(debuffStatus);
            
            buffStatus.AddValue(1f);
            debuffStatus.AddValue(0.5f);
            
            var basicAndEquipmentStatus = new StatOperator("basicAndEquipment", OperatorType.Add);
            basicAndEquipmentStatus.AddChild(new StatValue("Basic", 100f));
            basicAndEquipmentStatus.AddChild(new StatValue("Equipment", 100f));
            
            var levelStatus = new StatValue("Level", 1f);
            
            outGameStatus.AddChild(basicAndEquipmentStatus);
            outGameStatus.AddChild(levelStatus);
            
            levelStatus.AddValue(0.2f);
            levelStatus.SetValue(1.0f);
            levelStatus.AddValue(1.0f);

            var inGameNode = playerStatus.FindChild("InGame");
            var outGameNode = playerStatus.FindChild("OutGame");

            if (inGameNode == null || outGameNode == null)
            {
                Debug.Log("Test Case 6 Fail : Cannot find node");
                return;
            }

            var result1 = 1.5f;
            var result2 = 400f;
            var isSuccess1 = Mathf.Approximately(inGameNode.Value, result1);
            var isSuccess2 = Mathf.Approximately(outGameNode.Value, result2);

            Debug.Log(isSuccess1 && isSuccess2
                ? "Test Case 6 Success"
                : $"Test Case 6 Fail : Your answer '{inGameNode.Value}'(answer : {result1}) | '{outGameNode.Value}'(answer : {result2})'");
        }
    }
}