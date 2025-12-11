# SWU Metaverse - Inventors Day

A Unity-based metaverse application for SWU (Srinakharinwirot University) Inventors Day event, featuring immersive virtual environments and interactive character selection systems.

## 📋 Requirements

### Unity Version
- **Unity Editor 2022.3.21f3** (LTS)
- Universal Render Pipeline (URP)

### System Requirements
- Windows 10/11 (64-bit)
- DirectX 11 compatible graphics card
- 8 GB RAM minimum (16 GB recommended)
- 10 GB available disk space

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/YourUsername/SWU-Metaverse-InventorsDay.git
cd SWU-Metaverse-InventorsDay
```

### 2. Install Git LFS
This project uses Git LFS for large assets. Make sure you have Git LFS installed:
```bash
git lfs install
git lfs pull
```

### 3. Open in Unity
1. Open Unity Hub
2. Click "Add project from disk"
3. Select the cloned repository folder
4. Unity will automatically import and compile the project

## 📦 Dependencies

### Required Packages

#### Singleplayer Select Character System (SSS)
This project requires the SSS package for character selection functionality.

**Installation:**
1. Download or clone the SSS package from: https://github.com/NonthawatKRT/Singleplayer-SelectCharacter-System
2. Import the package into your project
3. **Important**: Move all imported third-party assets to `Assets/_ThirdpartyAsset/` folder to maintain project organization

### Package Management
- All third-party packages and assets should be placed in `Assets/_ThirdpartyAsset/`
- This helps maintain clean project structure and easier asset management
- The `_ThirdpartyAsset` folder is properly configured in `.gitignore` and `.gitattributes`

## 📁 Project Structure

```
Assets/
├── _Project/           # Main project files
│   ├── Art/           # Art assets (models, textures, materials)
│   └── Scenes/        # Unity scenes
├── _Resource/         # Character and game resources
├── _Modules/          # Custom modules and systems
├── _ThirdpartyAsset/ # Third-party packages and assets
└── Settings/         # Project settings and configurations
```

## 🎮 Features

- **Character Selection System**: Interactive character selection with multiple avatar options
- **Metaverse Environment**: Immersive virtual spaces for the Inventors Day event
- **Cross-platform Compatibility**: Built with URP for broad device support
- **Optimized Performance**: Git LFS integration for efficient asset management

## 🛠️ Setup Instructions

### Third-Party Asset Management
When importing any new packages or assets:

1. **Import the package** through Unity Package Manager or Asset Store
2. **Locate imported files** in the Assets folder
3. **Move all third-party content** to `Assets/_ThirdpartyAsset/`
4. **Update references** if necessary in your scripts or prefabs
5. **Commit changes** to maintain project structure

### Character System Setup
1. Ensure the SSS package is properly imported to `_ThirdpartyAsset`
2. Configure character prefabs in the `_Resource/PlayerCharacter/` directory
3. Set up character selection scenes as needed

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the project structure guidelines
4. Commit your changes (`git commit -m 'Add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

## 📝 Development Guidelines

- Keep all third-party assets in `_ThirdpartyAsset` folder
- Use meaningful commit messages
- Test on Unity 2022.3.21f3 before submitting
- Follow Unity's naming conventions
- Document any new features or systems

## 📄 License

This project is developed for SWU Inventors Day event. Please refer to the license file for usage terms.

## 🎓 About SWU Inventors Day

This project is part of Srinakharinwirot University's Inventors Day, showcasing innovative technology and creative solutions in virtual environments.

---

**Note**: Make sure to have Unity Editor 2022.3.21f3 installed before opening the project. The project may not work correctly with other Unity versions.

