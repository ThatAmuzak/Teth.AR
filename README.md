# Teth.AR

Remote mixed reality training system for spatial instruction and equipment assembly.

## Video Demo
[<img src="https://img.youtube.com/vi/RqbDehOqA6Q/hqdefault.jpg" width="600" height="600"
/>](https://www.youtube.com/embed/RqbDehOqA6Q)

## Overview

Teth.AR enables synchronized remote training across passthrough mixed reality (MR) and virtual reality (VR) environments. A trainee works in a passthrough MR workspace while a remote trainer operates from a VR environment. Both interact with the same digital objects and assembly steps, with trainer demonstrations visible directly in the trainee's workspace.

### Use Cases
- Equipment maintenance and repair
- Manufacturing assembly procedures
- Warehouse operations training
- Field service instruction
- Healthcare procedure training

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Teth.AR System                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────────┐          ┌──────────────────────┐ │
│  │   Trainee (Quest 3)  │          │   Trainer (Quest 3)  │ │
│  │  Passthrough MR      │          │   VR Environment     │ │
│  │                      │          │                      │ │
│  │ - Video passthrough  │          │ - Avatar presence    │ │
│  │ - Digital overlays   │◄────────►│ - Object control     │ │
│  │ - Real tools/parts   │          │ - Gesture tracking   │ │
│  └──────────────────────┘          └──────────────────────┘ │
│           │                                  │              │
│           └──────────┬───────────────────────┘              │
│                      │                                      │
│  ┌───────────────────▼─────────────────────┐                │
│  │   Networking Layer (Photon Fusion)      │                │
│  │  - State synchronization                │                │
│  │  - Object transforms                    │                │
│  │  - Spatial anchors                      │                │
│  └─────────────────────────────────────────┘                │
│           │                         │                       │
│           ▼                         ▼                       │
│  ┌──────────────────┐    ┌──────────────────┐               │
│  │  Video Stream    │    │  Motion Stream   │               │
│  │  (Agora)         │    │  (Photon Fusion) │               │
│  │  Trainee → VR    │    │  Trainer ← → T.  │               │
│  └──────────────────┘    └──────────────────┘               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Core Components

| Component | Role | Technology |
|-----------|------|-----------|
| **Passthrough MR Workspace** | Trainee view with real environment | Meta XR SDK |
| **VR Trainer Interface** | Remote trainer environment | Meta XR SDK |
| **Object Synchronization** | Shared assembly state | Photon Fusion |
| **Gesture Recognition** | Avatar motion and trainer actions | Meta XR SDK |
| **Video Streaming** | Live passthrough feed to trainer | Agora |
| **Spatial Anchors** | Stable object placement | Meta XR SDK |

## Prerequisites

- Unity 2022.3 LTS or later
- Meta Quest 3 headsets (2 required for full testing)
- Meta Quest Developer Hub
- Android SDK/NDK (via Android Studio)
- Agora account and App ID
- Photon Fusion account and App ID

## Dependencies

All dependencies will auto-download, except the following two (as they are not UPM packages):

- Photon Fusion
`https://github.com/ExitGames/Fusion2-SDK.git`

- Agora
`https://github.com/agoraio/agora-rtc-sdk-csharp.git`

## Running the Application

#### Step 1: Configure Build Settings

```
File → Build Settings
  - Platform: Android
  - Architecture: ARM64
  - Min API Level: 29
  - Target API Level: 33+
```

#### Step 2: Set Quest Build Profile

```
File → Build Profiles
  - Select "Quest 3"
  - Texture Compression: ASTC
  - Graphics API: OpenGL ES 3
  - Enable: XR Plugin Management → Oculus
```

#### Step 3: Build APK

```bash
# From Unity menu:
File → Build And Run
```


#### Step 4: Launch Session


Both devices automatically join the same session using the configured `sessionId`. Photon Fusion handles state synchronization, and Agora streams video from trainee to trainer.
