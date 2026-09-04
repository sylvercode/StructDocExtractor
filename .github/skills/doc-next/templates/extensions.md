# Documentation template: Extension methods class

Applies to `static class` files whose purpose is to provide extension methods
(file names typically end in `Extensions.cs` or `Extentions.cs`).

---

## Type-level comment

```xml
/// <summary>FILL</summary>
```

### `<summary>`
- One sentence: *what type(s) this class extends* and *what the extensions help with*.
- Pattern: *"Provides extension methods for `TypeName` to [do X]."*
- Or: *"Extension helpers for [registering / composing / building] [Y] in [context]."*
- Examples:
  - `"Provides extension methods for <see cref=\"IUriRedirector\"/> to register and compose redirect rules."`
  - `"Extension helpers for registering <see cref=\"MarkdownStreamWriterProvider\"/> in a DI container."`

> Use `<see cref="…"/>` to cross-reference the extended type.

---

## Method comments

Each extension method gets full documentation:

```xml
/// <summary>FILL</summary>
/// <param name="this">FILL (the extended object)</param>  <!-- or omit if obvious -->
/// <param name="paramName">FILL</param>
/// <returns>FILL</returns>
/// <exception cref="ExceptionType">FILL</exception>  <!-- if applicable -->
```

### `<summary>`
- Verb phrase, one sentence — same as any method.
- May describe the **fluent** or **builder** pattern if applicable:
  *"Registers `XyzProvider` in `services` and returns `services` for chaining."*

### `<param name="this">` (the extended type parameter)
- For DI builder extensions, use: `"The <see cref=\"IServiceCollection\"/> to add services to."`
- For fluent builder extensions: `"The builder to configure."`
- May be omitted if obvious from the summary.

### `<returns>`
- For builder/DI fluent extensions: *"The same `services` instance for chaining."*
- For query extensions: describe what is returned.

---

## Style rules

- Use `<see cref="…"/>` generously to link related types.
- Third person present tense.
- No trailing period in `<summary>`.
- For DI registration methods, always mention what is registered.
- For helper/utility extensions, focus on *what is computed or produced*.
