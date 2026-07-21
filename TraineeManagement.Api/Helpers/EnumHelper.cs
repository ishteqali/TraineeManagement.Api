using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.Helpers
{
    public static class EnumHelper
    {
        public static T ParseOrThrow<T>(string? value, string fieldName) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BadRequestException(ExceptionMessages.EnumFieldEmpty(fieldName));
            }
            if (!Enum.TryParse<T>(value, ignoreCase: true, out T result))
            {
                string validValues = string.Join(", ", Enum.GetNames<T>());
                throw new BadRequestException(ExceptionMessages.EnumNotValid(value, fieldName, validValues));
            }
            return result;
        }
    }
}