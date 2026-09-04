# Documentation Plan

This file tracks the XML documentation progress for the entire project.
Items are ordered from least to most dependent (foundations first, integrations last).
Each `Namespace.cs` entry is a **new file** to be created holding namespace-level `<summary>` docs.

Legend: `[ ]` = pending · `[x]` = done · **[NEW FILE]** = file to create

---

## Layer 1 — Core Primitives: `Sylvercode.StructDocExtractor` model & metadata

These types have no intra-project dependencies and form the vocabulary of the whole system.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 1 | [ ] | `src/Sylvercode.StructDocExtractor/Metadatas/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Metadatas` — key/value metadata attached to nodes. |
| 2 | [ ] | `src/Sylvercode.StructDocExtractor/Metadatas/Metadata.cs` | Immutable key/value pair representing a single metadata entry. |
| 3 | [ ] | `src/Sylvercode.StructDocExtractor/Metadatas/MetadataDictionary.cs` | Dictionary of `Metadata` entries keyed by name, attached to structural nodes. |
| 4 | [ ] | `src/Sylvercode.StructDocExtractor/Metadatas/StdMetadata.cs` | Constants for well-known metadata key names used across the extractor pipeline. |
| 5 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Model` — core structural document node contracts. |
| 6 | [ ] | `src/Sylvercode.StructDocExtractor/Model/IStructDocNode.cs` | Base contract for every node in the structured document tree. |
| 7 | [ ] | `src/Sylvercode.StructDocExtractor/Model/IStructDocBlock.cs` | Marker contract identifying block-level structural document nodes. |
| 8 | [ ] | `src/Sylvercode.StructDocExtractor/Model/IStructDocNodeHolder.cs` | Contract for nodes that hold an ordered collection of child `IStructDocNode`s. |
| 9 | [ ] | `src/Sylvercode.StructDocExtractor/Model/IStructDocReferencer.cs` | Contract for nodes that carry a URI reference (links, images, etc.). |
| 10 | [ ] | `src/Sylvercode.StructDocExtractor/Model/StructDocRootBlock.cs` | Concrete root block node that is the top-level container of a structured document. |
| 11 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Model.Base` — abstract base classes for node types. |
| 12 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocNode.cs` | Abstract base providing identity and metadata storage for all structural nodes. |
| 13 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocBlock.cs` | Abstract base for block-level nodes, implementing `IStructDocBlock`. |
| 14 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocBlockWithAnyContent.cs` | Abstract base for block nodes that accept any content type as children. |
| 15 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocBlockWithAnyParent.cs` | Abstract base for block nodes that can live under any parent node. |
| 16 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocBlockWithAnyParentAndContent.cs` | Abstract base for block nodes with unrestricted parent and content constraints. |
| 17 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocRootBlock.cs` | Abstract base for root-level container block nodes. |
| 18 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Base/BaseStructDocRootBlockWithAnyContent.cs` | Abstract base for root blocks that accept any child content type. |
| 19 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Model.Init` — initializers that link parent/child relationships. |
| 20 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/IStructDocNodeInitializer.cs` | Contract for initializing a single structural node after construction. |
| 21 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/IStructDocNodeInitializerExtensions.cs` | Extension helpers that apply `IStructDocNodeInitializer` to collections. |
| 22 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/IStructDocNodeHolderInitializer.cs` | Contract for initializing a node holder and wiring its children. |
| 23 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/IStructDocNodeHolderInitializerExtetnion.cs` | Extension helpers for `IStructDocNodeHolderInitializer` bulk initialization. |
| 24 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/IParentChildLinkInitializer.cs` | Contract for setting up bi-directional parent/child links between nodes. |
| 25 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Init/ParentChildLinkInitializer.cs` | Default implementation that establishes parent/child links during tree construction. |
| 26 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Utils/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Model.Utils` — utilities for traversing the node tree. |
| 27 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Utils/ISrcNodeStack.cs` | Contract for a stack that tracks source nodes during tree traversal. |
| 28 | [ ] | `src/Sylvercode.StructDocExtractor/Model/Utils/SrcNodeStack.cs` | Stack implementation that maintains source-node context during document traversal. |

---

## Layer 2 — Core Extraction: interfaces & task model

