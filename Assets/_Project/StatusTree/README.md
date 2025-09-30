# Status Tree Node Graph Editor

캐릭터의 스탯을 효율적으로 관리하기 위한 **Unity GraphView 기반** 노드 그래프 에디터입니다.

## 🚀 주요 기능

### 1. 전문적인 노드 그래프 에디터
- **Unity GraphView** 기반의 전문적인 노드 에디터
- **Port 기반 연결 시스템** - 드래그로 직관적인 노드 연결
- **Value Node**: 실제 값을 저장하는 리프 노드 (초록색)
- **Operator Node**: 자식 노드들을 계산하는 연산 노드 (빨간색)
- **실시간 노드 편집** - 각 노드 내에서 직접 속성 수정
- **스마트 검색 메뉴** - 우클릭 또는 스페이스바로 노드 생성

### 2. 고급 노드 연결 시스템
- **Port 기반 연결** - 입력/출력 포트로 명확한 데이터 흐름
- **시각적 연결선** - 노드 간 관계를 명확하게 표시
- **자동 연결 검증** - 유효하지 않은 연결 방지
- **다중 자식 지원** - 하나의 부모가 여러 자식을 가질 수 있음

### 3. 스마트 노드 생성
- **검색 기반 노드 생성** - 타입하여 원하는 노드 빠르게 찾기
- **사전 정의된 템플릿** - Attack, Defense, Health 등 게임 스탯 템플릿
- **연산자 노드** - Add, Subtract, Multiply, Divide
- **일반적인 조합** - Buff System, Equipment + Base 등

## 📖 사용 방법

### 1. 에디터 윈도우 열기
```
Unity 메뉴 → Window → Status Tree Editor
```

### 2. 새 트리 생성
1. **"New Tree"** 버튼 클릭
2. 파일명 입력하여 저장
3. 자동으로 GraphView에 로드됨

### 3. 노드 생성 (3가지 방법)
- **우클릭** → 컨텍스트 메뉴에서 "Create Value Node" 또는 "Create Operator Node"
- **스페이스바** → 검색 창에서 원하는 노드 타입하여 선택
- **빈 공간 우클릭** → 빠른 생성 메뉴

### 4. 노드 연결
1. **출력 포트**(노드 오른쪽)에서 드래그 시작
2. **입력 포트**(노드 왼쪽)로 드래그하여 연결
3. 자동으로 부모-자식 관계 설정됨

### 5. 노드 편집
- **노드 내부 필드**에서 직접 값 수정
- **Key**: 노드 식별자
- **Value**: 값 (Value 노드)
- **Operator**: 연산 타입 (Operator 노드)
- **Min/Max**: 제약 조건

### 6. 고급 기능
- **노드 우클릭** → "Set as Root" 또는 "Delete"
- **Auto Layout**: 트리 구조에 따라 자동 정렬
- **Test Tree**: 현재 트리의 계산 결과 및 각 노드 값 출력
- **드래그**: 노드 위치 자유롭게 이동
- **줌/팬**: 마우스 휠로 줌, 중간 버튼으로 팬

## 코드에서 사용하기

### 1. DataDrivenStatTree 사용
```csharp
// StatTreeData를 사용하는 스탯 트리
var statTree = new DataDrivenStatTree();
statTree.SetTreeData(yourTreeData);
statTree.Setup(characterController);

// 값 조작
statTree.AddValueToNode("Attack", 10f);
statTree.SetValueToNode("Defense", 50f);

// 값 조회
float totalStat = statTree.Value;
float attackValue = statTree.GetNodeValue("Attack");
```

### 2. StatTreeTester 컴포넌트
1. GameObject에 `StatTreeTester` 컴포넌트 추가
2. Inspector에서 생성한 StatTreeData 할당
3. 플레이 모드에서 실시간 테스트 가능

## 📁 파일 구조

```
StatusTree/
├── Scripts/
│   ├── Core/
│   │   ├── EntityStatBase.cs          # 기본 스탯 시스템
│   │   └── EntityStatNode.cs          # 노드 인터페이스 및 구현
│   ├── Character/
│   │   ├── CharacterStatTree.cs       # 기존 하드코딩된 트리
│   │   ├── DataDrivenStatTree.cs      # 데이터 기반 트리
│   │   └── StatTreeTester.cs          # 테스트 컴포넌트
│   ├── SO/
│   │   └── StatTreeData.cs            # ScriptableObject 데이터
│   └── Editor/                        # 🆕 GraphView 기반 에디터
│       ├── StatTreeEditor.cs          # 메인 에디터 윈도우
│       ├── Views/
│       │   └── StatGraphView.cs       # GraphView 구현
│       ├── Nodes/
│       │   └── StatNode.cs            # 개별 노드 구현
│       └── Search/
│           └── StatNodeSearchWindowProvider.cs  # 검색 창
└── README.md
```

## 예시 트리 구조

```
Total (Multiply)
├── InGame (Multiply)
│   └── BuffAll (Subtract)
│       ├── Buff (Value: 1.0)
│       └── Debuff (Value: 0.0)
└── OutGame (Multiply)
    ├── BasicAndEquipment (Add)
    │   ├── Basic (Value: 100)
    │   └── Equipment (Value: 0)
    └── Level (Value: 1.0)
```

이 구조에서:
- Total = InGame * OutGame
- InGame = BuffAll
- BuffAll = Buff - Debuff
- OutGame = BasicAndEquipment * Level
- BasicAndEquipment = Basic + Equipment

## ⚠️ 주의사항

1. **루트 노드 설정**: 트리가 작동하려면 반드시 루트 노드를 설정해야 합니다. (빨간색 테두리로 표시)
2. **연산자 제한**: Divide와 Subtract는 정확히 2개의 자식 노드만 가질 수 있습니다.
3. **순환 참조**: GraphView가 자동으로 순환 참조를 방지하지만, 복잡한 구조에서는 주의하세요.
4. **Unity 버전**: Unity 2021.3 이상에서 GraphView가 안정적으로 작동합니다.

## 🚀 새로운 GraphView 에디터의 장점

### 이전 버전 대비 개선사항:
- ✅ **전문적인 UI**: Unity의 Shader Graph, Visual Scripting과 동일한 인터페이스
- ✅ **직관적인 연결**: 포트를 드래그하여 직관적으로 노드 연결
- ✅ **실시간 편집**: 노드 내부에서 직접 값 수정 가능
- ✅ **스마트 검색**: 타입하여 원하는 노드 빠르게 찾기
- ✅ **자동 검증**: 잘못된 연결 시도를 자동으로 방지
- ✅ **확장성**: 새로운 노드 타입을 쉽게 추가 가능

## 🔧 확장 가능성

- **새로운 연산자 타입** - Min, Max, Average 등
- **조건부 노드** - IF/ELSE, Switch 노드
- **수학 함수 노드** - Sin, Cos, Log, Pow 등
- **애니메이션 커브 노드** - 시간 기반 계산
- **외부 데이터 노드** - 다른 ScriptableObject 참조
- **디버그 노드** - 중간 계산 값 시각화
- **그룹 노드** - 복잡한 로직을 하나로 묶기
