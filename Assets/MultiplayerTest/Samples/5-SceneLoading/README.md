# Scene Loading

This sample show how to load different scenes depending on the selected `Game Type` by the player

---


<details>
<summary>Scene Loading</summary>

`SceneRef scene` is the scene that will be loaded when the player joins the game. It is set by the `Host` when creating the game session; and `Clients` will load the scene when they join the game.

```cs
...
public async Task CreateSessionWithCustomProperties(Sample_Utils.GameMap map, Sample_Utils.GameType gameType, SceneRef scene)
{
    ...
    var result = await _runner.StartGame(new StartGameArgs()
    {
        GameMode = GameMode.AutoHostOrClient,
        SessionProperties = customProperties,
        Scene = scene,
        SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>(),
    });
    ...
}
...
```

</details>
