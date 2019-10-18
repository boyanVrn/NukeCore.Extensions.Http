using System;

namespace UCS.Extensions.Http.Models.Base
{
    public interface IFail
    {
        Enum Code { get; }

        string Description { get; }

    }
}
