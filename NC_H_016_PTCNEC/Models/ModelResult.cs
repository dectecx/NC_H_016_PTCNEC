namespace NC_H_016_PTCNEC.Models;

class ModelResult
{
    public bool IsOk { get; private set; }

    public string? ErrorMsg { get; private set; }

    public ModelResult()
    {
        this.IsOk = true;
    }

    public ModelResult(string errorMsg)
    {
        this.IsOk = false;
        this.ErrorMsg = errorMsg;
    }
}

class ModelResult<T> : ModelResult
{
    public T? Data { get; private set; }

    public ModelResult(T? model) : base()
    {
        this.Data = model;
    }

    public ModelResult(string errorMsg) : base(errorMsg) { }
}