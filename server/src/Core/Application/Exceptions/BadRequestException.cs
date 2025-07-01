using System;

namespace Application.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base("bad request exception") { }

    public BadRequestException(string message) : base(message) { }
}