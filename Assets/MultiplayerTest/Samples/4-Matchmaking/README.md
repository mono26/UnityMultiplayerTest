# Matchmaking

Photon Fusion exposes a set of API calls that can be used to create the best experience for players looking for the perfect match

```cs
    NetworkRunner.StartGame(new StartGameArgs {
    // other args...
    SessionName = [string],
    SessionProperties = [Dictionary<string, SessionProperty>],
    CustomLobbyName = [string],
    DisableClientSessionCreation = [bool],
    PlayerCount = [int],
    DisableNATPunchthrough = [bool],
    CustomSTUNServer = [string],
    AuthValues = [AuthenticationValues],
    });
```

---

#### `Game Session`:
- Or just `Session` is where Players meet to play a match or communicate. This is what gets published in the Photon Cloud and it is made available so other clients can search, filter, and join a particular game. Communication outside of any Session is not possible and any client can only be active in one room. Game Sessions have the following properties and methods: can be created and joined by name, `Custom Properties`, has a maximum amount of players, can be hidden or visible, can be closed (no one can join) or opened

#### `SessionName`:
- The Game Session's Name, it will identify the session on the Photon Cloud and it must be unique within a [Region](https://doc.photonengine.com/en-us/fusion/current/connection-and-authentication/regions). If no name is set, Fusion will generate a random `GUID` to identify the Session.

#### `SessionProperties`:
- The Session's Custom Properties are the way to include metadata on your Game Session, like the game mode/type or the current map, for example.

#### `PlayerCount`:
- Defines the max number of clients that can join a Session. This parameter is only used when creating a new Session and by default, it takes the value from the `Default Players` field on the `NetworkProjectConfig/Simulation`.

---

<details>
<summary>Join a Random Session</summary>

This way, the local peer will start and connect to a `Random Game Session`, if none can be found, it will create a new one with a random `Session Name`

```cs
public async Task StartPlayer(NetworkRunner runner)
{
    var result = await runner.StartGame(new StartGameArgs()
    {
        GameMode = GameMode.AutoHostOrClient, // or GameMode.Shared
    });

    if (result.Ok) {
        // all good
    } else {
        Debug.LogError($"Failed to Start: {result.ShutdownReason}");
    }
}
```

</details>
<details>
<summary>Starting A New Game Session With Custom Properties</summary>

The sample code shows the use of `Enums` for the `Custom Properties` values of the Game Session, but this is just one way to add meaning to the values. Calling the `runner.StartGame` as a Host is enough to start a new session with a Random Name and by using the `SessionProperties` argument, Fusion will include those properties in the Session

```cs
public enum GameType : int 
{
    FreeForAll,
    Team,
    Timed
}

public enum GameMap : int
{
    Forest,
    City,
    Desert
}

public async Task StartHost(NetworkRunner runner, GameMap gameMap, GameType gameType)
{
    var customProps = new Dictionary<string, SessionProperty>();

    customProps["map"] = (int)gameMap;
    customProps["type"] = (int)gameType;

    var result = await runner.StartGame(new StartGameArgs()
    {
        GameMode = GameMode.Host,
        SessionProperties = customProps,
    });

    if (result.Ok) {
        // all good
    } else {
        Debug.LogError($"Failed to Start: {result.ShutdownReason}");
    }
}
```

</details>