# smartPIMS - Electron.js Port

A modern Electron.js port of the legacy C# WinForms smartPIMS application for ultrasonic thickness measurement systems.

## Overview

smartPIMS is a professional ultrasonic thickness measurement system for industrial applications. This Electron.js version modernizes the original C# WinForms application while maintaining full compatibility with existing DSI devices and data formats.

## Features

### Core Functionality
- **Device Communication**: ModBus protocol support for DSI ultrasonic sensors
- **Real-time Measurement**: Live thickness readings with A-scan data visualization
- **Multi-device Support**: Scan and connect to multiple DSI devices simultaneously
- **Data Management**: Import/export measurement data in CSV and XML formats
- **Commissioning Tools**: Device setup and calibration workflows

### Modern Enhancements
- **Cross-platform**: Runs on Windows, macOS, and Linux
- **Responsive UI**: Modern web-based interface with responsive design
- **Type Safety**: Full TypeScript implementation for better code quality
- **Real-time Updates**: Event-driven architecture for live data streaming
- **Multi-language**: Support for English, Japanese, and Chinese

## Getting Started

### Prerequisites
- Node.js 18.x or higher
- npm 8.x or higher
- Serial port access (for device communication)

### Installation
```bash
# Install dependencies
npm install

# Start development mode
npm run dev

# Build for production
npm run build
```

## Project Structure
```
src/
├── main/           # Main Electron process
├── renderer/       # Renderer process (UI)
├── logic/          # Core business logic (TypeScript)
└── preload.js      # Secure IPC bridge
legacy_cs_project/  # Original C# WinForms application
```

## License

© 2015-2024 Sensor Networks, Inc. All rights reserved.