Abstractions for the extraction pipeline; no implementations yet.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 29 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Extraction` — orchestration of node extraction tasks. |
| 30 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/IExtractionTask.cs` | Contract for a single unit of work that extracts a node from source data. |
| 31 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/IExtractor.cs` | Top-level contract for running an extraction pass on a document. |
| 32 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/IProcessTaskResult.cs` | Contract for the outcome returned by a processed extraction task. |
| 33 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/IExtractorTaskSequencerHandler.cs` | Contract for handling sequenced task execution in the extractor pipeline. |
| 34 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/IRouterExtractorSelector.cs` | Contract for selecting the appropriate extractor from a router based on the task. |
| 35 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/IRouterExtractorSelectorProvider.cs` | Contract for providing `IRouterExtractorSelector` instances. |
| 36 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskResultType.cs` | Enum of possible task result types (e.g., Success, Skip, Abort). |
| 37 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Extraction.Factory` — factory contracts for node creation. |
| 38 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/IStructDocNodeFactory.cs` | Contract for creating a `IStructDocNode` from a source element and task context. |
| 39 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/IStructDocNodeFactoryProvider.cs` | Contract for resolving the correct `IStructDocNodeFactory` for a given source element. |
| 40 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/IStructDocNodeFactoryProviderStack.cs` | Contract for a stack of factory providers enabling layered factory resolution. |
| 41 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/IDataDiscriminatorFactory.cs` | Contract for discriminating which factory should handle a given source data element. |
| 42 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/IChildrenTaskInfoFactory.cs` | Contract for creating child-task descriptors from a parent extraction result. |
| 43 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/PreviewProvider/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Extraction.PreviewProvider` — preview text generation. |
| 44 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/PreviewProvider/IDataPreviewProvider.cs` | Contract for generating a human-readable preview string from source data. |
| 45 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/PreviewProvider/StringPreviewProvider.cs` | Preview provider that returns a string source value directly as the preview. |
| 46 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/PreviewProvider/ToStringPreviewProvider.cs` | Preview provider that calls `ToString()` on the source data to produce a preview. |
| 47 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Extraction.TaskInfo` — descriptors for child task dispatch. |
| 48 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/IChildrenTaskInfo.cs` | Contract describing how child tasks should be created from an extracted parent node. |
| 49 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/ChildrenTaskInfo.cs` | Standard children-task descriptor carrying child source data for further extraction. |
| 50 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/NodeHolderChildrenTaskInfo.cs` | Children-task descriptor that targets a node holder's children list. |
| 51 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/ParentTaskInfo.cs` | Carries parent-context information that child extraction tasks may need. |
| 52 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/ProxyChildrenTaskInfo.cs` | Children-task descriptor that proxies another descriptor to override behaviour. |
| 53 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskInfo/TaskIndex.cs` | Represents the positional index of a task within a sibling task sequence. |

---

## Layer 3 — Core Extraction: implementations & task orchestration

Concrete pipeline logic that builds on the Layer 2 contracts.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 54 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ExtractionTask.cs` | Concrete extraction task carrying source data, context, and target node reference. |
| 55 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ExtractionResult.cs` | Outcome of a completed extraction task, including the produced node. |
| 56 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ExtractionTaskResult.cs` | Wraps an `ExtractionResult` together with its task result type. |
| 57 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/TaskContext.cs` | Immutable context passed through the extraction task chain (depth, parent info, etc.). |
| 58 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ProcessTaskResult.cs` | Default implementation of `IProcessTaskResult` returned by factory-based extraction. |
| 59 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ProcessTaskResultBuilder.cs` | Fluent builder for constructing `ProcessTaskResult` instances. |
| 60 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ExtractorOption.cs` | Configuration options that control extractor behaviour (depth limits, etc.). |
| 61 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Extractor.cs` | Main implementation of `IExtractor` — drives the recursive extraction pipeline. |
| 62 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ExtractorTaskSequencer.cs` | Sequences extraction tasks in breadth/depth order, calling sequencer handlers. |
| 63 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/BaseTaskSequencerHandler.cs` | Abstract base for `IExtractorTaskSequencerHandler` providing shared dispatch logic. |
| 64 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/ExtractorTaskSequencerHandler.cs` | Default sequencer handler that processes each task and enqueues children. |
| 65 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/FactoryProviderStackByTask.cs` | Builds and exposes the factory-provider stack scoped to a specific task. |
| 66 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/RouterExtractor.cs` | Extractor that routes each task to a sub-extractor selected by a selector. |
| 67 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/RouterExtractorList.cs` | Ordered list of router extractors consulted in priority order. |
| 68 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/RouterExtractorExtension.cs` | Extension methods for registering and composing router extractors. |
| 69 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/StructDocNodeFactoryProvider.cs` | Default `IStructDocNodeFactoryProvider` resolving factories by element type. |
| 70 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/ExtractionTaskFactory.cs` | Creates `IExtractionTask` instances for child nodes discovered during extraction. |
| 71 | [ ] | `src/Sylvercode.StructDocExtractor/Extraction/Factory/ChildrenTaskInfoFactory.cs` | Default `IChildrenTaskInfoFactory` that produces child-task info from node children. |

---

## Layer 4 — Core Serialization

