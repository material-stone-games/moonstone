using System;
using System.Collections.Generic;

namespace Moonstone.D3.Application
{
    public sealed class Result<TData> : Result
    {
        public TData Data { get; }

        private Result(TData data)
        {
            Data = data;
        }

        private Result(IEnumerable<string> errors, string errorCode = null, Exception exception = null)
            : base(errors, errorCode, exception)
        {
        }

        public static Result<TData> Success(TData data) => new(data);

        public new static Result<TData> Failure(string errorMessage, string errorCode = null)
        {
            return new Result<TData>(new[] { errorMessage }, errorCode);
        }

        public new static Result<TData> Failure(IEnumerable<string> errors, string errorCode = null)
        {
            return new Result<TData>(errors, errorCode);
        }

        public new static Result<TData> Failure(Exception exception, string errorCode = null)
        {
            return new Result<TData>(new[] { exception?.Message }, errorCode, exception);
        }
    }
}
