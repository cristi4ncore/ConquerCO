# Codebase Analysis

This document provides a high-level overview of the ConquerCO private server codebase.

## Project Structure

The solution consists of two main C# projects:

*   **`AccServer`**: Handles user authentication and account management. It connects to a MySQL database to store account information.
*   **`GameServer` (`DeathWish.csproj`)**: Contains the core game logic, including character management, combat, and NPC interactions. It uses both MySQL and a large number of flat files (`.ini`, `.txt`) for game data.

Other important directories include:

*   **`DB`**: Contains game data files that are loaded at runtime by the `GameServer`. This includes information about items, NPCs, quests, and more.
*   **`ServerIPNameChanger`**: A utility tool for changing the server's IP address and name in the configuration files.

## Database

*   **`AccServer`**: The database interaction is straightforward, primarily dealing with an `accounts` table for user authentication and a `servers` table to list available game servers.
*   **`GameServer`**: The database logic is far more complex. The `Database` directory in the `GameServer` project contains numerous files for managing different aspects of the game:
    *   **`Server.cs`**: This file is the central hub for loading all game data, including information from the database and the flat files in the `DB` directory.
    *   **`ItemType.cs`**: Defines the properties of all items in the game, including their stats, requirements, and upgrade paths.
    *   **Other files**: There are dedicated files for managing clans, guilds, quests, NPCs, and other game features.

## Network Layer

The client-server communication is handled through a custom TCP socket implementation.

*   **`AccServer/Network`**: Manages the initial connection and authentication process.
    *   **`ServerSocket.cs`**: A basic socket server that accepts incoming connections.
    *   **`ClientWrapper.cs`**: A wrapper around the `Socket` class that manages the connection for a single client.
*   **`GameServer/ServerSockets`**: Handles the main game communication.
    *   **`ServerSocket.cs`**: A more advanced socket server that manages the game's client connections.
    *   **`SecuritySocket.cs`**: A secure socket implementation that uses a Diffie-Hellman key exchange to encrypt communication between the client and server.

## Game Logic

The core game logic is located in the `GameServer/Game` directory.

*   **`GamePackets.cs`**: Defines a comprehensive list of packet IDs that represent the various actions and events in the game.
*   **`MsgServer` directory**: Contains a large number of files, each responsible for handling a specific packet type.
    *   **`MsgLoginHandler.cs`**: Manages the player login process, sending character data and world state to the client.
    *   **`MsgMovement.cs`**: Handles player movement requests, validating them against the game map and broadcasting the new position to other clients.
    *   **`AttackHandler/Attack.cs`**: The main entry point for all combat interactions. It processes attack requests, calculates damage, and applies status effects. The `AttackHandler` directory contains a wide variety of files for different spells and abilities.

## Summary for Future Modifications

*   **Adding/Modifying Items**: To add or modify items, you will need to edit the `itemtype.txt` file in the `DB` directory and potentially modify the `ItemType.cs` file in the `GameServer`'s `Database` directory.
*   **Changing Game Mechanics**: To change game mechanics, such as how a spell works, you will need to find the corresponding file in the `GameServer/Game/MsgServer/AttackHandler` directory and modify the logic.
*   **Adding New Features**: Adding new features will likely involve creating new packet types in `GamePackets.cs` and creating new handler files in the `GameServer/Game/MsgServer` directory.
