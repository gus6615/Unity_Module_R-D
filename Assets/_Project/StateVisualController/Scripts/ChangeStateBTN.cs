using UnityEngine;
using UnityEngine.UI;
using StateVisualController;

/// <summary>
/// StateVisualController의 상태를 Dia와 Gold 사이에서 무한 반복하는 버튼 스크립트
/// </summary>
public class ChangeStateBTN : MonoBehaviour
{
    [Header("State Settings")]
    [SerializeField] private string[] states = { "Dia", "Gold" };
    [SerializeField] private StateVisualController.StateVisualController stateController;
    
    private int currentStateIndex = 0;
    private Button button;
    private Image image;

    private void Awake()
    {
        // 컴포넌트 참조 가져오기
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        
        // StateVisualController가 할당되지 않았다면 자동으로 찾기
        if (stateController == null)
        {
            stateController = FindObjectOfType<StateVisualController.StateVisualController>();
        }
        
        // 버튼 클릭 이벤트 등록
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void Start()
    {
        // 초기 상태 설정
        if (stateController != null && states.Length > 0)
        {
            stateController.SetState(states[currentStateIndex]);
        }
    }

    /// <summary>
    /// 버튼 클릭 시 호출되는 메서드
    /// </summary>
    private void OnButtonClicked()
    {
        if (stateController == null)
        {
            Debug.LogWarning("StateVisualController가 할당되지 않았습니다!");
            return;
        }

        if (states.Length == 0)
        {
            Debug.LogWarning("상태 배열이 비어있습니다!");
            return;
        }

        // 다음 상태로 인덱스 이동 (무한 반복)
        currentStateIndex = (currentStateIndex + 1) % states.Length;
        
        // 상태 변경
        string newState = states[currentStateIndex];
        stateController.SetState(newState);
        
        Debug.Log($"상태가 '{newState}'로 변경되었습니다.");
    }

    /// <summary>
    /// 현재 상태 인덱스를 반환
    /// </summary>
    public int GetCurrentStateIndex()
    {
        return currentStateIndex;
    }

    /// <summary>
    /// 현재 상태 이름을 반환
    /// </summary>
    public string GetCurrentStateName()
    {
        if (states.Length > 0 && currentStateIndex < states.Length)
        {
            return states[currentStateIndex];
        }
        return string.Empty;
    }

    /// <summary>
    /// 특정 상태로 직접 설정
    /// </summary>
    /// <param name="stateName">설정할 상태 이름</param>
    public void SetSpecificState(string stateName)
    {
        if (stateController == null)
        {
            Debug.LogWarning("StateVisualController가 할당되지 않았습니다!");
            return;
        }

        // 상태 배열에서 해당 상태의 인덱스 찾기
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i] == stateName)
            {
                currentStateIndex = i;
                stateController.SetState(stateName);
                Debug.Log($"상태가 '{stateName}'로 직접 설정되었습니다.");
                return;
            }
        }

        Debug.LogWarning($"상태 '{stateName}'를 찾을 수 없습니다!");
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 정리
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}