Serialization pipeline built on top of the core model.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 72 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Serialization` — serialization pipeline for structural documents. |
| 73 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/ISerializer.cs` | Base contract for all node serializers in the pipeline. |
| 74 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/ISerializerProvider.cs` | Contract for resolving the correct `ISerializer` for a given node type. |
| 75 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IStructDocNodeSerializer.cs` | Contract for a serializer that operates on a single `IStructDocNode`. |
| 76 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IStructDocNodeHolderSerializer.cs` | Contract for a serializer that also serializes a node's children. |
| 77 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IStructDocSerializer.cs` | Top-level contract for serializing an entire structured document tree. |
| 78 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/ITextWriterAdapterProvider.cs` | Contract for providing a `TextWriter` adapter used during serialization output. |
| 79 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IndentType.cs` | Enum of indentation strategies (spaces, tabs, none) for the indented writer. |
| 80 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IndentSpec.cs` | Value type specifying indent type and size used by `IndentedStreamWriter`. |
| 81 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IndentedStreamWriter.cs` | `TextWriter` decorator that prepends configurable indentation on each new line. |
| 82 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IndentedSerializerHandler.cs` | Serializer handler that manages indentation depth as nodes are serialized. |
| 83 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IndentedStreamWriterProvider.cs` | Provides `IndentedStreamWriter` instances scoped to a serialization pass. |
| 84 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/IndentedStreamWriterProviderExtensions.cs` | DI/builder extensions for registering `IndentedStreamWriterProvider`. |
| 85 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/NodeSerializationResult.cs` | Holds the textual result produced by serializing a single node. |
| 86 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/SerializerTask.cs` | Describes a pending serialization unit (node + writer + parent context). |
| 87 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/SerializerTaskParentInfo.cs` | Carries parent-context data needed by a serializer task. |
| 88 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/SerializerProvider.cs` | Default `ISerializerProvider` that resolves serializers by exact or base node type. |
| 89 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/SerializerExtansions.cs` | Extension helpers for invoking serializers on nodes and collections. |
| 90 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/StringTextWriter.cs` | In-memory `TextWriter` adapter that accumulates output in a `StringBuilder`. |
| 91 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/BaseStructDocNodeSerializer.cs` | Abstract base providing common dispatch logic for all node serializers. |
| 92 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/BaseStructDocNodeHolderSerializer.cs` | Abstract base for holder serializers that recurse into child node serialization. |
| 93 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/StructDocSerializer.cs` | Main `IStructDocSerializer` implementation that drives the serialization pipeline. |
| 94 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/StructDocSerializerExecutor.cs` | Executes a serializer task queue, dispatching tasks to the resolved serializers. |
| 95 | [ ] | `src/Sylvercode.StructDocExtractor/Serialization/Init/IStructDocSerializerServiceInit.cs` | Contract for service-initialisation hooks run before the serializer pipeline starts. |

---

## Layer 5 — Core StructDataStack (scoring/matching)

Scoring machinery used by both StdHtml and AngleSharp layers.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 96 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.StructDataStack` — stack-based structural data selection. |
| 97 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/IStructDataStack.cs` | Contract for a stack of structural data nodes used during discriminated extraction. |
| 98 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/BaseStructDataStack.cs` | Abstract base stack implementation providing push/pop and access to stacked entries. |
| 99 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.StructDataStack.Score` — score-based node selection criteria. |
| 100 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/IValueMatcher.cs` | Contract for testing whether a given value matches a criterion. |
| 101 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/StaticValueMatcher.cs` | Matcher that compares a value against a fixed constant string. |
| 102 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/RegexValueMatcher.cs` | Matcher that tests a value against a compiled regular expression. |
| 103 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/ValuesMatcherAll.cs` | Composite matcher that requires all inner matchers to pass (logical AND). |
| 104 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/ValuesMatcherAny.cs` | Composite matcher that passes when at least one inner matcher passes (logical OR). |
| 105 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/NodeScoreCriteriaSet.cs` | Defines a named set of criteria whose cumulative score can be evaluated on a node. |
| 106 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/NodeScore.cs` | Stores the computed score for a node against a criteria set. |
| 107 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/StackedNodesScore.cs` | Aggregated score across all stacked nodes for a criteria set evaluation. |
| 108 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/IStackScoreCalculator.cs` | Contract for calculating the aggregate score of a node stack against criteria. |
| 109 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/StackScoreCalculator.cs` | Default `IStackScoreCalculator` implementation summing individual node scores. |
| 110 | [ ] | `src/Sylvercode.StructDocExtractor/StructDataStack/Score/BaseScoreCriteriaSetsBuilder.cs` | Abstract builder for constructing a collection of `NodeScoreCriteriaSet` instances. |

---

## Layer 6 — StdHtml: standard HTML document model nodes

HTML-specific node types built on the core model; no extraction logic yet.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 111 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.StdHtml.Model` — concrete HTML structural node types. |
| 112 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/PlainTextNode.cs` | Node representing raw inline text content without markup. |
| 113 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlNodeDiscriminator.cs` | Value type carrying the HTML tag name and CSS classes used to discriminate factory selection. |
| 114 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlHeading.cs` | Node representing an HTML heading element (`h1`–`h6`) with its level. |
| 115 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlParagraph.cs` | Node representing an HTML `<p>` paragraph element. |
| 116 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlAnchor.cs` | Node representing an HTML `<a>` anchor element with its `href` URI. |
| 117 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlImg.cs` | Node representing an HTML `<img>` element with its `src` URI and alt text. |
| 118 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlBr.cs` | Node representing an HTML `<br>` line-break element. |
| 119 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlEmphases.cs` | Node representing an HTML `<em>` emphasis element. |
| 120 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlStrong.cs` | Node representing an HTML `<strong>` bold element. |
| 121 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlList.cs` | Node representing an HTML `<ul>` or `<ol>` list container. |
| 122 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlListItem.cs` | Node representing an HTML `<li>` list item. |
| 123 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlDiv.cs` | Node representing an HTML `<div>` generic block container. |
| 124 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlAside.cs` | Node representing an HTML `<aside>` supplemental content block. |
| 125 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlFigure.cs` | Node representing an HTML `<figure>` grouped media block. |
| 126 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlFigCaption.cs` | Node representing an HTML `<figcaption>` caption for a figure. |
| 127 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/IHtmlTableElement.cs` | Marker contract for all HTML table-related structural nodes. |
| 128 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/IHtmlTableRowElement.cs` | Marker contract for nodes that represent a row within an HTML table. |
| 129 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTable.cs` | Node representing an HTML `<table>` element. |
| 130 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTableHeader.cs` | Node representing an HTML `<thead>` table header section. |
| 131 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTableBody.cs` | Node representing an HTML `<tbody>` table body section. |
| 132 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTableFooter.cs` | Node representing an HTML `<tfoot>` table footer section. |
| 133 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTableRow.cs` | Node representing an HTML `<tr>` table row. |
| 134 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTableRowData.cs` | Node representing an HTML `<td>` table data cell. |
| 135 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlTableRowHeader.cs` | Node representing an HTML `<th>` table header cell. |
| 136 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Model/HtmlExtractionStack.cs` | Stack of HTML node discriminators maintained during recursive extraction. |

---

## Layer 7 — StdHtml: factory provider & scoring

HTML node factory contracts and score criteria on top of Layer 5 & 6.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 137 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Extraction/Factory/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory` — HTML-specific factory contracts. |
| 138 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Extraction/Factory/IHtmlNodeFactory.cs` | Contract for HTML-specific node factories keyed on `HtmlNodeDiscriminator`. |
| 139 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/Extraction/Factory/HtmlNodeFactoryProvider.cs` | Provides `IHtmlNodeFactory` instances, selecting by discriminator matching. |
| 140 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/StructDataStack/Score/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score` — HTML-specific score helpers. |
| 141 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/StructDataStack/Score/HtmlNodeDiscriminatorExtentions.cs` | Extension methods on `HtmlNodeDiscriminator` for fluent score-criteria construction. |
| 142 | [ ] | `src/Sylvercode.StructDocExtractor.StdHtml/StructDataStack/Score/HtmlScoreCriteriaBuilder.cs` | Fluent builder for assembling HTML-specific `NodeScoreCriteriaSet` collections. |

