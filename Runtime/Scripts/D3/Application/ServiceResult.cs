using UnityEngine;

namespace Moonstone.D3.Application
{
    public class ServiceResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public System.Exception Exception { get; }

        public ServiceResult()
        {
            IsSuccess = true;
        }

        public ServiceResult(string errorMessage, System.Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public ServiceResult PrintError()
        {
            if (IsSuccess) return this;
            Debug.LogException(Exception);
            return this;
        }

        public static ServiceResult Success() => new();
        public static ServiceResult Failure(string errorMessage, System.Exception ex) => new(errorMessage, ex);
    }

    public class ServiceResult<TData>
    {
        public TData Data { get; }
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public string StackTrace { get; }

        public ServiceResult(TData data)
        {
            Data = data;
            IsSuccess = true;
        }

        public ServiceResult(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public ServiceResult(System.Exception ex)
        {
            IsSuccess = false;
            ErrorMessage = ex.Message;
            StackTrace = ex.StackTrace;
        }

        public ServiceResult<TData> PrintError()
        {
            if (IsSuccess) return this;
            Debug.LogError($"[{GetType().Name}]: {ErrorMessage}\n{StackTrace}");
            return this;
        }

        public static ServiceResult<TData> Success(TData result) => new(result);
        public static ServiceResult<TData> Failure(System.Exception ex) => new(ex);
    }
}