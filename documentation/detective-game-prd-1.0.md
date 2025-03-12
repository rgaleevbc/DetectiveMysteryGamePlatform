# Detective Mystery Game Platform PRD

## 1. Product Overview

### 1.1 Product Definition
The Detective Mystery Game Platform is a web-based application designed to transform offline detective story games into an online experience. The platform enables game masters to create, manage, and run interactive detective mystery games where players take on character roles to solve murder mysteries together.

### 1.2 Objectives
- Create a platform that allows offline detective mystery quests to be played online
- Provide tools for game masters to manage quest content, characters, and game progression
- Enable a seamless player experience with minimal friction to join games
- Support the core gameplay mechanics of collaborative mystery-solving with progressive round-based revelation

### 1.3 Target Audience
- Primary users: Game masters who want to host detective mystery games online
- Secondary users: Players invited to participate in detective mystery games

## 2. Features and Functionality

### 2.1 Authentication and User Management

#### Admin Authentication
- Secure login for the game master/admin with email and password
- Session management with appropriate security measures
- Password reset functionality

#### Player Experience
- Players join via email invitation links
- Players only need to provide a name/nickname to participate (no account creation required)
- Session persistence to allow rejoining an active game

### 2.2 Admin Portal

#### Quest Management
- Create, edit, and delete quests
- Upload PDF files containing quest content (supporting files 200KB to 5MB)
- Interface for splitting PDFs into relevant sections:
  - Character information
  - Round descriptions
  - Clues
  - General and round-specific instructions
- Organize content by rounds (typically 5-10 rounds per quest)
- Save quest creation progress for later completion

#### Character Management
- Create and edit character profiles with descriptions
- Assign characters to players
- Manage character-specific instructions and information
- Create a public character list viewable by all players

#### Game Session Management
- Create new game sessions based on existing quests
- Invite players via email with unique links
- Monitor player connection status
- Save and resume game sessions
- Run multiple concurrent game sessions

#### Game Master Controls
- Reveal clues to all players
- Advance the game to the next round
- Display timers/countdowns when needed
- Show results or answers at appropriate times
- Control the flow of information to players

### 2.3 Player Interface

#### Game Lobby
- Join games via unique invitation links
- Select/confirm assigned character
- View public character list with descriptions
- See current player roster and connection status

#### Gameplay Screen
- View current round information
- Access character-specific instructions
- See revealed clues and information
- Track game progress (current round, time remaining if applicable)
- Access historical information from previous rounds

### 2.4 Content Display
- Display PDF-extracted content as images in the first version
- Organize content by round, character, and information type
- Ensure readability on desktop screens
- Support for different PDF formats and layouts

### 2.5 Game State Persistence
- Save game state automatically
- Allow games to be paused and resumed later
- Maintain history of revealed information and round progression

## 3. Technical Architecture

### 3.1 Technology Stack
- **Frontend**: Blazor WebAssembly for interactive SPA experience
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL for structured data storage
- **File Storage**: File system storage on VPS for uploaded content
- **Authentication**: ASP.NET Core Identity for authentication and authorization
- **Hosting**: Self-hosted on personal VPS

### 3.2 Data Model

#### Core Entities

**User**
- Id: UUID
- Email: string
- PasswordHash: string
- Role: enum (Admin, Player)

**Quest**
- Id: UUID
- Title: string
- Description: string
- CreatedAt: datetime
- UpdatedAt: datetime
- CreatedById: UUID (reference to User)
- NumberOfRounds: int

**Character**
- Id: UUID
- QuestId: UUID (reference to Quest)
- Name: string
- Description: string
- IsPublicInfo: boolean
- AvatarImagePath: string (nullable)

**Round**
- Id: UUID
- QuestId: UUID (reference to Quest)
- Number: int
- Title: string
- Description: string

**Content**
- Id: UUID
- QuestId: UUID (reference to Quest)
- RoundId: UUID (nullable, reference to Round)
- CharacterId: UUID (nullable, reference to Character)
- Type: enum (GeneralInstruction, CharacterInstruction, Clue, RoundDescription)
- Title: string
- ImagePath: string
- IsPublic: boolean
- DisplayOrder: int

**GameSession**
- Id: UUID
- QuestId: UUID (reference to Quest)
- CreatedAt: datetime
- UpdatedAt: datetime
- Status: enum (Created, InProgress, Paused, Completed)
- CurrentRoundId: UUID (reference to Round)

**PlayerSession**
- Id: UUID
- GameSessionId: UUID (reference to GameSession)
- CharacterId: UUID (nullable, reference to Character)
- PlayerName: string
- InvitationToken: string
- Email: string
- LastActiveAt: datetime
- IsConnected: boolean

### 3.3 API Endpoints

#### Authentication
- POST /api/auth/login
- POST /api/auth/logout
- POST /api/auth/reset-password

#### Quest Management
- GET /api/quests
- GET /api/quests/{id}
- POST /api/quests
- PUT /api/quests/{id}
- DELETE /api/quests/{id}
- POST /api/quests/{id}/upload-pdf
- POST /api/quests/{id}/extract-content

#### Character Management
- GET /api/quests/{questId}/characters
- POST /api/quests/{questId}/characters
- PUT /api/characters/{id}
- DELETE /api/characters/{id}
- POST /api/characters/{id}/assign

