# Documentation template: Interface

Applies to any `interface` declaration (files typically named `I*.cs`).

---

## Type-level comment

```xml
/// <summary>FILL</summary>
/// <remarks>FILL (optional)</remarks>
```

### `<summary>`
- One sentence describing **the contract this interface defines**.
- Focus on *what callers use it for*, not how it is implemented.
- Pattern: *"Defines the contract for [doing X / providing Y / representing Z]."*
- Or more natural: *"Contract for [X] that [does Y]."*

### `<remarks>` (add only when truly useful)
- Non-obvious lifetime or threading constraints.
- Important invariants the contract guarantees.
- Where in the pipeline this interface sits (e.g., "Resolved per-task by `IExtractor`.").

---

## Member comments

For **every** public method, property, and event:

```xml
/// <summary>FILL</summary>
/// <typeparam name="T">FILL</typeparam>  <!-- if generic -->
/// <param name="paramName">FILL</param>  <!-- one per parameter -->
/// <returns>FILL</returns>               <!-- if non-void -->
/// <exception cref="ExceptionType">FILL</exception>  <!-- only contractual throws -->
```

### `<summary>`
- Verb phrase, one sentence. Describe *what this member does*.
- Properties: start with "Gets" (read-only) or "Gets or sets" (read-write).
- Methods: start with a verb — "Returns", "Creates", "Resolves", "Applies", "Registers".

### `<param name="…">`
- Describe **purpose**, not type. What should the caller pass?
- Bad: `"The string value."` — Good: `"The source element to discriminate against."`

### `<returns>`
- Describe **what comes back** and when it may be `null`.
- For `Task<T>` / `ValueTask<T>`: describe T, mention async.
- For `bool`: state what `true` and `false` mean.

### `<exception cref="…">`
- Only document exceptions that are **part of the contract** (guaranteed by the interface,
  not just thrown by one implementation).
- Common: `ArgumentNullException`, `KeyNotFoundException`, `InvalidOperationException`.

### `<typeparam name="…">`
- State the **constraint** and **role**: `"The type of source element being discriminated."`

---

## Style rules

- Describe the **contract**, not any specific implementation.
- Use third person present tense: "Returns the …", not "Return the …"
- No trailing periods in `<summary>` — match Roslyn/BCL style.
- Keep `<remarks>` empty if there is nothing genuinely non-obvious to say.
- Avoid restating the parameter type in `<param>`.
