using System;
using NukeCore.Extensions.Http.Common.Additional;
using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models.Base.Resolvers
{
    public class FailBase : IFail
    {
        public string Code { get; }
        public string Description { get; }
        public bool IsInternalError { get; protected set; }

        public FailBase(Enum code, string description = default)
        {
            Code = code.ToString();
            Description = description ?? string.Empty;
        }

        private FailBase(string code, string description)
        {
            Code = code;
            Description = description ?? string.Empty;
        }

        public T GetCodeAs<T>(T def = default) where T : struct, Enum
            => Code.AsEnum(def);


        public static FailBase CreateInstance<T>(T fail)
            where T : IFail => new FailBase(fail.Code, fail.Description);

        public static FailBase CreateInstance(string code, string description) => new FailBase(code, description);
    }
}