---

## Layer 8 — AngleSharp: extraction factory implementations

AngleSharp-backed factories that convert DOM nodes to `IStructDocNode`s.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 143 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory` — AngleSharp DOM-to-model factories. |
| 144 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/BaseAngleNodeFactory.cs` | Abstract base factory for all AngleSharp node factories, handling DOM element access. |
| 145 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/BaseParagraphFactory.cs` | Abstract factory for paragraph-like AngleSharp elements (reusable inline-text logic). |
| 146 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/PlainTextNodeFactory.cs` | Factory that converts AngleSharp text nodes to `PlainTextNode`. |
| 147 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlAnchorFactory.cs` | Factory that converts `<a>` elements to `HtmlAnchor` nodes with resolved `href`. |
| 148 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlAsideFactory.cs` | Factory that converts `<aside>` elements to `HtmlAside` nodes. |
| 149 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlBrFactory.cs` | Factory that converts `<br>` elements to `HtmlBr` nodes. |
| 150 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlDivFactory.cs` | Factory that converts `<div>` elements to `HtmlDiv` nodes. |
| 151 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlEmphasesFactory.cs` | Factory that converts `<em>` elements to `HtmlEmphases` nodes. |
| 152 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlFigCaptionFactory.cs` | Factory that converts `<figcaption>` elements to `HtmlFigCaption` nodes. |
| 153 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlFigureFactory.cs` | Factory that converts `<figure>` elements to `HtmlFigure` nodes. |
| 154 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlHeadingFactory.cs` | Factory that converts `<h1>`–`<h6>` elements to `HtmlHeading` nodes with level. |
| 155 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlImgFactory.cs` | Factory that converts `<img>` elements to `HtmlImg` nodes with resolved `src`. |
| 156 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlListFactory.cs` | Factory that converts `<ul>`/`<ol>` elements to `HtmlList` nodes. |
| 157 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlListItemFactory.cs` | Factory that converts `<li>` elements to `HtmlListItem` nodes. |
| 158 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlNodeDiscriminatorFactory.cs` | Factory that creates a `HtmlNodeDiscriminator` from an AngleSharp element. |
| 159 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlParagraphFactory.cs` | Factory that converts `<p>` elements to `HtmlParagraph` nodes. |
| 160 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlStrongFactory.cs` | Factory that converts `<strong>` elements to `HtmlStrong` nodes. |
| 161 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableBodyFactory.cs` | Factory that converts `<tbody>` elements to `HtmlTableBody` nodes. |
| 162 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableFactory.cs` | Factory that converts `<table>` elements to `HtmlTable` nodes. |
| 163 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableFooterFactory.cs` | Factory that converts `<tfoot>` elements to `HtmlTableFooter` nodes. |
| 164 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableHeaderFactory.cs` | Factory that converts `<thead>` elements to `HtmlTableHeader` nodes. |
| 165 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableRowDataFactory.cs` | Factory that converts `<td>` elements to `HtmlTableRowData` nodes. |
| 166 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableRowFactory.cs` | Factory that converts `<tr>` elements to `HtmlTableRow` nodes. |
| 167 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlTableRowHeaderFactory.cs` | Factory that converts `<th>` elements to `HtmlTableRowHeader` nodes. |
| 168 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Factory/HtmlDataPreviewProvider.cs` | Preview provider that extracts a short text summary from AngleSharp HTML element data. |
| 169 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.AngleSharp.Extraction` — AngleSharp pipeline integration. |
| 170 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/AngleProcessTaskResultBuilder.cs` | Builds `IProcessTaskResult` objects from AngleSharp extraction outcomes. |
| 171 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Extraction/AngleSharpExtractorExtentions.cs` | DI/builder extension methods for registering AngleSharp extractors. |
| 172 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.AngleSharp` — top-level AngleSharp extraction package. |
| 173 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/AngleSharpNodeFactoryProvider.cs` | Entry-point provider that assembles the full set of AngleSharp node factories. |
| 174 | [ ] | `src/Sylvercode.StructDocExtractor.AngleSharp/StructDocNodeFactoryProviderSelector.cs` | Selector that chooses the correct AngleSharp factory provider for a given source. |

---

## Layer 9 — Markdown: serialization implementation

Markdown serializers built on the core serialization layer and StdHtml model.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 175 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Writer/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Markdown.Serialization.Writer` — Markdown stream writers. |
| 176 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Writer/StyleCharacter.cs` | Value type associating a Markdown style marker character with its semantic meaning. |
| 177 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Writer/MarkdownStyle.cs` | Enum defining Markdown inline styles (bold, italic, code, etc.). |
| 178 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Writer/MarkdownStreamWriter.cs` | `TextWriter` derivative that emits Markdown syntax, managing style stacks and block scope. |
| 179 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Writer/MarkdownStreamWriterProvider.cs` | Provides scoped `MarkdownStreamWriter` instances for a serialization session. |
| 180 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Writer/MarkdownStreamWriterProviderExtensions.cs` | DI/builder extensions for registering `MarkdownStreamWriterProvider`. |
| 181 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Base/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Markdown.Serialization.Base` — abstract Markdown serializer bases. |
| 182 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Base/BaseMarkdownSerializer.cs` | Abstract base providing shared Markdown output helpers to all node serializers. |
| 183 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Base/BaseStyleSerializer.cs` | Abstract base for inline-style serializers (bold, italic) that wrap content in style markers. |
| 184 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.StructDocExtractor.Markdown.Serialization` — Markdown serializers for all node types. |
| 185 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/MarkdownLinkFormater.cs` | Formats `[text](uri)` Markdown link syntax from anchor and image nodes. |
| 186 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/IStructDocNodeExtentions.cs` | Extension methods that add Markdown-specific helpers to `IStructDocNode`. |
| 187 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/PlainTextSerializer.cs` | Serializer that outputs plain-text node content verbatim. |
| 188 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/HeadingSerializer.cs` | Serializer that outputs `HtmlHeading` nodes as ATX Markdown headings (`# … ######`). |
| 189 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/ParagraphSerializer.cs` | Serializer that wraps `HtmlParagraph` children in a Markdown paragraph block. |
| 190 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/AnchorSerializer.cs` | Serializer that emits `HtmlAnchor` nodes as Markdown inline links. |
| 191 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/ImageSerializer.cs` | Serializer that emits `HtmlImg` nodes as Markdown image syntax `![alt](src)`. |
| 192 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/BrSerializer.cs` | Serializer that emits `HtmlBr` as a Markdown line break (two trailing spaces). |
| 193 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/EmphasesSerializer.cs` | Serializer that wraps `HtmlEmphases` content in Markdown italic markers. |
| 194 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/StrongSerializer.cs` | Serializer that wraps `HtmlStrong` content in Markdown bold markers. |
| 195 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/ListSerializer.cs` | Serializer that outputs `HtmlList` as a Markdown bulleted or numbered list. |
| 196 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/ListItemSerializer.cs` | Serializer that emits `HtmlListItem` as a single Markdown list item line. |
| 197 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/CalloutSerializer.cs` | Serializer that emits callout/aside nodes as Markdown block-quote sections. |
| 198 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/FigCaptionSerializer.cs` | Serializer that emits `HtmlFigCaption` as an italicised Markdown caption line. |
| 199 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableSerializer.cs` | Serializer that emits `HtmlTable` nodes as a Markdown GFM table. |
| 200 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableHeaderSerializer.cs` | Serializer that emits the `<thead>` portion of a Markdown GFM table. |
| 201 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableBodySerializer.cs` | Serializer that emits the `<tbody>` rows of a Markdown GFM table. |
| 202 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableFooterSerializer.cs` | Serializer that emits the `<tfoot>` rows of a Markdown GFM table. |
| 203 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableRowSerializer.cs` | Serializer that emits a `HtmlTableRow` as a `| … |` Markdown table row. |
| 204 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableRowDataSerializer.cs` | Serializer that emits `<td>` cells within a Markdown table row. |
| 205 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/TableRowHeaderSerializer.cs` | Serializer that emits `<th>` cells within a Markdown table header row. |
| 206 | [ ] | `src/Sylvercode.StructDocExtractor.Markdown/Serialization/MarkdownSerializationExtentions.cs` | DI extension methods (in `Microsoft.Extensions.DependencyInjection`) that register all Markdown serializers. |

---

## Layer 10 — SiteExtractor: URI utilities & core abstractions

`Sylvercode.SiteExtractor` primitives with no dependencies on extractor or site-specific logic.

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 207 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.UriUtils` — URI matching, translation, and redirection helpers. |
| 208 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/IUriMatcher.cs` | Contract for testing whether a `Uri` satisfies a given matching rule. |
| 209 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/IUriTranslater.cs` | Contract for translating a `Uri` from one form to another during extraction. |
| 210 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/IUriRedirector.cs` | Contract for redirecting a `Uri` to a different target location. |
| 211 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/UriExtentions.cs` | Extension helpers for `Uri` manipulation (combining, normalising, relativising, etc.). |
| 212 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/ImageUriMatcher.cs` | Matcher that identifies URIs pointing to common image file extensions. |
| 213 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/UriMatcherByBase.cs` | Matcher that accepts URIs whose base matches a configured root URI. |
| 214 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/NoopUriTranslater.cs` | Identity translater that returns its input URI unchanged. |
| 215 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/UriBaseTranslater.cs` | Translater that rewrites a URI by swapping its base to a new root. |
| 216 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/RelativeUriTranslater.cs` | Translater that resolves a relative URI against a configured base. |
| 217 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/StaticDirUriTranslater.cs` | Translater that maps a URI to a path inside a static output directory. |
| 218 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/UriRelativeFromBaseTranslater.cs` | Translater that produces a relative URI expressed from a given base URI. |
| 219 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/UriRedirector.cs` | Default `IUriRedirector` that applies a list of redirect rules in order. |
| 220 | [ ] | `src/Sylvercode.SiteExtractor/UriUtils/UriRedirectorExtentions.cs` | Extension helpers for registering and composing `IUriRedirector` instances. |

