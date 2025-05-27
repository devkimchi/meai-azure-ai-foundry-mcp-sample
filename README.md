# MEAI, Azure AI Foundry, Foundry Local and MCP Sample

Did you know .NET is the most versatile language to build both frontend and backend apps? In addition to that, .NET is also capable of building AI infused app really easily. This repository provides several sample apps using Microsoft.Extensions.AI, Azure AI Foundry, Foundry Local and MCP server/client.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 17.14 or later](https://visualstudio.microsoft.com/downloads/)
- [VS Code](https://code.visualstudio.com/download) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Azure Subscription](http://azure.microsoft.com/free)
- [Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/get-started)

## Getting Started

### Build Foundry Local SDK

> **NOTE**: This is an interim solution until the official NuGet package is ready.

1. Clone Foundry Local repository.

    ```bash
    git clone https://github.com/microsoft/Foundry-Local.git foundry-local
    ```

1. Build the C# SDK.

    ```bash
    cd foundry-local/sdk/cs
    dotnet restore && dotnet build
    ```

1. Get the NuGet package location.

    ```bash
    # bash/zsh
    FOUNDRY_LOCAL_NUGET_PACKAGE_PATH=$(find $(pwd) -name "*.nupkg" -type f -exec dirname {} \; | sort -u)
    ```

    ```powershell
    # PowerShell
    $FOUNDRY_LOCAL_NUGET_PACKAGE_PATH = $(Get-ChildItem -Path . -Filter *.nupkg -Recurse).Directory.FullName
    ```

### Get ready to run apps on your local machine

1. Clone this repository.

    ```bash
    git clone https://github.com/devkimchi/meai-azure-ai-foundry-mcp-sample.git
    ```

1. Create `nuget.config`. `{{FOUNDRY_LOCAL_NUGET_PACKAGE_PATH}}` is the value from the [previous section](#build-foundry-local-sdk).

    ```bash
    # bash/zsh
    cd meai-azure-ai-foundry-mcp-sample
    cat ./nuget.sample.config \
        | sed 's|/path/to/foundry-local/sdk/cs/src/bin/Debug|{{FOUNDRY_LOCAL_NUGET_PACKAGE_PATH}}|g' \
        > nuget.config
    ```

    ```powershell
    # PowerShell
    cd meai-azure-ai-foundry-mcp-sample
    $(Get-Content -Path ./nuget.sample.config) `
        -replace "/path/to/foundry-local/sdk/cs/src/bin/Debug", "{{FOUNDRY_LOCAL_NUGET_PACKAGE_PATH}}" `
        | Out-File -Path nuget.config -Force
    ```

   > **NOTE**: This step is only required until the official NuGet package is published.

### `Microsoft.Extensions.AI` sample app

1. Make sure you have API keys for OpenAI, Azure OpenAI, GitHub Models, Anthropic and/or Google Vertex AI.
1. Store those API keys to user secrets:

    ```bash
    # OpenAI
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:openai "Endpoint=https://api.openai.com/v1;Key={{OPENAI_API_KEY}}"
    
    # Azure OpenAI
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:openai "Endpoint={{AZURE_OPENAI_ENDPOINT}};Key={{AZURE_OPENAI_API_KEY}}"
    
    # GitHub Models
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:openai "Endpoint=https://models.inference.ai.azure.com;Key={{GITHUB_PAT}}"
    
    # Anthropic
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:anthropic "Endpoint=https://api.anthropic.com;Key={{ANTHROPIC_API_KEY}}"
    
    # Google Vertex AI
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:google "Endpoint=https://generativelanguage.googleapis.com;Key={{GOOGLE_API_KEY}}"
    ```

1. Make sure which LLM you're going to use at `src/Meai.ClientApp/appsettings.json`

    ```jsonc
    // To use OpenAI, Azure OpenAI or GitHub Models
    {
      "MEAI": {
        "ChatClient": "openai"
      }
    }
    ```

    ```jsonc
    // To use Anthropic
    {
      "MEAI": {
        "ChatClient": "anthropic"
      }
    }
    ```

    ```jsonc
    // To use Google Vertex AI
    {
      "MEAI": {
        "ChatClient": "google"
      }
    }
    ```

1. Run the app.

    ```bash
    dotnet watch run --project ./src/Meai.ClientApp
    ```

### Azure AI Foundry sample app

1. Make sure you have an API key for Azure AI Foundry.
1. Store the API key to user secrets:

    ```bash
    # Azure AI Foundry
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:foundry "Endpoint={{AZURE_AI_FOUNDRY_ENDPOINT}};Key={{AZURE_AI_FOUNDRY_API_KEY}}"
    ```

1. Make sure which LLM you're going to use at `src/Foundry.ClientApp/appsettings.json`

    ```jsonc
    {
      "MEAI": {
        "ChatClient": "foundry"
      }
    }
    ```

1. Run the app.

    ```bash
    dotnet watch run --project ./src/Foundry.ClientApp
    ```

### Foundry Local sample app

1. Make sure you have installed Foundry Local CLI.
1. Add model to Foundry Local. You can add any model from the list by running `foundry model ls`.

    ```bash
    foundry model download qwen2.5-0.5b
    ```

1. Make sure which LLM you're going to use at `src/Foundry.ClientApp/appsettings.json`

    ```jsonc
    {
      "MEAI": {
        "ChatClient": "local"
      }
    }
    ```

1. Run the app.

    ```bash
    dotnet watch run --project ./src/Foundry.ClientApp
    ```

### MCP server/client sample app

1. Make sure you have API keys for OpenAI, Azure OpenAI or GitHub Models.
1. Store those API keys to user secrets:

    ```bash
    # OpenAI
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:openai "Endpoint=https://api.openai.com/v1;Key={{OPENAI_API_KEY}}"
    
    # Azure OpenAI
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:openai "Endpoint={{AZURE_OPENAI_ENDPOINT}};Key={{AZURE_OPENAI_API_KEY}}"
    
    # GitHub Models
    dotnet user-secrets --project ./src/Meai.ClientApp set ConnectionStrings:openai "Endpoint=https://models.inference.ai.azure.com;Key={{GITHUB_PAT}}"
    ```

1. Run the MCP server app.

    ```bash
    dotnet run --project ./src/McpTodo.ServerApp
    ```

1. Run the MCP client app in another terminal.

    ```bash
    dotnet watch run --project ./src/mcpTodo.ClientApp
    ```

## Known Issues: Foundry Local CLI installation

On Windows, if you keep failing to install Foundry Local CLI on your machine, try this way as a [workaround](https://github.com/microsoft/Foundry-Local/issues/79#issuecomment-2899281617):

```powershell
# Download the package and its dependency
$releaseUri = "https://github.com/microsoft/Foundry-Local/releases/download/v0.3.9267/FoundryLocal-x64-0.3.9267.43123.msix"
Invoke-WebRequest -Method Get -Uri $releaseUri -OutFile .\FoundryLocal.msix
$crtUri = "https://aka.ms/Microsoft.VCLibs.x64.14.00.Desktop.appx"
Invoke-WebRequest -Method Get -Uri $crtUri -OutFile .\VcLibs.appx

# Install the Foundry Local package
Add-AppxPackage .\FoundryLocal.msix -DependencyPath .\VcLibs.appx
```
