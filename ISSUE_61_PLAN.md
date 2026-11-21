# High-level solution: cross-extraction links

## Problem

Issue #61 in `sylvercode/StructDocExtractor` requires bidirectional local linking across separate extraction roots:

- Links in a new extraction must resolve to files in previously extracted roots.
- Links in previously extracted files that point to the new extraction's source URLs must be rewritten to the new local files.

The existing implementation only knows resources discovered during the current crawl. It does not persist an exact source-URI-to-output-file map, and output filenames can be derived from page headings.

## High-level solution

Use a versioned extraction manifest for every output root.

The manifest is the persisted source of truth for:

- The source base URI represented by the output root.
- Every extracted source URI.
- The exact output file for each source URI.
- Schema/version information needed for safe evolution.

A new extraction will:

1. Validate configured linked extraction roots and load their manifests.
2. Merge linked manifest entries with resources discovered during the current run.
3. Rewrite links in newly generated content against that combined catalog.
4. Write the current output root's new manifest once output paths are final.
5. Run a format-specific persisted-link updater over linked roots so old documents can point to the current output.

The framework core remains format-independent. Source/output mapping, validation, and orchestration belong to `Sylvercode.SiteExtractor`; persisted document editing belongs to the relevant output integration, initially `Sylvercode.SiteExtractor.Markdown`.

## Confirmed compatibility policy

- A configured linked root must have a valid manifest.
- Missing or invalid manifests fail explicitly.
- Legacy roots are migrated by re-extracting them once to generate the manifest.
- No silent directory inference, best-effort legacy scan, or implicit re-crawl is part of the solution.

## Epic map

The epics are intentionally ordered by dependency. Each epic will receive its own later analysis and detailed implementation plan.

### Epic 1 — Define and persist extraction manifests

**Goal**

Establish the durable contract that maps an extracted source site to exact generated files.

**In scope**

- Linked extraction configuration model.
- Manifest schema and schema versioning.
- JSON persistence for an output root.
- Strict validation for source bases, output roots, duplicate/overlapping mappings, and malformed manifests.
- Writing the current extraction's manifest after output paths are final.
- One-time migration workflow for legacy outputs.

**Out of scope**

- Loading manifests into the crawl resource graph.
- Rewriting any links.
- Parsing or editing existing Markdown files.

**Dependencies**

- None.

**Epic acceptance criteria**

- A completed extraction writes a valid manifest with exact source URI → output file entries.
- A configured linked root with a missing or invalid manifest fails with a useful error.
- Re-extracting a legacy root creates a manifest without requiring manual mapping.
- Manifest behavior is covered by focused unit/integration tests.

### Epic 2 — Integrate linked resources and forward-link rewriting

**Goal**

Make a current extraction reference already extracted content as locally available files.

**In scope**

- Load linked manifests before crawling.
- Seed exact linked mappings as locally available, non-pullable resources.
- Prevent the crawler from downloading resources already supplied by linked manifests.
- Distinguish linked/local, current-pullable, and ordinary external/unavailable resources.
- Resolve cross-root relative links.
- Preserve fragments and existing query behavior.
- Consider all linked and current resources when choosing Markdown wiki-link short names, so duplicate filenames do not become ambiguous.

**Out of scope**

- Updating files in linked roots.
- Markdown parsing of persisted files.

**Dependencies**

- Epic 1.

**Epic acceptance criteria**

- A current page linking to a source URI in a linked manifest is emitted with a working local link.
- Ordinary unconfigured external URLs remain unchanged.
- Fragment references continue to work.
- Duplicate filenames across roots produce path-qualified, unambiguous links.
- Linked resources are not fetched by the current run.

### Epic 3 — Update persisted links in previous extractions

**Goal**

Rewrite references in previously generated files when they point to source URLs represented by the current extraction.

**In scope**

- Format-independent post-extraction hook and persisted-reference updater contract.
- Markdown implementation for changing actual link and image destinations only.
- Cross-root relative path generation.
- Source-preserving updates so unrelated Markdown, code, prose, and front matter remain unchanged.
- Atomic file updates and explicit file-level errors.
- Idempotent behavior when a link is already local.

**Out of scope**

- Configuring or generating manifests.
- Forward rewriting during current extraction.
- Automatic handling of arbitrary output formats beyond the extension point; Markdown is the first implementation.

**Dependencies**

- Epic 1 for the current and linked manifests.
- Epic 2 for consistent cross-root target resolution and local-link rules.

**Epic acceptance criteria**

- An old Markdown page containing a link to a newly extracted source URI is updated to the new local file.
- Only link/image destinations are changed; surrounding content is preserved.
- Fragments survive.
- Re-running produces the same result.
- A partial or failed write does not corrupt an existing file.

### Epic 4 — End-to-end orchestration, CLI, and documentation

**Goal**

Expose and validate the complete cross-extraction workflow through the real application.

**In scope**

- Order the phases as: load linked catalogs → crawl/current mappings → forward rewrite → write manifest → persisted reverse rewrite.
- Expose linked extraction configuration to the DDB Markdown CLI as needed.
- End-to-end tests covering two roots that reference each other in both directions.
- Operational diagnostics for manifest loading, skipped external references, and persisted file updates.
- Update README and architecture documentation.
- Validate the reference `StructDocExtractor` solution and the parent `DDBMarkdown` solution.

**Out of scope**

- New extraction formats unrelated to Markdown.
- Changes to site-specific DDB extraction semantics unless required by the integration.

**Dependencies**

- Epics 1–3.

**Epic acceptance criteria**

- A two-root scenario matching issue #61 succeeds end-to-end.
- The documented migration/configuration sequence is sufficient for a user to reproduce it.
- Focused and full test suites pass in both solutions.
- Logs make manifest and link-update failures diagnosable without silent success.

## Cross-epic implementation principles

- Prefer new providers/interfaces over editing the core crawl loop where possible.
- Keep persisted-file updates atomic and fail explicitly.
- Do not treat linked resources as ordinary external dependencies.
- Preserve fragments and URI equality semantics consistently.
- Keep exact output mapping in manifests; do not infer heading-derived filenames.
- Add focused tests within each epic before moving to the next one.

## Suggested execution sequence

1. Epic 1 analysis and detailed plan.
2. Epic 1 implementation and verification.
3. Epic 2 analysis, plan, and implementation.
4. Epic 3 analysis, plan, and implementation.
5. Epic 4 analysis, plan, integration, and full validation.
