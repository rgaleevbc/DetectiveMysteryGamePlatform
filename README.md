# Detective Mystery Game Platform

A web-based platform for creating and running interactive detective mystery games. The platform allows game masters to create quests, define characters, structure game rounds, upload PDF content, and manage real-time game sessions.

## Features

- **Admin Authentication**: Secure login system for administrators
- **Quest Management**: Create and manage detective mystery quests
- **Character Management**: Define characters for each quest with public and private information
- **Round Structure**: Organize quest content into rounds for progressive gameplay
- **PDF Content**: Upload and extract content from PDF files
- **Game Session Management**: Create and control game sessions
- **Real-time Updates**: Live game progression using SignalR
- **Player Experience**: Interactive interface for players to join games and view their character information

## Technology Stack

- **Backend**: ASP.NET Core API with PostgreSQL database
- **Frontend**: Blazor WebAssembly
- **Real-time Communication**: SignalR
- **PDF Processing**: Custom PDF content extraction service
- **Authentication**: JWT-based authentication

## Project Structure

- **DetectiveMysteryGamePlatform.Api**: Backend API services
- **DetectiveMysteryGamePlatform.Client**: Blazor WebAssembly frontend
- **DetectiveMysteryGamePlatform.Shared**: Shared models and contracts

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL database

### Setup

1. Clone the repository
   ```
   git clone https://github.com/rgaleevbc/DetectiveMysteryGamePlatform.git
   ```

2. Configure database connection in `appsettings.json` in the API project

3. Run the API project
   ```
   cd DetectiveMysteryGamePlatform.Api
   dotnet run
   ```

4. Run the Client project
   ```
   cd DetectiveMysteryGamePlatform.Client
   dotnet run
   ```

5. Navigate to `https://localhost:5001` in your browser

## License

This project is licensed under the MIT License - see the LICENSE file for details. 