---

## Layer 11 — SiteExtractor: sources & store abstractions

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 221 | [ ] | `src/Sylvercode.SiteExtractor/Sources/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.Sources` — site source contracts and in-memory implementation. |
| 222 | [ ] | `src/Sylvercode.SiteExtractor/Sources/ISiteSource.cs` | Contract for fetching the raw document content of a resource by URI. |
| 223 | [ ] | `src/Sylvercode.SiteExtractor/Sources/ISiteSourceProvider.cs` | Contract for providing the appropriate `ISiteSource` for a given URI. |
| 224 | [ ] | `src/Sylvercode.SiteExtractor/Sources/MemorySiteSource.cs` | In-memory `ISiteSource` backed by a dictionary of pre-loaded document strings. |
| 225 | [ ] | `src/Sylvercode.SiteExtractor/Sources/MemorySiteSourceExtensions.cs` | Extension helpers for populating and registering `MemorySiteSource`. |
| 226 | [ ] | `src/Sylvercode.SiteExtractor/Store/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.Store` — data stores for persisting extracted content. |
| 227 | [ ] | `src/Sylvercode.SiteExtractor/Store/IDataStore.cs` | Contract for reading and writing extracted document data to a persistent store. |
| 228 | [ ] | `src/Sylvercode.SiteExtractor/Store/BaseDataStore.cs` | Abstract base providing common implementation helpers for `IDataStore`. |
| 229 | [ ] | `src/Sylvercode.SiteExtractor/Store/MemoryDataStore.cs` | In-memory `IDataStore` implementation suitable for testing and temporary buffering. |
| 230 | [ ] | `src/Sylvercode.SiteExtractor/Store/MemoryDataStoreExtensions.cs` | Extension methods for registering `MemoryDataStore` in DI containers. |
| 231 | [ ] | `src/Sylvercode.SiteExtractor/Store/DirectoryDataStore.cs` | `IDataStore` implementation that writes extracted files to a directory on disk. |
| 232 | [ ] | `src/Sylvercode.SiteExtractor/Store/DirectoryDataStoreExtensions.cs` | Extension methods for registering `DirectoryDataStore` in DI containers. |

