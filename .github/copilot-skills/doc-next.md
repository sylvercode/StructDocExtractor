# Skill: doc-next

**Trigger**: invoke this skill when the user says "doc-next", "document next layer", or similar.

---

## Purpose

Drive one full DOCPLAN.md **layer** of XML documentation per session.
Find the next undocumented layer, rename the session, scaffold and fill XML doc comments
for every pending file, then mark them complete in `DOCPLAN.md`.

---

## Step 1 — Find the target layer

1. Read `DOCPLAN.md`.
2. Find the **first `| [ ] |` row** in the file.
3. Identify the `## Layer N —` heading that contains that row.
4. Collect **all rows** in that layer's table where `Status = [ ]` (skip any already `[x]`).
5. Note the layer number N and the layer short name (the text after `—` in the heading).

Log to the user:
> Starting **Layer N: \<name\>** — X items pending.

If **no `[ ]` rows exist anywhere**, reply:
> 🎉 All 310 items in DOCPLAN.md are documented. Nothing left to do!

---

## Step 2 — Rename the session

Set the session title to:
```
Doc Layer N: <short layer name>
```
Examples:
- `Doc Layer 1: Core Primitives`
- `Doc Layer 4: Core Serialization`
- `Doc Layer 9: Markdown Serialization`

---

## Step 3 — Process each pending item

Work through the `[ ]` items in the layer **in order** (lowest item number first).

### 3a. Parse the row

From the DOCPLAN table row extract:
- `item_number` — the `#` column value
- `file_path` — the backtick-quoted path in the `File` column (strip backticks and `**[NEW FILE]**`)
- `one_liner` — the `One-liner` column text
- `is_new_file` — `true` if the row contains `**[NEW FILE]**`

### 3b. Classify the file

Determine the **template type** using these rules in order:

| Condition | Template |
|-----------|----------|
| `is_new_file` is true (path ends `Namespace.cs`) | `namespace` |
| File content contains `interface ` | `interface` |
| File content contains `abstract class ` | `abstract-class` |
| File content contains `\nenum ` or `\n    enum ` | `enum` |
| File content contains `record ` (as a type declaration) or `struct ` | `record` |
| File name ends with `Extensions.cs` or `Extentions.cs` | `extensions` |
| Otherwise | `class` |

For new files: classify by convention (Namespace.cs → `namespace`).
For existing files: read the file first, then classify.

### 3c. Load the template

Read `.github/copilot-skills/templates/<template-type>.md` as the documentation guide.

### 3d. Decide whether to launch a Haiku subagent

**Launch a Haiku subagent** (`claude-haiku-4.5` model) when the item meets **any** of:

- Template type is `class` or `abstract-class`
- The constructor has ≥ 2 parameters (DI-heavy)
- The `one_liner` contains any of: `orchestrat`, `pipeline`, `coordinat`, `sequenc`,
  `assembl`, `provid`, `resolv`, `register`, `drives`, `manages`, `execut`
- The file has > 6 public members

**Skip the Haiku subagent** for: `namespace`, simple `interface` (≤ 4 members), `enum`,
simple `record`/`struct`, or any file < 30 lines.

**Haiku subagent prompt template:**

> You are a code summarizer. Read the following files and write a **3–5 sentence summary**
> suitable for use as C# XML documentation context.
>
> Focus on:
> 1. What this type **does** (its primary responsibility)
> 2. **Why** it exists (what problem it solves in the pipeline)
> 3. Its main **collaborators** (types it depends on or produces)
> 4. Any **invariants** or non-obvious behaviour a doc writer should know
>
> Files to read:
> - Target: `<file_path>`
> - Interface(s) it implements (if any — check the class declaration line)
> - 1–2 closely related types (the result type it returns, or the factory/provider it uses)
>
> Respond with only the summary text; no headings or bullet points.

Wait for the Haiku agent to complete before proceeding to 3e.

### 3e. Generate XML documentation

Using ALL of:
- The **source file content** (or the target namespace for new files)
- The **template guide** from 3c
- The DOCPLAN **one-liner** for this item
- The **Haiku summary** (if generated in 3d)

Produce complete, correct C# XML doc comments following the template's rules.

**For `namespace` (new file):**
Create the file at `file_path` with this exact structure:
```csharp
namespace <NamespaceFromPath>;

// <summary>
// <one or two sentences from template guide>
// </summary>
```
Derive the namespace from the file path:
- Strip `src/` prefix and `.cs` suffix
- Replace `/` with `.`
- Drop the trailing `Namespace` segment
  - e.g. `src/Sylvercode.StructDocExtractor/Metadatas/Namespace.cs`
    → namespace `Sylvercode.StructDocExtractor.Metadatas`

**For existing files:**
Insert `///` XML doc comments immediately before each type declaration and each public member.
Do not alter any existing code. Do not remove existing comments if any exist — merge or replace
only undocumented declarations.

**Quality bar:**
- Every public type and every public member must have at least a `<summary>`.
- Use `<inheritdoc/>` for interface-implementation overrides where the summary would be identical.
- Concrete pipeline types: mention the most important injected dependency in `<remarks>` if
  it clarifies the type's role.

### 3f. Apply the changes

- **New file**: create it at the specified `file_path`.
- **Existing file**: edit the file to insert the generated XML doc comments.

### 3g. Mark the item done in DOCPLAN.md

In `DOCPLAN.md`, change the row's status cell from `[ ]` to `[x]`:
```
| N | [ ] | `...` | ... |
→
| N | [x] | `...` | ... |
```

---

## Step 4 — Layer completion report

After all items in the layer are processed, print:

```
## Layer N complete ✓

| # | File | Action |
|---|------|--------|
| 1 | src/... | documented |
| 2 | src/... | created (new file) |
...

X/X items documented. DOCPLAN.md updated.
```

Then prompt the user:
> Run **doc-next** again to start the next layer, or review the changes now.

---

## Error handling

- **File not found** for a non-new-file item: log a warning, skip the item, continue with the next.
- **Haiku subagent fails**: proceed without the summary; use only the source file and template.
- **DOCPLAN.md write conflict**: re-read the file and apply the `[x]` change fresh.

---

## Template reference

All templates live in `.github/copilot-skills/templates/`:

| File | Applies to |
|------|-----------|
| `namespace.md` | New `Namespace.cs` files |
| `interface.md` | `interface` declarations |
| `class.md` | Non-abstract, non-static classes |
| `abstract-class.md` | `abstract class` declarations (`Base*.cs`) |
| `enum.md` | `enum` declarations |
| `extensions.md` | Static extension-method classes (`*Extensions.cs`, `*Extentions.cs`) |
| `record.md` | `record`, `record struct`, `struct`, simple value carriers |
