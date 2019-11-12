using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models
{
    public class RequestBase<T> : IRequest<T>
    {
        private T _data;

        public T Data
        {
            get => _data;
            set
            {
                _data = value;
                IsSuccess = value != null;
            }
        }

        public bool IsSuccess { get; private set; }
    }
}
