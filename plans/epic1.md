# Epic 1: Define and persist extraction manifests

## Problem and approach

Issue #61 needs a durable, versioned catalog of the exact source URI to generated output-file mapping for each extraction root. Epic 1 will add that contract without loading linked mappings into the crawl graph or rewriting any current or persisted links.

The implementation will keep manifest concerns in `Sylvercode.SiteExtractor`, use a strict JSON file at each output root, and make `SiteExtractor` validate all configured linked roots before crawling. After all normal continuation/serialization work completes, it will build the current manifest from only resources that actually produced output and persist it to the current root. A normal re-extraction is the legacy migration workflow: once it completes, the existing output root gains its first valid manifest.

## Proposed durable contract

- Add `LinkedExtraction` configuration entries to `SiteExtractorOptions`, each containing an `OutputDirectory`. The output directory is the root from which the manifest is read.
- Use `extraction-manifest.json` as the reserved manifest filename directly under every extraction output root.
- Use schema version `1` with:
  - `schemaVersion`
  - `sourceBaseUri`
  - `entries`, where each entry contains `sourceUri` and `outputFile`
- Persist `sourceBaseUri` and each `sourceUri` as absolute, fragment-free URIs. Keep query strings because they are part of the source identity.
- Persist `outputFile` as a normalized, portable relative path under the output root using `/` separators. Absolute paths, `.`/`..` traversal, empty paths, and the reserved manifest filename are invalid.
- Keep the JSON deterministic (stable entry ordering and no generated timestamp) so unchanged extractions produce equivalent manifests.
- Treat an unsupported schema version, missing/unknown required fields, malformed JSON, malformed URIs, duplicate source/output mappings, or output paths escaping their root as explicit manifest errors containing the affected root and useful field/error context.

## Todos

1. **Defining the manifest and linked-root configuration model**
   - Add `ExtractionManifest` and `ExtractionManifestEntry` immutable/public contract types under the SiteExtractor core.
   - Add a linked extraction options type and collection to `SiteExtractorOptions`; preserve existing `SourceAuthority`, `SourceBasePath`, and `OutputDirectory` behavior.
   - Add validation helpers for normalized source URIs, source-base containment, canonical output roots, and portable relative output paths.
   - Define a specific public exception hierarchy or exception messages for configuration errors versus unreadable/invalid manifests.

2. **Implementing strict manifest persistence**
   - Add an `IExtractionManifestStore` abstraction that can load a manifest for an arbitrary output root and write the current manifest.
   - Add a JSON file implementation using `System.Text.Json` with camel-case properties, strict unmapped-member handling, and explicit schema-version validation.
   - Write atomically with a same-directory temporary file followed by replacement; clean up a temporary file only when it was created by the operation.
   - Load linked manifests without directory inference or fallback scanning: a configured root without `extraction-manifest.json` fails immediately.
   - Provide a test/in-memory implementation as needed so core orchestration tests do not depend on disk access.
   - Register the correct store from the existing data-store DI extensions (`AddDirectoryDataStore` and `AddMemoryDataStore`) and use `TryAdd`-style registration so callers can replace it.

3. **Recording actual output provenance**
   - Extend `Resource` with an explicit output marker and/or finalized output accessor, rather than inferring output from pull state or URI inequality.
   - Mark a resource as output-producing only when `SiteExtractor` accepts a non-null `ResourceUriTranslaterToSet` from a processor.
   - Preserve the final translated URI after metadata is copied, including heading-derived Markdown names, and make unsuccessful/no-node processing leave no manifest entry.
   - Ensure copied assets and serialized pages are both represented, while ordinary external/non-pullable resources and failed/no-output resources are excluded.

4. **Adding manifest orchestration to extraction**
   - At the start of `SiteExtractor.Extract`, resolve and validate the current source/output roots and load every configured linked manifest.
   - Reject canonical duplicate output roots, current/linked root equality or nesting, duplicate/overlapping source bases, and duplicate source/output mappings across the current configuration and loaded linked manifests before the first resource is processed.
   - Retain loaded manifests in the manifest/catalog service for Epic 2, but do not seed them into `ResourcesTracker` in Epic 1.
   - After the initial resource queue and all continuation batches finish successfully, build entries from output-producing resources using their finalized translated paths.
   - Persist the new current manifest only after every file has been written successfully. If processing or validation fails, do not create or replace the manifest.
   - Do not add post-extraction link rewriting, persisted Markdown editing, or linked-resource download suppression in this epic.

5. **Covering manifest behavior with focused tests**
   - Add unit tests for schema round-trip, deterministic ordering, unsupported versions, malformed/missing JSON, unknown fields, invalid URIs, fragments, path escape/absolute output paths, reserved filenames, and duplicate entries.
   - Add tests for missing linked manifests, invalid linked manifests, duplicate linked roots, overlapping roots/source bases, and useful root-specific errors.
   - Add orchestration tests proving:
     - the current manifest is not written before unfinished continuations complete;
     - heading-derived output names are captured exactly;
     - pages and copied assets are included;
     - no-output and non-pullable resources are excluded;
     - failed extraction leaves an existing manifest untouched;
     - a successful re-extraction creates a manifest in a legacy output root without a manual mapping.
   - Prefer temporary directories for file-store integration tests and the in-memory manifest store for pipeline unit tests.

6. **Documenting compatibility and migration**
   - Update the SiteExtractor README and architecture/configuration sections with the manifest location/schema, linked-root configuration, strict failure policy, and exact output-path semantics.
   - Document the one-time migration: re-run extraction against each legacy output directory with a compatible configuration; the successful run writes the required manifest, and no manual mapping or legacy scan occurs.
   - Keep cross-root link usage out of the Epic 1 docs so it is not advertised before Epics 2 and 3 are complete.

7. **Verifying Epic 1**
   - Run focused SiteExtractor tests while iterating.
   - Run `dotnet test references/StructDocExtractor/tests/Sylvercode.SiteExtractor.Tests/Sylvercode.SiteExtractor.Tests.csproj`.
   - Run `dotnet test references/StructDocExtractor/StructDocExtractor.sln`.
   - Run `dotnet build DDBMarkdown.sln` and `dotnet test DDBMarkdown.sln` to verify reference integration.

## Important boundaries

- Epic 1 may validate and retain loaded linked catalogs, but it must not use them to prevent downloads, seed resources, resolve forward links, select short names, or update old documents.
- The manifest is the exact source-to-output truth; filenames are never reconstructed from headings during loading.
- A missing/invalid linked manifest is always fatal. There is no silent scan, inference, or automatic re-crawl.
- No timestamp, process ID, or machine-specific absolute output path belongs in schema version 1.

## Notes and trade-offs

- A dedicated manifest store is preferable to extending `IDataStore` with read/list semantics because linked roots live outside the active data store and atomic manifest replacement is a different persistence concern.
- Explicit output provenance avoids incorrectly publishing resources that were discovered, attempted, or non-pullable but never generated a file.
- Rejecting overlapping roots/source bases and duplicate target files is intentionally conservative; it prevents ambiguous bidirectional linking in later epics before files are changed.
- A successful ordinary extraction is the migration mechanism. A separate migration command would duplicate the crawl path and is outside Epic 1.
