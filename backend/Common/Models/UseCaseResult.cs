using System;

namespace Common.Models;

public class UseCaseResult<T>
{
	private UseCaseResult()
	{
	}

	public T? Result { get; private set; }
	public UseCaseStatus Status { get; private set; }
	public string? ErrorMessage { get; private set; }
	public Exception? Exception { get; private set; }

	public static UseCaseResult<T> Success(T? result = default)
	{
		return new UseCaseResult<T>
		{
			Result = result,
			Status = UseCaseStatus.Success
		};
	}

	public static UseCaseResult<T> Failure(string? errorMessage = null, Exception? exception = null)
	{
		return new UseCaseResult<T>
		{
			Status = UseCaseStatus.Failure,
			ErrorMessage = errorMessage,
			Exception = exception
		};
	}
}

public enum UseCaseStatus
{
	Success,
	Failure
}