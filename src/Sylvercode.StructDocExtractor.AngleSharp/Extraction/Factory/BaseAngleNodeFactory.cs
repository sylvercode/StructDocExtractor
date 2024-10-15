using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public abstract class BaseAngleNodeFactory : IHtmlNodeFactory<IElement>
{
    public virtual HtmlNodeDiscriminator[]? DefaultSelector { get; }

    public IProcessTaskResult<IElement, HtmlNodeDiscriminator> NewNode(HtmlNodeDiscriminator discriminator, IElement data)
    {
        AngleProcessTaskResultBuilder builder = new(data);
        builder.WithDataDiscriminator(discriminator);

        return BuildNode(builder, data) ? builder.Build() : ProcessTaskResult.NewError<IElement, HtmlNodeDiscriminator>();
    }

    protected abstract bool BuildNode(AngleProcessTaskResultBuilder resultBuilder, IElement element);
}
