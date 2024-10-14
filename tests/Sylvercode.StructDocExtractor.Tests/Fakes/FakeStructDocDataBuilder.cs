using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Extraction;

namespace Sylvercode.StructDocExtractor.Tests.Fakes;

public class FakeStructDocDataBuilder(FakeStructDocData? data = null)
{
    private FakeStructDocData? _data = data;
    private FakeStructDocData Data
    {
        get
        {
            InitData();
            return _data;
        }
    }
    private readonly FakeStructDocDataBuilder? _parent;

    private FakeStructDocDataBuilder(FakeStructDocDataBuilder? parent) : this()
    {
        _parent = parent;
    }

    public static implicit operator FakeStructDocData(FakeStructDocDataBuilder builder) => builder.Build();

    [MemberNotNull(nameof(_data))]
    private void InitData(bool force = false)
    {
        if (!force && _data is not null)
            return;

        _data = new();
        _parent?.Data.Children.Add(_data);
    }

    public FakeStructDocDataBuilder New(string? id = null, string? data = null)
    {
        if (_parent is null)
            throw new InvalidOperationException("Only child nodes can be created using this method.");

        InitData(force: true);

        if (id is not null)
            _data.Id = id;

        if (data is not null)
            _data.Data = data;

        return this;
    }

    public FakeStructDocDataBuilder WithMetadata(string name, object value)
    {
        InitData();
        _data.Metadatas.AddMetadata(name, value);
        return this;
    }

    public FakeStructDocDataBuilder WithId(string id)
    {
        InitData();
        _data.Id = id;
        return this;
    }

    public FakeStructDocDataBuilder WithData(string data)
    {
        InitData();
        _data.Data = data;
        return this;
    }

    public FakeStructDocDataBuilder WithChild(string id, string data)
    {
        WithChild(id, data, out _);
        return this;
    }

    public FakeStructDocDataBuilder WithChild(string id, string data, out FakeStructDocData result)
    {
        InitData();
        FakeStructDocData child = new()
        {
            Id = id,
            Data = data,
            Parent = _data
        };
        _data.Children.Add(child);
        result = child;
        return this;
    }

    public FakeStructDocDataBuilder WithResultType(TaskResultType resultType)
    {
        InitData();
        _data.ResultType = resultType;
        return this;
    }

    public FakeStructDocDataBuilder NewChildrenBuilder()
    {
        return new(this);
    }

    public FakeStructDocDataBuilder BuildChildren()
        => _parent ?? throw new InvalidOperationException("Root node cannot have a parent.");

    public FakeStructDocData Build()
    {
        if (_parent is not null)
            throw new InvalidOperationException("Only the root node can be built.");

        InitData();
        return _data;
    }

}