---

## Layer 12 — SiteExtractor: resource model & processing pipeline

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 233 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.Resources` — resource tracking and update orchestration. |
| 234 | [ ] | `src/Sylvercode.SiteExtractor/Resources/IReadOnlyResourceRepository.cs` | Read-only contract for querying the set of tracked site resources. |
| 235 | [ ] | `src/Sylvercode.SiteExtractor/Resources/IReferencerUpdater.cs` | Contract for updating embedded URIs inside a resource's serialized content. |
| 236 | [ ] | `src/Sylvercode.SiteExtractor/Resources/IResourceUriTranslater.cs` | Contract for translating a resource URI into its final output path. |
| 237 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ResourcePullState.cs` | Enum of states a resource can be in during the download/copy cycle (pending, pulled, etc.). |
| 238 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ResourceState.cs` | Tracks the current pull state and any error for a single resource entry. |
| 239 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Resource.cs` | Represents a site resource (page or asset) with its source URI and pull state. |
| 240 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ResourceQueue.cs` | Thread-safe queue of resources pending extraction or download. |
| 241 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ResourceRepository.cs` | Writable repository that stores and indexes all discovered `Resource` objects. |
| 242 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ResourcesTracker.cs` | Coordinates resource discovery, deduplication, and state transitions. |
| 243 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ReferencerUpdater.cs` | Applies `IReferencerUpdater` passes on serialized content to rewrite embedded URIs. |
| 244 | [ ] | `src/Sylvercode.SiteExtractor/Resources/ResourceUriTranslater.cs` | Default `IResourceUriTranslater` composing an ordered chain of URI translaters. |
| 245 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.Resources.Processors` — resource processing contracts and implementations. |
| 246 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/IResourceProcessorResult.cs` | Contract for the outcome produced by a resource processor. |
| 247 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/IResourceProcessor.cs` | Contract for processing a single resource (extracting data, copying bytes, etc.). |
| 248 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/IResourceProcessorProvider.cs` | Contract for resolving the correct `IResourceProcessor` for a given resource. |
| 249 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/IResourceCopiler.cs` | Contract for copying resource bytes from source to a data store. |
| 250 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/IResourceDataExtractor.cs` | Contract for extracting structured data from a resource document. |
| 251 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/IResourceDependancyFilter.cs` | Contract for filtering which dependency links within a resource should be followed. |
| 252 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/BaseResourceProcessorResult.cs` | Abstract base for processor results sharing common outcome fields. |
| 253 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/NoPostProcessResult.cs` | Processor result indicating no further post-processing is required. |
| 254 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/DataExtractedProcessorResult.cs` | Processor result carrying extracted structural data and discovered child URIs. |
| 255 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/ResourceCopierOptions.cs` | Configuration options for the `ResourceCopier` (retry policy, buffer size, etc.). |
| 256 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/ResourceCopier.cs` | `IResourceProcessor` that copies resource bytes verbatim to the data store. |
| 257 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/ResourceDataExtractor.cs` | `IResourceProcessor` that parses and extracts structured data from a document resource. |
| 258 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/ResourceProcessorProvider.cs` | Default `IResourceProcessorProvider` selecting processors by resource MIME type or URI pattern. |
| 259 | [ ] | `src/Sylvercode.SiteExtractor/Resources/Processors/ResourceProcessorExtentions.cs` | Extension methods for composing and registering resource processors. |

