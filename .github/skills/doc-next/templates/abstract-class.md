# Documentation template: Abstract class

Applies to any `abstract class` declaration (files typically named `Base*.cs`).

---

## Type-level comment

```xml
/// <summary>FILL</summary>
/// <typeparam name="T">FILL</typeparam>  <!-- if generic -->
/// <remarks>FILL (optional)</remarks>
```

### `<summary>`
- One sentence: *what shared concern this base class encapsulates* and *what concrete
  subclasses gain from it*.
- Pattern: *"Abstract base [for / providing] [X] to all [subclass category]."*
- Examples:
  - `"Abstract base for all node serializers, providing common dispatch and writer-access logic."`
  - `"Abstract base providing shared metadata storage and identity to all structural nodes."`

### `<remarks>` (add when useful)
- **Inheritance contract**: what a subclass *must* override vs. what it *can* override.
- Any important state shared across subclasses.
- Pipeline stage this base anchors.

---

## Constructor comments

```xml
/// <summary>Initializes a new instance of <see cref="ClassName"/>.</summary>
/// <param name="paramName">FILL</param>
```

- Use `protected` constructor phrasing when the constructor is protected.

---

## Abstract member comments

```xml
/// <summary>FILL</summary>
/// <param name="paramName">FILL</param>
/// <returns>FILL</returns>
```

- Describe **what subclasses must implement**, not how.
- Use imperative hints: *"Implement to return…"*, *"When overridden, should…"*

---

## Virtual member comments

```xml
/// <summary>FILL</summary>
```

- Note that behaviour can be overridden, and what the base implementation does (if any).
- Pattern: *"[Does X] by default; override to [provide custom Y]."*

---

## Concrete helper member comments

```xml
/// <summary>FILL</summary>
```

- Same style as the concrete-class template: verb phrase, one sentence.

---

## `<inheritdoc/>` for interface implementations

When the abstract class implements an interface and merely delegates to abstract members:

```xml
/// <inheritdoc/>
```

---

## Style rules

- Third person present tense.
- No trailing period in `<summary>`.
- `<remarks>` on the type is valuable here — use it to document the subclassing contract.
- For generic abstract classes: always document `<typeparam>` elements.
