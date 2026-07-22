# Moonstone

A lightweight reusable Unity package.

## Project Structure Template

You can set up project structure templates in the Editor in Window > Material Stone > Project Setup.

Press apply button to copy template to asset folder.
After applying, you can see directory structure like below.

```txt
Assets/
├── 01_Scenes/
│   └── Template Scene.unity
├── 02_Scripts/
├── 03_Prefabs/
├── 04_Data/
└── 05_Resources/
    ├── Animations/
    ├── Externals/
    ├── Fonts/
    ├── Materials/
    ├── Shaders/
    ├── Sounds/
    └── Sprites/
```

## Hierarchy Customization

You can customize hierarchy in Editor in Tools > Moonstone > Hierarchy Customization > Edit Settings.

## UI Script Management

You can manage UI scripts in Editor in Tools > Moonstone > UI Script Manager.

Set your app name and output path.
You can generate ui scripts for children of canvas.

Press attach button to attach component of generated scripts to children of canvas.
You can also detach that by pressing detach button.

## Scripts

- Core
  - Lifecycle
    - IInitializable
    - IAsyncInitializable
    - ILifecycleDisposable
    - LifecycleBehaviour
    - LifecycleRunner

- Ore
  - Lifecycle
    - Local
      - Entity
      - System
      - View
      - Visible
    - LifecycleState
  - Model
  - Repository
  - Bootstrapper
  - Container

- Arc
  - DependencyInjection
    - InjectAttribute
    - IResolver
    - SceneInjector
    - ServiceResolver
  - Events
    - EventBus
    - IEventBus
    - EventSubscription
  - Bootstrapper
  - Container

Arc starts from a Bootstrapper component placed in the scene.
Override Configure, Initialize, StartAsync, and Dispose to configure services and lifecycle flow.
Register service instances in Configure with Container.Register.
Arc supports [Inject] fields and [Inject] properties.
Bootstrapper registers EventBus as IEventBus by default unless Configure registers a custom IEventBus first.
Bootstrapper initializes scene components through LifecycleRunner.InitializeHierarchyAsync so sync and async lifecycle components share one initialization path.

- Lapidary
  - View
    - UI
      - Tab

- D3
  - Application
    - ICommand
    - ICommandHandler
    - IQuery
    - IQueryHandler
    - IDomainEventPublisher
    - IUnitOfWork
    - Result
    - IMapper
    - IParameterizedMapper
  - Domain
    - Entity
    - AggregateRoot
    - DomainEvent
    - ValueObject
    - IRepository
    - DomainException

D3 provides DDD and application-layer primitives.
It does not own bootstrapping, dependency injection, scene injection, or event bus infrastructure.
D3 Domain and Application assemblies do not reference Arc or UnityEngine.
