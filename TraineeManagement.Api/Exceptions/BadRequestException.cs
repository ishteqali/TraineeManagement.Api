using System.Net;
using TraineeManagement.Api.Exceptions.Base;

namespace TraineeManagement.Api.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string message) : base(message, (int)HttpStatusCode.BadRequest)
        {
        }
    }
}