---

## Layer 13 — SiteExtractor: HTTP downloader

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 260 | [ ] | `src/Sylvercode.SiteExtractor/Downloader/Http/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.Downloader.Http` — HTTP-based resource downloader. |
| 261 | [ ] | `src/Sylvercode.SiteExtractor/Downloader/Http/HttpDownloader.cs` | `IResourceCopiler` that downloads resources over HTTP and writes them to the data store. |
| 262 | [ ] | `src/Sylvercode.SiteExtractor/Downloader/Http/HttpDownloaderExtentions.cs` | DI extension methods for registering `HttpDownloader` and its HTTP client. |

---

## Layer 14 — SiteExtractor: top-level orchestration

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 263 | [ ] | `src/Sylvercode.SiteExtractor/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor` — top-level site extraction orchestration. |
| 264 | [ ] | `src/Sylvercode.SiteExtractor/ISiteExtractor.cs` | Top-level contract for running a complete site extraction pass. |
| 265 | [ ] | `src/Sylvercode.SiteExtractor/SiteExtractorOptions.cs` | Configuration options controlling the site extractor (seed URIs, depth limit, etc.). |
| 266 | [ ] | `src/Sylvercode.SiteExtractor/SiteExtractor.cs` | Main `ISiteExtractor` implementation coordinating source fetch, processing, and storage. |
| 267 | [ ] | `src/Sylvercode.SiteExtractor/SiteExtractorExtensions.cs` | DI/builder extensions for registering the full `SiteExtractor` pipeline. |

---

## Layer 15 — SiteExtractor.StdHtml: HTML dependency filter

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 268 | [ ] | `src/Sylvercode.SiteExtractor.StdHtml/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.StdHtml` — HTML-specific dependency filtering for site extraction. |
| 269 | [ ] | `src/Sylvercode.SiteExtractor.StdHtml/HtmlResourceDependencyFilterMode.cs` | Enum of filter modes (Keep, Remove, Download) applied to HTML dependency links. |
| 270 | [ ] | `src/Sylvercode.SiteExtractor.StdHtml/HtmlResourceDependencyFilterOptions.cs` | Configuration options for the HTML resource dependency filter (rules per link type). |
| 271 | [ ] | `src/Sylvercode.SiteExtractor.StdHtml/HtmlResourceDependencyFilter.cs` | `IResourceDependancyFilter` that classifies HTML resource links (stylesheets, scripts, images). |
| 272 | [ ] | `src/Sylvercode.SiteExtractor.StdHtml/HtmlResourceDependencyFilterExtensions.cs` | DI extensions for registering the HTML dependency filter. |

---

## Layer 16 — SiteExtractor.Markdown: URI translation in Markdown content

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 273 | [ ] | `src/Sylvercode.SiteExtractor.Markdown/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.Markdown` — Markdown-specific URI updating after site extraction. |
| 274 | [ ] | `src/Sylvercode.SiteExtractor.Markdown/MarkdownUriTranslater.cs` | Translates resource URIs embedded inside serialised Markdown text. |
| 275 | [ ] | `src/Sylvercode.SiteExtractor.Markdown/MarkdownReferencerUpdater.cs` | `IReferencerUpdater` that rewrites link and image URIs in Markdown content post-extraction. |
| 276 | [ ] | `src/Sylvercode.SiteExtractor.Markdown/MarkdownSerializationExtentions.cs` | DI extensions (in `Microsoft.Extensions.DependencyInjection`) for registering Markdown site-extraction services. |

---

