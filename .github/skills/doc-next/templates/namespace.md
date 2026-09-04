# Documentation template: Namespace file (NEW FILE)

This template applies when creating a brand-new `Namespace.cs` file.
The file's only purpose is to carry a namespace-level `<summary>` XML doc comment.

---

## File structure to create

```csharp
namespace <FullNamespace>;

// <summary>
// FILL
// </summary>
```

> Use the `//` comment style (not `///`) for namespace docs — this is the correct C# syntax
> for namespace-level documentation.

---

## What to write in `<summary>`

- One or two sentences max.
- Describe **what kinds of types live in this namespace** and **what concern they serve**.
- Draw from the DOCPLAN one-liner for the namespace (it already has a good description).
- Use the pattern: *"Contains [noun phrase] that [verb phrase]."*

**Examples:**
- `"Contains immutable key/value metadata types attached to structural document nodes."`
- `"Provides contracts and base classes for the extraction pipeline task model."`
- `"Defines Markdown-specific stream writers and style management for serialization output."`

---

## Style rules

- Do **not** add `<remarks>` to namespace docs — keep it minimal.
- Sentence should read naturally without naming the namespace itself.
- Match the casing and noun style of the rest of the project (professional, concise).
