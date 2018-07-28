using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSocket
{
    class TResult
    {
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = null;
        public int ErrorCode { get; set; } = -1;
        public Exception Exception { get; set; } = null;


        public TResult(Exception exception)
        {
            Success = false;
            ErrorCode = 1;
            ErrorMessage = exception.Message;
        }

        public TResult()
        {
        }

    }
}