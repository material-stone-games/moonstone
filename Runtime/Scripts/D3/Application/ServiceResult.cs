using UnityEngine;

namespace Moonstone.D3.Application
{
    public class ServiceResult
    {
        public bool IsSuccess { get; }
        public System.Exception Exception { get; }

        public ServiceResult()
        {
            IsSuccess = true;
        }

        public ServiceResult(System.Exception exception)
        {
            IsSuccess = false;
            Exception = exception;
        }

        public ServiceResult PrintError()
        {
            if (IsSuccess) return this;
            Debug.LogException(Exception);
            return this;
        }

        public static ServiceResult Success() => new();
        public static ServiceResult Failure(System.Exception exception) => new(exception);
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