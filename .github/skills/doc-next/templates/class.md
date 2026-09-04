# Documentation template: Concrete class

Applies to any non-abstract, non-static `class` declaration.

---

## Type-level comment

```xml
/// <summary>FILL</summary>
/// <remarks>FILL (optional)</remarks>
```

### `<summary>`
- One sentence: *what this class is / does* and *where it fits in the pipeline*.
- Pattern: *"[Adjective] implementation of `IXxx` that [does Y]."*
- Or if it has no explicit interface: *"[Noun phrase] that [verb phrase]."*
- Include the most important runtime characteristic if it's not obvious (e.g., "thread-safe",
  "stateless", "immutable").

### `<remarks>` (add only when useful)
- How the class fits in its pipeline stage.
- Key collaborators injected via constructor (list the most important ones).
- Non-obvious state or lifecycle notes.

---

## Constructor comments

```xml
/// <summary>Initializes a new instance of <see cref="ClassName"/>.</summary>
/// <param name="paramName">FILL</param>
```

- Always add the standard "Initializes a new instance of…" phrasing.
- Document every constructor parameter with its **purpose**, not type.
- If the constructor just stores fields, you may omit `<remarks>`.

---

## Property comments

```xml
/// <summary>FILL</summary>
```

- "Gets …" for read-only, "Gets or sets …" for read-write.
- One sentence is almost always enough.
- Do NOT add `<value>` unless there is a meaningful distinction from `<summary>`.

---

## Method comments

```xml
/// <summary>FILL</summary>
/// <param name="paramName">FILL</param>
/// <returns>FILL</returns>
/// <exception cref="ExceptionType">FILL</exception>  <!-- if thrown -->
```

- `<summary>`: verb phrase, one sentence.
- `<param>`: purpose not type.
- `<returns>`: what is returned and nullability.
- `<exception>`: only for exceptions this *implementation* is known to throw.

---

## Override / interface-implementation members

If a member is `override` or explicitly implements an interface, prefer:

```xml
/// <inheritdoc/>
```

unless the implementation has meaningfully different behaviour worth documenting.

---

## Style rules

- Third person present tense throughout.
- No trailing period in `<summary>`.
- Do not repeat the class name in `<summary>` (it's already the context).
- For DI-heavy classes: name the most important injected dependency in `<remarks>`.
- Immutable / record-like classes: emphasise that in `<summary>` if relevant.