## Layer 17 — SiteExtractor.AngleSharp: web source implementations

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 277 | [ ] | `src/Sylvercode.SiteExtractor.AngleSharp/Namespace.cs` **[NEW FILE]** | Namespace doc for `Sylvercode.SiteExtractor.AngleSharp` — AngleSharp-powered live and proxy site sources. |
| 278 | [ ] | `src/Sylvercode.SiteExtractor.AngleSharp/BaseAngleSharpSiteSource.cs` | Abstract base `ISiteSource` managing AngleSharp browsing context and HTTP client setup. |
| 279 | [ ] | `src/Sylvercode.SiteExtractor.AngleSharp/AngleSharpWebSource.cs` | `ISiteSource` that fetches and parses live web pages over the internet via AngleSharp. |
| 280 | [ ] | `src/Sylvercode.SiteExtractor.AngleSharp/AngleSharpProxySource.cs` | `ISiteSource` that fetches pages through a configured HTTP proxy via AngleSharp. |
| 281 | [ ] | `src/Sylvercode.SiteExtractor.AngleSharp/AngleSharpSiteExtractorExtentions.cs` | DI extensions (in `Microsoft.Extensions.DependencyInjection`) for registering AngleSharp site sources. |

---

## Layer 18 — Test projects

Test files are documented last (least critical, most knowledge of above layers required).

### `Sylvercode.SiteExtractor.Tests`

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 282 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Stubs/UriNode.cs` | Stub `IStructDocNode` carrying a `Uri` used in URI-translation tests. |
| 283 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Stubs/UriReferenceNode.cs` | Stub `IStructDocReferencer` node for verifying URI-reference update behaviour. |
| 284 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/DataStoreMock.cs` | Mock `IDataStore` recording write calls for assertion in unit tests. |
| 285 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/ExtractorMock.cs` | Mock `IExtractor` returning pre-canned results for isolation testing. |
| 286 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/MockCallsTracker.cs` | Helper that records method invocation order and arguments across mock objects. |
| 287 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/ResourceProcessorMock.cs` | Mock `IResourceProcessor` capturing process calls for verification. |
| 288 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/ResourceProcessorResultMock.cs` | Mock `IResourceProcessorResult` with configurable outcome fields. |
| 289 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/SiteSourceMock.cs` | Mock `ISiteSource` serving predefined document strings by URI. |
| 290 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Mocks/StructDocSerializerMock.cs` | Mock `IStructDocSerializer` returning stub serialisation strings. |
| 291 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/Fakes/FakeResourceProcessoProvider.cs` | Fake `IResourceProcessorProvider` returning a fixed processor for all resources. |
| 292 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ImageUriMatcherTests.cs` | Tests for `ImageUriMatcher.IsMatching` against various image and non-image URIs. |
| 293 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/MemorySiteSourceTests.cs` | Tests for `MemorySiteSource` document retrieval and missing-key behaviour. |
| 294 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ReferencerUpdaterTests.cs` | Tests for `ReferencerUpdater.UpdateReferencers` rewriting embedded URIs. |
| 295 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/RelativeUriTranslaterTests_Translate.cs` | Tests for `RelativeUriTranslater.Translate` resolving relative URIs against a base. |
| 296 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourceCopierTests.cs` | Tests for `ResourceCopier.Download` copying bytes to the data store. |
| 297 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourceDataExtractorTests.cs` | Tests for `ResourceDataExtractor.Extract` producing structured nodes from documents. |
| 298 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourceDictionaryTests.cs` | Tests for `ResourceRepository.Add` deduplication and indexing logic. |
| 299 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourceProcessorProviderTests.cs` | Tests for `ResourceProcessorProvider.GetProcessor` resolution by MIME type and URI. |
| 300 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourceStateTests.cs` | Tests for `ResourceState.IsPullable` state-machine transitions. |
| 301 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourceTests.cs` | Tests for `Resource.TranslateUri` producing correct output URIs. |
| 302 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/ResourcesTrackerTests.cs` | Tests for `ResourcesTracker.AddResource` discovery and deduplication. |
| 303 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/SiteExtractorTests.cs` | End-to-end tests for `SiteExtractor.Extract` orchestrating the full pipeline. |
| 304 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/StaticDirUriTranslaterTests_Translate.cs` | Tests for `StaticDirUriTranslater.Translate` mapping URIs to local paths. |
| 305 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/UriBaseTranslaterTests.cs` | Tests for `UriBaseTranslater.Translaste` swapping URI bases. |
| 306 | [ ] | `tests/Sylvercode.SiteExtractor.Tests/UriRelativeFromBaseTranslaterTests.cs` | Tests for `UriRelativeFromBaseTranslater.Translate` producing relative-from-base URIs. |

### `Sylvercode.SiteExtractor.StdHtml.Tests`

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 307 | [ ] | `tests/Sylvercode.SiteExtractor.StdHtml.Tests/Stubs/StubReferencer.cs` | Stub `IStructDocReferencer` for verifying HTML dependency filter behaviour. |
| 308 | [ ] | `tests/Sylvercode.SiteExtractor.StdHtml.Tests/HtmlResourceDependencyFilterTetsts.cs` | Tests for `HtmlResourceDependencyFilter.Accepted` against various HTML link types. |

### `Sylvercode.SiteExtractor.Markdown.Tests`

| # | Status | File | One-liner |
|---|--------|------|-----------|
| 309 | [ ] | `tests/Sylvercode.SiteExtractor.Markdown.Tests/MarkdownReferencerUpdaterTests.cs` | Tests for `MarkdownReferencerUpdater.UpdateReferencers` rewriting Markdown links. |
| 310 | [ ] | `tests/Sylvercode.SiteExtractor.Markdown.Tests/MarkdownUriTranslaterTests.cs` | Tests for `MarkdownUriTranslater.Translate` converting URIs in Markdown text. |

---

*Total items: **310** (including **47** new `Namespace.cs` files to create)*
