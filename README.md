# Moonstone

Moonstone은 Unity 프로젝트에서 반복해서 쓰는 에디터 도구와 런타임 기본 구조를 모아 둔 경량 패키지입니다.

- 프로젝트 폴더 구조 템플릿 적용
- Hierarchy 창 표시 커스터마이징
- Canvas 기반 UI 스크립트 생성, 연결, 해제
- 런타임 생명주기, 간단한 DI, 이벤트 버스
- DDD와 애플리케이션 계층에서 쓰는 기본 인터페이스와 타입

## 요구 사항

- Unity 2023.1 이상
- 패키지 이름: `com.materialstone.moonstone`

## 설치

Unity Package Manager에서 다음 방식 중 하나로 추가합니다.

1. 로컬 패키지로 추가: `Add package from disk...`를 선택한 뒤 이 패키지의 `package.json`을 지정합니다.
2. Git 패키지로 추가: 저장소를 Git URL로 관리하는 경우 `Add package from git URL...`에 저장소 URL을 입력합니다.

## 빠른 시작

### 1. 프로젝트 구조 템플릿 적용

Unity 메뉴에서 `Window > Moonstone > Project Setup`을 엽니다.

`Apply` 버튼을 누르면 패키지의 `Templates/ProjectStructure` 내용이 현재 프로젝트의 `Assets` 폴더로 복사됩니다.
기존 파일과 충돌하는 경우 확인 창에서 진행 여부를 선택합니다.

적용 후 기본 구조는 다음과 같습니다.

```txt
Assets/
+-- 01_Scenes/
+-- 02_Scripts/
+-- 03_Prefabs/
+-- 04_Data/
+-- 05_Resources/
    +-- Animations/
    +-- Externals/
    +-- Fonts/
    +-- Materials/
    +-- Shaders/
    +-- Sounds/
    +-- Sprites/
```

### 2. Arc 부트스트래퍼 만들기

Arc는 씬에 배치한 `Bootstrapper` 컴포넌트에서 시작합니다.

```csharp
using System.Threading.Tasks;
using Moonstone.Arc;

public sealed class GameBootstrapper : Bootstrapper
{
    protected override void Configure()
    {
        Container.Register<IMyService>(new MyService());
    }

    protected override void Initialize()
    {
        // 동기 초기화가 필요한 작업을 배치합니다.
    }

    protected override Task StartAsync()
    {
        // 비동기 시작 작업을 배치합니다.
        return Task.CompletedTask;
    }

    protected override void Dispose()
    {
        // 부트스트래퍼가 소유한 리소스를 정리합니다.
    }
}
```

등록한 서비스는 씬 안의 컴포넌트 필드나 프로퍼티에 `[Inject]`를 붙여 주입받을 수 있습니다.

```csharp
using Moonstone.Arc.DependencyInjection;
using UnityEngine;

public sealed class PlayerPresenter : MonoBehaviour
{
    [Inject] private IMyService service;
}
```

`Bootstrapper`는 기본적으로 `IEventBus` 구현체인 `EventBus`를 등록합니다. `Configure`에서 직접 `IEventBus`를 등록하면 기본 등록은 건너뜁니다.

## 에디터 도구

### Project Setup

- 메뉴: `Window > Moonstone > Project Setup`
- 역할: 패키지에 포함된 프로젝트 구조 템플릿을 `Assets` 폴더에 복사합니다.
- 기존 파일과 같은 경로가 있으면 확인 후 덮어씁니다.
- 복사 중 오류가 발생하면 이번 적용에서 만든 파일과 덮어쓴 파일을 되돌립니다.
- `Create Scene Hierarchy` 버튼은 현재 열린 씬에 `Core`, `UI` 루트 구조의 게임 오브젝트 양식을 추가합니다.

### Hierarchy Customization

- 설정 열기: `Tools > Moonstone > Hierarchy Customization > Edit Settings`
- 구분선 토글: `Tools > Moonstone > Hierarchy Customization > Toggle Separator`
- 트리 표시 토글: `Tools > Moonstone > Hierarchy Customization > Toggle Tree`

