using System;
using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlNodeDiscriminatorFactory : IDataDiscriminatorFactory<IElement, HtmlNodeDiscriminator>
{
    public HtmlNodeDiscriminator CreateDataDiscriminator(IElement data)
    {
        throw new NotImplementedException();
    }
}
