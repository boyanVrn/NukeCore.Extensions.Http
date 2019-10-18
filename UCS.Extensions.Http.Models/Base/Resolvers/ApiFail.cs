using System;

namespace UCS.Extensions.Http.Models.Base.Resolvers
{
    public class ApiFail : IFail
    {
        public ApiFail(Enum code, string description = default)
        {
            Code = code;
            Description = description ?? string.Empty;
        }

        public Enum Code { get; }
        public string Description { get; }

    }
}
