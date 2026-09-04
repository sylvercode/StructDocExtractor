# Documentation template: Record / Value type / Struct

Applies to `record`, `record struct`, `struct`, and simple immutable class declarations
that act as value/data carriers with no significant behaviour.

---

## Type-level comment

```xml
/// <summary>FILL</summary>
/// <typeparam name="T">FILL</typeparam>  <!-- if generic -->
/// <remarks>FILL (optional)</remarks>
```

### `<summary>`
- One sentence: *what data this type carries* and *where it is used*.
- Pattern: *"Immutable [noun] carrying [fields/properties] used by [context]."*
- Or: *"Represents [noun phrase] in the [pipeline stage]."*
- Examples:
  - `"Immutable key/value pair representing a single metadata entry attached to a node."`
  - `"Specifies the indent type and character count used by the indented stream writer."`
  - `"Positional index of a task within a sibling task sequence."`

### `<remarks>` (rarely needed for records)
- Add if there is a non-obvious equality semantic or a special factory pattern.

---

## Primary constructor / positional parameters

For positional records, document via `<param>` on the type:

```xml
/// <summary>FILL</summary>
/// <param name="field1">FILL</param>
/// <param name="field2">FILL</param>
public record MyRecord(Type1 field1, Type2 field2);
```

---

## Property / field comments

```xml
/// <summary>FILL</summary>
```

- "Gets …" for read-only init properties.
- "Gets or sets …" for mutable properties.
- One sentence is enough — value types are usually self-explanatory.

---

## Method comments (if any)

Same rules as the concrete class template:
- `<summary>`, `<param>`, `<returns>`, `<exception>` as applicable.
- Conversion operators / factory methods: describe *what the result represents*.

---

## Style rules

- Third person present tense.
- No trailing period in `<summary>`.
- Lean on the DOCPLAN one-liner — it often provides the clearest description.
- For structs with operator overloads: document each operator with what it tests/produces.
