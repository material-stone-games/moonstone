using System;
using System.Collections.Generic;
using System.Linq;

namespace Moonstone.D3.Application
{
    public class Result
    {
        private static readonly string[] EmptyErrors = Array.Empty<string>();

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string ErrorCode { get; }
        public IReadOnlyList<string> Errors { get; }
        public string ErrorMessage => Errors.Count > 0 ? Errors[0] : null;
        public Exception Exception { get; }

        protected Result()
        {
            IsSuccess = true;
            Errors = EmptyErrors;
        }

        protected Result(IEnumerable<string> errors, string errorCode = null, Exception exception = null)
        {
            var errorList = errors?
                .Where(error => !string.IsNullOrWhiteSpace(error))
                .ToArray() ?? EmptyErrors;

            IsSuccess = false;
            ErrorCode = errorCode;
            Errors = errorList;
            Exception = exception;
        }

        public static Result Success() => new();

        public static Result Failure(string errorMessage, string errorCode = null)
        {
            return new Result(new[] { errorMessage }, errorCode);
        }

        public static Result Failure(IEnumerable<string> errors, string errorCode = null)
        {
            return new Result(errors, errorCode);
        }

        public static Result Failure(Exception exception, string errorCode = null)
        {
            return new Result(new[] { exception?.Message }, errorCode, exception);
        }
    }
}
