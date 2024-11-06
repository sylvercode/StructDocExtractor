using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.AngleSharp;

public class AngleSharpNodeFactoryProvider : HtmlNodeFactoryProvider<INode>, IRouterExtractorSelectorProvider<INode>
{
    protected sealed class RouterSelector(Func<INode, INode?> matchFunc) : IRouterExtractorSelector<INode>
    {
        public static Func<INode, INode?> GetElementSelector(string elementSelector)
            => (node) => (node is IElement element) ? element.QuerySelector(elementSelector) : null;

        public RouterSelector() : this((node) => node)
        {
        }
        public INode? Match(INode data)
            => matchFunc(data);
    }

    public IRouterExtractorSelector<INode> DefaultRouterSelector { get; }

    public AngleSharpNodeFactoryProvider(string elementSelector, bool addDefaultFactory = true)
        : this(RouterSelector.GetElementSelector(elementSelector), addDefaultFactory)
    {
    }

    public AngleSharpNodeFactoryProvider(Func<INode, INode?>? selector, bool addDefaultFactory = true)
    {
        DefaultRouterSelector = selector is not null ? new RouterSelector(selector) : new RouterSelector();
        if (addDefaultFactory)
            AddDefaultFactory();
    }

    protected void AddDefaultFactory()
    {
        AddFactory(new PlainTextNodeFactory());
        AddFactory(new HtmlHeadingFactory(HtmlHeadingFactoryOptions));
        AddFactory(new HtmlBrFactory());
        AddFactory(new HtmlParagraphFactory());
        AddFactory(new HtmlDivFactory());
        AddFactory(new HtmlFigureFactory());
        AddFactory(new HtmlAsideFactory());
        AddFactory(new HtmlEmphasesFactory());
        AddFactory(new HtmlStrongFactory());
        AddFactory(new HtmlAnchorFactory());
        AddFactory(new HtmlFigCaptionFactory());
        AddFactory(new HtmlImgFactory());
        AddFactory(new HtmlListFactory());
        AddFactory(new HtmlListItemFactory());
        AddFactory(new HtmlTableFactory());
        AddFactory(new HtmlTableHeaderFactory());
        AddFactory(new HtmlTableBodyFactory());
        AddFactory(new HtmlTableFooterFactory());
        AddFactory(new HtmlTableRowFactory());
        AddFactory(new HtmlTableRowDataFactory());
        AddFactory(new HtmlTableRowHeaderFactory());
    }

    protected virtual HtmlHeadingFactory.Options? HtmlHeadingFactoryOptions => null;
}
