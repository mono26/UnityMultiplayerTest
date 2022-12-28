To start a game with Fusion the `Startgame` method needs to be called on the Fusion [NetworkRunner]

To do this, it is created a "Spawner" that implements the `INetworkRunnerCallback` Interface

---
#### `Startgame`
- Creates the `NetworkRunner` and lets it know that this client will be providing input
- Starts a new session with a name and specified `GameMode`

#### `OnGUI`
- Draws a UI for the player to choose which game mode starts in

#### `OnPlayerJoined` And `OnPlayerLeft`
- Sapwns/Despawns players in the scene

#### `_playerPrefab`
- [Network Object] that contains tha character for the player
- Has attached a CharacterController

#### `_spawnedCharacters`
- Saves a reference of the current players

---

[NetworkRunner]:<https://doc-api.photonengine.com/en/fusion/current/class_fusion_1_1_network_runner.html#details>
[Network Object]: <https://doc-api.photonengine.com/en/fusion/current/class_fusion_1_1_network_object.html>