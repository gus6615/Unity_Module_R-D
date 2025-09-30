
using System;
using System.Reflection;
using UnityEngine;

namespace Test.Obfuscation
{
    public class Tester : MonoBehaviour
    {
        private void Awake()
        {
            ShowTypeInfos(typeof(PrivateClass));
            Debug.Log("===============================");
            ShowTypeInfos(typeof(PublicClass));
        }

        private void ShowTypeInfos(Type type)
        {
            Debug.Log("난독화 확인중 ... ");
            Debug.Log($"타입 이름: {type.Name}");
            Debug.Log($"전체 이름: {type.FullName}");
            
            // 메서드 이름 확인
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                Debug.Log($"메서드: {method.Name}");
            }
        
            // 필드 이름 확인
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                Debug.Log($"필드: {field.Name}");
            }
        }
    }
    
    [System.Reflection.Obfuscation(Exclude = false, ApplyToMembers = true)]
    public interface IObfuscation { }

    [System.Reflection.Obfuscation(Exclude = false, ApplyToMembers = true)]
    public class PrivateClass
    {
        private int secretValue = 0000;
        private string secretMessage = "This is secret!";

        public void DoSecretWork()
        {
            Console.WriteLine($"Secret: {secretValue}, Message: {secretMessage}");
            Debug.Log($"Secret: {secretValue}, Message: {secretMessage}");
        }

        private void HiddenMethod()
        {
            Console.WriteLine("Hidden functionality");
            Debug.Log($"Hidden functionality");
        }
    }

    // 난독화 제외
    [System.Reflection.Obfuscation(Exclude = true)]
    public class PublicClass
    {
        public int publicValue = 0000;
        public string publicMessage = "This is public!";

        public void DoPublicWork()
        {
            Console.WriteLine($"Public: {publicValue}, Message: {publicMessage}");
            Debug.Log($"Public: {publicValue}, Message: {publicMessage}");
        }
    }
}