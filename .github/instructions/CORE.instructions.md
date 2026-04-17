---
description: "Core workspace coding instructions for Wave Tower migration and refactors."
applyTo: "**/*.cs"
---

# Core Instructions

- Prioritize migration from dynamic Godot patterns to typed C# APIs.
- Keep refactors incremental and validate with a build after each micro-batch.
- Prefer C# collections and typed events over Godot dynamic containers/signals when possible.
- For dictionaries, use C# dictionaries with `using System.Collections.Generic;` and `Dictionary<TKey, TValue>`.
- Do not use `Godot.Collections.Array` in new or refactored C# code.
- When replacing array-like collections, prefer `List<T>` instead of `T[]` when the data is mutable or list-like.
- Do not use `Variant` in new or refactored C# code; prefer explicit C# types and typed models.
- Exception: use `Variant` only when required by a native Godot API boundary/interoperability call, never as a data type between project classes/nodes.
- Do not use `this` in regular member access (except when explicitly required by language constraints).
- Naming convention: public fields/properties/methods in `PascalCase`.
- Naming convention: private fields in `_camelCase`.
- Naming convention: method arguments and local variables in `camelCase`.
- Remove legacy GDScript shadowing naming from parameters/locals: do not use `p_*` prefixes or `*_p` suffixes.
- When refactoring existing code, rename `p_*` and `*_p` arguments/locals to standard `camelCase`.