Hierarchy 창에서 특정 오브젝트를 구분선처럼 보이게 하거나, 부모-자식 관계를 트리 형태로 표시할 수 있습니다.

### UI Script Management

- 메뉴: `Tools > Moonstone > UI Script Management`
- 입력값:
  - `Application Name`: 생성할 스크립트의 루트 네임스페이스
  - `Output Path`: 스크립트를 생성할 경로
  - `Canvas`: 기준이 될 Canvas 오브젝트
- 주요 버튼:
  - `Generate Scripts`: Canvas 자식 구조를 기준으로 스크립트를 생성합니다.
  - `Attach Scripts`: 생성된 컴포넌트를 Canvas 자식 오브젝트에 연결합니다.
  - `Detach Scripts`: 연결된 컴포넌트를 제거합니다.

Canvas 바로 아래의 자식은 화면 단위로, 그 아래 자식은 화면에 속한 View 단위로 취급합니다. 오브젝트 이름은 C# 클래스명으로 사용할 수 있어야 하므로 영문자, 숫자, 언더스코어를 사용하고 숫자로 시작하지 않아야 합니다.

## 런타임 모듈

### Core

생명주기와 표시 상태를 다루는 기본 타입입니다.

- `IInitializable`
- `IAsyncInitializable`
- `ILifecycleDisposable`
- `IVisible`
- `LifecycleBehaviour`
- `LifecycleRunner`
- `View`
- `Visible`

`LifecycleRunner.InitializeHierarchyAsync`는 씬 계층 안의 `IInitializable`, `IAsyncInitializable` 컴포넌트를 한 경로로 초기화합니다. `DisposeHierarchy`는 하위 오브젝트부터 정리합니다.

### Arc

씬 시작, 의존성 주입, 이벤트 전달을 담당하는 런타임 구조입니다.

- `Bootstrapper`
- `Container`
- `InjectAttribute`
- `IResolver`
- `SceneInjector`
- `ServiceResolver`
- `IEventBus`
- `EventBus`
- `EventSubscription`

Arc의 흐름은 다음 순서로 진행됩니다.

1. `Bootstrapper.Start`에서 부트스트랩 시작
2. `Configure`에서 서비스 등록
3. 기본 서비스 등록
4. 씬 컴포넌트에 `[Inject]` 의존성 주입
5. `Initialize` 실행
6. 씬 계층 생명주기 초기화
7. `StartAsync` 실행

### Lapidary

UI View에서 사용할 수 있는 보조 컴포넌트를 제공합니다.

- `Tab`

### D3

DDD와 애플리케이션 계층에서 사용하는 순수 C# 기본 타입입니다.

Application:

- `IApplicationService`
- `ApplicationService`
- `ICommand`
- `ICommandHandler`
- `IQuery`
- `IQueryHandler`
- `IDomainEventPublisher`
- `IUnitOfWork`
- `Result`
- `Result<TData>`
- `IMapper`
- `IParameterizedMapper`

Domain:

- `Entity`
- `Entity<TId>`
- `Aggregate`
- `AggregateRoot<TId>`
- `DomainEvent`
- `ValueObject`
- `IRepository`
- `DomainException`
- `EntityNotFoundException`

D3의 Domain, Application 어셈블리는 UnityEngine과 Arc를 참조하지 않습니다. 게임 런타임이나 Unity 씬 구조와 분리된 도메인 로직을 작성할 때 사용합니다.

## 어셈블리

- `Moonstone.Core`: 생명주기와 View 기본 타입
- `Moonstone.Arc`: 부트스트랩, DI, 이벤트 버스
- `Moonstone.D3.Domain`: 도메인 계층 기본 타입
- `Moonstone.D3.Application`: 애플리케이션 계층 기본 타입
- `Moonstone`: Core를 참조하는 기본 어셈블리
