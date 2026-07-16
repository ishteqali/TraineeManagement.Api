using System.Net;
using TraineeManagement.Api.Exceptions.Base;

namespace TraineeManagement.Api.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message) : base(message, (int)HttpStatusCode.NotFound)
        {
        }
    }
}

