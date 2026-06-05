# Vktun.Antd Project Architecture & Structure

## Project Overview

Vktun.Antd is a multi-platform .NET desktop UI component library inspired by Ant Design. 
It provides a shared theme token system and platform-specific implementations for WPF and Avalonia.

**Key Characteristics:**
- Target Framework: .NET 8.0 (WPF: net8.0-windows, others: net8.0)
- Architecture: Layered with platform-agnostic core
- License: MIT
- Version: 1.0.0
- Main Language: C# with XAML/AXAML for styling

## Directory Structure

```
vktun.antd/
├── src/
│   ├── Vktun.Antd.Core/         # Platform-agnostic theme tokens
│   ├── Vktun.Antd.Wpf/          # WPF control implementations
│   └── Vktun.Antd.Avalonia/     # Avalonia control implementations
├── samples/
│   ├── Vktun.Antd.Wpf.Sample/   # WPF sample app
│   └── Vktun.Antd.Avalonia.Sample/  # Avalonia sample app
├── tests/                        # Unit tests
├── Vktun.Antd.slnx              # Solution manifest
└── README.md                     # Usage documentation
```

## Architectural Layers

### 1. Core Layer (Vktun.Antd.Core)
**Purpose:** Platform-agnostic token system and color algorithms

**Key Classes:**
- AntdSeedToken: Immutable seed values (colors, sizes, spacing)
- AntdTokenFactory: Creates resolved AntdTokenSet from seed + theme mode
- AntdTokenSet: Complete resolved tokens with computed colors/sizes
- AntdColor: RGB color with parsing utilities
- AntdColorMath: Color blending, opacity operations
- AntdResourceKeys: String constants for resource lookup
- AntdEnums: Shared enumerations (ButtonType, Status, ControlSize, etc.)
- AntdThemeMode: Enum (Light, Dark, Compact)

**Dependencies:** None (pure .NET 8.0)

**Token Flow:**
```
AntdSeedToken (user input)
    ↓
AntdTokenFactory.Create(mode, seed)
    ↓
AntdTokenSet (resolved values)
    ↓
Platform-specific ResourceDictionary
```

### 2. WPF Platform Layer (Vktun.Antd.Wpf)
**Target:** Windows only, net8.0-windows

**Key Responsibilities:**
- Control implementations using WPF patterns
- Attached properties (Assist pattern) for standard controls
- Service implementations (Message, Modal, Notification)
- WPF-specific ResourceDictionary management

**Architecture Pattern:**
- Attached Behaviors: ButtonAssist, InputAssist, StatusAssist, etc.
- Custom Controls: 60+ (Button, Input, Card, Table, Menu, etc.)
- Services: Message, Modal, Notification with OverlayHost
- Converters: AntdButtonForegroundContrastConverter for contrast
- Theme Manager: AntdThemeManager applies AntdThemeResources globally/scoped

**Theme Flow:**
```
AntdSeedToken
    ↓
AntdThemeManager.Apply(Application, mode, seed)
    ↓
AntdThemeResources (WPF ResourceDictionary)
    ↓
Application.Resources or FrameworkElement.Resources
    ↓
Controls use {DynamicResource Antd.Brush.Primary}
```

### 3. Avalonia Platform Layer (Vktun.Antd.Avalonia)
**Target:** Avalonia 11.3.2, cross-platform, net8.0

**Key Responsibilities:**
- Control implementations using Avalonia patterns
- Avalonia-specific ResourceDictionary management
- OverlayHost for floating UI
- Style management via AXAML

**Architecture Pattern:**
- Declarative Styling in Themes/Generic.axaml
- AntdThemeResources extends Avalonia ResourceDictionary
- AntdThemeManager applies themes to Application.Current
- Services: Message, Modal, Notification (overlay-based)

## Cross-Platform Design Principles

1. **Shared Core, Independent UI**
   - Core has token definitions, enums
   - WPF and Avalonia do NOT reference each other
   - Independent implementations per platform

2. **Token-Driven Theming**
   - Single source: AntdSeedToken
   - Factory computes semantic colors based on mode
   - Each platform maps to its resource system

3. **Resource Key Convention**
   - Centralized keys in AntdResourceKeys
   - Format: "Antd.<Category>.<Name>"
   - Example: "Antd.Brush.Primary", "Antd.FontSize.Base"

4. **No Circular Dependencies**
   - Core → (no deps)
   - WPF → Core only
   - Avalonia → Core only

## Key Types

AntdSeedToken properties:
- PrimaryColor (default: #1677FF)
- SuccessColor (default: #52C41A)
- WarningColor (default: #FAAD14)
- ErrorColor (default: #FF4D4F)
- FontSizeBase (default: 14d)
- ControlHeightSmall/Middle/Large (32, 40, 48)
- BorderRadius (default: 8d)

AntdThemeMode: Light, Dark, Compact

Shared Enums:
- AntdButtonType: Default, Primary, Dashed, Text, Link
- AntdStatus: None, Success, Warning, Error
- AntdControlSize: Small, Middle, Large
- Plus 50+ component-specific enums

## Package Dependencies

Core (Vktun.Antd.Core): Zero dependencies
WPF (Vktun.Antd.Wpf): Core only (implicit System.Windows)
Avalonia (Vktun.Antd.Avalonia): Core, Avalonia 11.3.2

## Recent Development

Latest commits:
1. 037eca6 - fix(theme): Cyber theme color values
2. f69d665 - docs(README): Usage guides
3. 9589acb - merge: Avalonia full port
4. 5c9fe61 - feat: Port components to Avalonia
5. d2862d4 - feat: Multi-platform refactor

Status: Active development—Avalonia port complete, improving theme consistency.

## Component Coverage

60+ components across both platforms:
- Generic: Button, FloatButton, Typography
- Layout: Divider, Row, Col, Space, Flex, Layout
- Navigation: Breadcrumb, Dropdown, Menu, Pagination, Steps, Tabs
- Data Input: Checkbox, Input, PasswordInput, ComboBox, Select, Form, etc.
- Data Display: Avatar, Badge, Card, Collapse, Table, Tag, Timeline, etc.
- Feedback: Alert, Drawer, Message, Modal, Notification, Spin, etc.

## Theme Application (End-to-End)

1. Create seed token with custom colors/sizes
2. Call AntdThemeManager.Current.Apply(app, mode, seed)
3. Factory computes semantic tokens based on mode
4. AntdThemeResources populates ResourceDictionary
5. Controls consume via {DynamicResource}
6. Theme switches at runtime with dynamic resources

## Summary

Framework: .NET 8.0
Architecture: Layered (Core → WPF/Avalonia)
Core Dependencies: None
Styling: Resource-driven, dynamic
Theme Modes: Light, Dark, Compact
Controls: 60+ per platform
Distribution: 3 NuGet packages
