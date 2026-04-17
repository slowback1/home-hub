using System;

namespace Logic.User;

public class TokenInvalidException(string? message = null, Exception? inner = null) : Exception(message, inner);

public class TokenExpiredException(string? message = null, Exception? inner = null) : Exception(message, inner);