# Documentation template: Enum

Applies to any `enum` declaration.

---

## Type-level comment

```xml
/// <summary>FILL</summary>
/// <remarks>FILL (optional)</remarks>
```

### `<summary>`
- One sentence: *what concept this enum represents* and *where it is used*.
- Pattern: *"Defines the possible [noun] values for [context]."*
- Or: *"Specifies [what] in the [pipeline stage / component]."*
- Examples:
  - `"Defines the possible outcomes of an extraction task."`
  - `"Specifies the indentation strategy used by the serialization writer."`

### `<remarks>` (rarely needed)
- Add only if the enum has non-obvious semantics (e.g., bitmask flags, ordering constraints).

---

## Member (value) comments

Add a `<summary>` to **every** enum value:

```xml
/// <summary>FILL</summary>
ValueName,
```

### `<summary>` per value
- One sentence: *what this value means in practice*.
- Be specific — avoid just paraphrasing the name.
- Good: `"The task completed successfully and produced a node."`
- Bad: `"Success value."`
- For a "None" or default zero value: `"No [X] specified; the default state."`

---

## `[Flags]` enums (if applicable)

If the enum has `[Flags]`, note in `<remarks>`:
- That values can be combined with bitwise OR.
- What `0` / "None" means.

---

## Style rules

- Third person present tense.
- No trailing period in `<summary>`.
- Do not start with "The" — start with the noun or a verb phrase.
  - Bad: `"The task succeeded."` → Good: `"Indicates the task succeeded and produced a node."`
