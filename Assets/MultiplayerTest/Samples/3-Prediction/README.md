# Predicted movement

It is used to provide snappy feedback on the client in a server authoritative network game.

In order to ahieve this it is needed to create a kinematic object:
- Create a new Empty GameObject and add a [NetworkTransform] to it
- Change Interpolation Data Source to Predicted and set it to World Space
- Add geometry as a child
- Remove collider(s) from child and add it(them) to the parent

---

#### `Sample_Ball`
- Kinematic object

#### `Init`
- Method used toset the `life` property to be `#` seconds into the future. This is best done with the static helper method CreateFromSeconds() on the TickTimer itself.

---

<details>
<summary> Ball
</summary>

The goal is to have instances of the Ball behave identically on all peers simultaneously.
"Simultaneous" in this context means "on the same simulation tick", not the same actual world time

```cs
...
    public class Sample_Ball : NetworkBehaviour
    {
        [Networked] private TickTimer life { get; set; }

        public void Init()
        {
            life = TickTimer.CreateFromSeconds(Runner, 5f);
        }

        public override void FixedUpdateNetwork()
        {
            if (life.Expired(Runner))
                Runner.Despawn(Object);
            else
                transform.position += 5 * transform.forward * Runner.DeltaTime;
        }
    }
...
...
    public void ShootBall(Sample_NetworkInputData data)
    {
        Runner.Spawn(_ballPrefab,
                     transform.position + _forward,
                     Quaternion.LookRotation(_forward),
                     Object.InputAuthority,
                     (runner, o) => // Initialize the Ball before synchronizing it
                     {
                        o.GetComponent<Sample_Ball>().Init();
                     });
    }
...
```

</details>

---

[NetworkTransform]:<https://doc-api.photonengine.com/en/fusion/current/class_fusion_1_1_network_transform.html>