#### Game Session Management
- GET /api/game-sessions
- POST /api/game-sessions
- GET /api/game-sessions/{id}
- PUT /api/game-sessions/{id}/status
- POST /api/game-sessions/{id}/invite-player
- PUT /api/game-sessions/{id}/advance-round

#### Player API
- GET /api/player/{token}/game-info
- GET /api/player/{token}/character
- GET /api/player/{token}/public-characters
- GET /api/player/{token}/current-round
- GET /api/player/{token}/revealed-content
- POST /api/player/{token}/join

### 3.4 File Management
- File upload for PDFs (multipart form data)
- Image extraction and storage from PDFs
- Secure file access controls
- Clean temporary files after processing

## 4. User Interface Design

### 4.1 Design Principles
- Noir detective visual theme (dark backgrounds, high contrast elements)
- Clean, readable typography for extensive text content
- Clear distinction between player and character information
- Intuitive navigation between rounds and game elements
- Responsive design focused on desktop browsers
- Clear visual indicators for game state and progression

### 4.2 Key Screens

#### Admin Portal
- Dashboard with quest list and active game sessions
- Quest creator/editor with PDF upload and content extraction tools
- Character management interface
- Game session control panel
- Player invitation management

#### Player Experience
- Game lobby / waiting room
- Character information screen
- Current round view with relevant information
- Clue display area
- Game history/previous rounds access
- Round transition screens

### 4.3 Interface Components
- PDF content viewer
- Round navigation
- Character profiles
- Clue cards
- Game progress indicator
- Admin control panel
- Round timer (when applicable)

## 5. Development Roadmap

### 5.1 Phase 1: Core Platform (MVP)
- Authentication system for admin
- Basic quest creation and PDF upload
- PDF content extraction as images
- Character creation and management
- Simple game session creation and player invitations
- Basic round progression mechanics
- Essential game master controls

### 5.2 Phase 2: Enhanced Gameplay
- Game state persistence and resume functionality
- Enhanced PDF splitting and organization
- Improved game master controls
- Round timers and notifications
- More robust character management

### 5.3 Future Enhancements (Post MVP)
- Text recognition for PDF content
- Additional game themes beyond noir detective
- Built-in communication tools
- Automated game progression features
- Scoring and statistics
- Enhanced multimedia support (audio, video)
- Mobile responsive design

## 6. Security Considerations

### 6.1 Authentication Security
- Secure password storage using ASP.NET Identity (bcrypt)
- HTTPS for all communications
- Proper session management and timeout
- Rate limiting for login attempts

### 6.2 Data Protection
- PostgreSQL security best practices
- Regular database backups
- Data validation and sanitization
- Protection against SQL injection and XSS

### 6.3 File Security
- Secure file storage with proper permissions
- Validation of uploaded content
- Non-executable storage locations
- Proper cleanup of temporary files

### 6.4 Access Controls
- Role-based access control
- Secure invitation links with expiration
- Validation of user permissions for all operations

## 7. Technical Implementation Notes

### 7.1 PDF Processing
- Use a library like iTextSharp or PDFsharp for PDF processing
- Extract images at appropriate resolution for readability
- Create an intuitive interface for selecting regions from PDFs
- Store extracted content with proper metadata for organization

### 7.2 Real-time Status Updates
- Use SignalR for real-time updates of game state
- Update player connection status
- Push notifications for round changes and new content

### 7.3 Deployment Considerations
- Configure Nginx as reverse proxy
- Set up SSL certificates via Let's Encrypt
- Implement proper logging with Serilog
- Configure database connection pooling for efficiency
- Set up automated backups

### 7.4 Performance Optimization
- Implement caching for static content
- Optimize database queries with proper indexing
- Lazy loading of game content by round
- Compression of responses

## 8. Assumptions and Constraints

### 8.1 Assumptions
- Players will have stable internet connections
- PDF files will be of reasonable quality for content extraction
- External communication tools (like Discord, Zoom) will be used alongside the platform
- Desktop browsers will be the primary access point

### 8.2 Constraints
- Initial version focuses on desktop experience only
- File size limitations (5MB maximum for PDF uploads)
- Manual game master control rather than automation
- Basic game progression without complex branching

## 9. Success Metrics

- Successful completion of detective games without technical issues
- Ease of quest creation and management (measured by time spent)
- Player satisfaction and engagement
- Game master ability to effectively control game flow
- System performance under concurrent game sessions

## 10. Appendix

### 10.1 Technical References
- .NET Core documentation: https://docs.microsoft.com/en-us/aspnet/core/
- Blazor WebAssembly: https://docs.microsoft.com/en-us/aspnet/core/blazor/
- PostgreSQL documentation: https://www.postgresql.org/docs/
- PDF processing libraries:
  - iTextSharp: https://github.com/itext/itextsharp
  - PDFsharp: http://www.pdfsharp.net/

### 10.2 Glossary
- **Quest**: A complete detective mystery story with characters, rounds, and clues
- **Round**: A distinct phase of gameplay with specific information revealed
- **Character**: A role assigned to a player with specific information and objectives
- **Clue**: A piece of information revealed during gameplay to help solve the mystery
- **Game Master**: The administrator who controls the flow of the game and information revealed
- **Game Session**: An instance of a quest being played by a group of players
