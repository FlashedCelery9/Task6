namespace Task6.Services;

public sealed class FileValidationException(string message) : Exception(message);