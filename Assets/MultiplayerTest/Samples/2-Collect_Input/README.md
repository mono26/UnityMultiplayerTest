To gather input from the player it is needed to create an struct, that implements [InetworkInput] Interface.

It is set in the `OnInput` callback, stored in the `Sample_NetworkInputData` struct and used in the `GetInput` method.

In order to prevent cheating, the client sends the input to the server, the server handles the movement logic, and changes are reflected back on the client

---
#### `Sample_NetworkInputData`
- Stores the input data sent by the player

#### `OnInput`
- Reads and sets the input of the player

#### `FixedUpdateNetwork`
- Fusion FixedUpdate timing callback

#### `GetInput`

- Gets the input of the player in order to use it

---

<details>
<summary> NetworkInput
</summary>

```cs
...
    public struct Sample_NetworkInputData : INetworkInput
    {
        public Vector3 direction;
    }
...
...
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new Sample_NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }
...
```

</details>

---

[InetworkInput]:<https://doc-api.photonengine.com/en/fusion/current/interface_fusion_1_1_i_network_input.html>