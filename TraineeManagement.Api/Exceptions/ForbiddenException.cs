using System.Net;
using TraineeManagement.Api.Exceptions.Base;

namespace TraineeManagement.Api.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message) : base(message, (int)HttpStatusCode.Forbidden)
        {
        }
    }
}

