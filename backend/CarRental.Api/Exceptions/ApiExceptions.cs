namespace CarRental.Api.Exceptions;

public class NotFoundException(string message) : Exception(message);
public class ConflictException(string message) : Exception(message);
public class RequestValidationException(string message) : Exception(message);
