using System.Net;
using TraineeManagement.Api.Exceptions.Base;

namespace TraineeManagement.Api.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message) : base(message, (int)HttpStatusCode.Unauthorized)
        {
        }
    }
}

