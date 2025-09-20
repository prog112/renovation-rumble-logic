# Renovation Rumble Logic

This project is part of a larger game, *Renovation Rumble*, where players place furniture items on a grid to maximize surface coverage and score points through hidden combos, style affinity, and other placement rules.

I'm building this module to showcase a clean, modular, and deterministic game logic architecture that I use in production.

The project is split into three repositories:

- `renovation-rumble-logic` - engine-agnostic logic library **← you are here!**
- `renovation-rumble-unity` - game client built in Unity
- `renovation-rumble-backend` - Node.js backend for async multiplayer and score verification

This is a work in progress. The Unity and backend repos are still under active development.

---

## Project Goals

- **Determinism**  
  All logic should be fully deterministic and replayable. Given the same command sequence, results should always be identical, making it perfect for verification, replays, or multiplayer.

- **Modularity & Extensibility**  
  The architecture allows mechanics to change without impacting the high-level flow. You can easily add or replace command types, executors, board rules, etc. without refactoring the core loop.

- **Data-Driven Design**  
  Piece definitions and shape data are authored in Unity via ScriptableObjects and exported into clean `*.json` data for use by the logic layer. This gives a clear separation between runtime simulation and authoring tools.

- **Testability**  
  A core goal was to write the game logic in a form that can be easily unit tested, independent of Unity. For this, I'm using `xUnit` with many tests already covering key parts like custom data structures, command serialization, and gameplay validation.

- **Maintainability & Developer Experience**  
  Source generators are used to eliminate boilerplate while keeping things type-safe and reflection-free for IL2CPP compatibility.

---

## Core Systems

### Commands & Executors

Commands like `Place`, `Grow`, or `Shrink` are modeled as polymorphic data objects inheriting from `CommandDataModel`. Each is handled by an `ICommandExecutor<T>` implementation that:

- First checks `CanApply` using a readonly view of game state (`ReadOnlyContext`)
- Then mutates state in `Apply`

This double-layer helps separate validation from mutation.

### CommandRunner & GameRunner

The `GameRunner` is the de-facto starting point, holding a simulation context and delegateing all command execution. Game clients or backend services can simply instantiate a runner instance to start simulating a match. The commands themselves pass then through a `CommandRunner` which resolves the proper executor per type, automatically hooked up via a Roslyn source generator.

### Validation
To showcase a practical application of the logic layer, this repo includes a minimal .NET web service `RenovationRumble.Verifier` that exposes a /verify HTTP endpoint. It takes a match state, a list of commands, and a claimed score, then replays the simulation using the same logic as the client to verify the result.

The service returns a structured response indicating whether the match ended correctly, if the score is valid, or if any commands failed. It's stateless, fast, and designed to be run alongside a backend written in any language (`renovation-rumble-backend` will use Node.js), making it easy to plug into real-world async multiplayer flows or anti-cheat systems.

### BitMatrix

To make the logic *a bit* (ha!) more interesting, the project introduces a custom `BitMatrix` type: a tightly-packed struct representing shape patterns on a 2D board. It uses a 64-bit ulong to store up to an 8×8 grid in row-major order which results in zero heap allocations and fast bitwise operations.

It supports efficient transformations like `Rotate`, `Shrink`, `Grow`, and exposes a custom `FilledCells()` struct enumerator for iterating only over the filled cells (without generating any garbage!). This lets command executors check collisions, bounds, and apply effects without having to materialize arrays or allocate memory.

On the Unity side, these shapes are authored using a custom visual editor tool, but at runtime everything boils down to raw bits.

### Serialization

- Uses `Newtonsoft.Json` with a custom `EnumDiscriminatedConverter<TBase, TEnum>` to handle polymorphism safely (no `TypeNameHandling`).
- The type mappings are auto-generated from annotated discriminated unions.
- The result is a fast, IL2CPP-safe, and reflection-free serialization pipeline.

---

## Closing Thoughts

While the foundation is already set, I have plans for many further additions, e.g.:

- Special tile mechanics and combos
- Win condition evaluation
- Effects, buffs, and status stored in `GameState`

All in all, the aim of this is to embody my core architecture philosophy:

> A strong architecture is one where the key components, ones at the top of the dependency hierarchy, stay relatively untouched even as gameplay evolves.  
> Construct your code so that volatile gameplay code sits on the leaves, not the roots, and it'll pay off tenfold.

This project is built around that principle.

Thanks for reading!