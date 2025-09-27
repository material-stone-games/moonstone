# moonstone

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
│   ├── Core/
│   │   ├── Manager/
│   │   └── Service/
│   ├── Dev/
│   ├── Model/
│   └── View/
│       ├── UI/
│       └── World/
├── 03_Prefabs/
├── 04_Data/
├── 05_Resources/
│   ├── Animations/
│   ├── Externals/
│   ├── Fonts/
│   ├── Materials/
│   ├── Shaders/
│   ├── Sounds/
│   └── Sprites/
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
  - Application
  - ServiceLocator
  - Managers
    - Event
  - Services

- View
  - UI
    - View
    - Scene
    - Tab
