# .NET Starter Kit

A starter kit for .NET projects with a clean architecture approach, including multiple data persistence options (
InMemory and FileData), comprehensive testing, and automated build tasks.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Task](https://taskfile.dev/installation/) (for automated development tasks)

## Quick Start

1. **Initial Setup**
   ```bash
   task setup
   ```
   This will restore NuGet packages and create the required `appsettings.Development.json` file.

2. **Build the Solution**
   ```bash
   task build
   ```

3. **Run Tests**
   ```bash
   task test
   ```

4. **Run the API**
   ```bash
   task run
   # or for development environment with enhanced logging:
   task run-dev
   ```

## Available Tasks

You can see all available tasks by running:

```bash
task --list
```

### Core Tasks

- **`task setup`** - Initial project setup (restore packages + create development appsettings)
- **`task build`** - Build the entire solution
- **`task test`** - Run all unit tests
- **`task run`** - Run the WebAPI project
- **`task run-dev`** - Run the WebAPI with Development environment
- **`task clean`** - Clean build artifacts
- **`task lint`** - Run code analysis
- **`task lint-fix`** - Fix code style issues automatically

### Additional Tasks

- **`task restore`** - Restore NuGet packages only
- **`task watch`** - Run the API with file watching (auto-restart on changes)
- **`task help`** - Show available tasks

## Project Structure

```
├── Common/                     # Shared models and interfaces
├── Common.Tests/               # Tests for Common project
├── FileData/                   # File-based data persistence layer
├── FileData.Tests/             # Tests for FileData project
├── InMemory/                   # In-memory data persistence layer
├── InMemory.Tests/             # Tests for InMemory project
├── Logic/                      # Business logic layer
├── Logic.Tests/                # Tests for Logic project
├── WebAPI/                     # ASP.NET Core Web API
├── WebAPI.Integration.Tests/   # Integration tests for WebAPI
├── DotNetStarterKit.sln        # Solution file
└── Taskfile.yml                # Task automation configuration
```

## Development Workflow

1. **Start a new feature**: Run `task setup` to ensure you have the latest dependencies
2. **Development**: Use `task watch` for auto-restarting development
3. **Testing**: Run `task test` frequently to ensure your changes don't break existing functionality
4. **Build verification**: Run `task build` before committing changes

## Configuration

The application supports different data persistence modes via `appsettings.json` configuration:

- **InMemory**: Data stored in memory (default for development)
- **FileData**: Data persisted to local JSON files

Example configuration in `appsettings.Development.json`:

```json
{
  "CrudFactory": {
    "Implementation": "InMemory"  // or "FileData"
  },
  "FileData": {
    "Directory": "data"  // Directory for FileData storage
  }
}
```

## API Documentation

When running in Development mode, Swagger UI is available at: `http://localhost:5272/swagger`

## Contributing

1. Make sure all tests pass: `task test`
2. Ensure the build succeeds: `task build`
3. Test the API runs correctly: `task run-dev`