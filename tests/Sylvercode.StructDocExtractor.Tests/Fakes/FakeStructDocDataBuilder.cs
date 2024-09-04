using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.StructDocExtractor.Tests.Fakes;

public class FakeStructDocDataBuilder
{
    private readonly FakeStructDocData _data;
    private readonly FakeStructDocDataBuilder? _parent;

    public FakeStructDocDataBuilder() : this(new FakeStructDocData(), null)
    {
    }

    private FakeStructDocDataBuilder(FakeStructDocData data, FakeStructDocDataBuilder? parent)
    {
        _data = data;
        _parent = parent;
    }

    public FakeStructDocDataBuilder WithId(string id)
    {
        _data.Id = id;
        return this;
    }

    public FakeStructDocDataBuilder WithData(string data)
    {
        _data.Data = data;
        return this;
    }

    public FakeStructDocDataBuilder WithChild(string id, string data)
    {
        FakeStructDocData child = new()
        {
            Id = id,
            Data = data,
            Parent = _data
        };
        _data.Children.Add(child);
        return this;
    }
    public FakeStructDocDataBuilder WithChild(string id, string data, out FakeStructDocData result)
    {
        result = WithChild(id, data)._data;
        return this;
    }

    public FakeStructDocDataBuilder WithResultType(TaskResultType resultType)
    {
        _data.ResultType = resultType;
        return this;
    }

    public FakeStructDocDataBuilder StartChildBuilder()
    {
        FakeStructDocData child = new()
        {
            Parent = _data
        };
        _data.Children.Add(child);
        return new(child, this);
    }

    public FakeStructDocDataBuilder StartChildBuilder(out FakeStructDocData result)
    {
        FakeStructDocDataBuilder childBuilder = StartChildBuilder();
        result = childBuilder._data;
        return this;
    }

    public FakeStructDocDataBuilder EndChildBuilder()
        => _parent ?? throw new InvalidOperationException("Root node cannot have a parent.");

    public FakeStructDocData Build()
    {
        if (_parent is not null)
            throw new InvalidOperationException("Only the root node can be built.");

        return _data;
    }
}
