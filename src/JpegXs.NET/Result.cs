using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegXs.NET
{
    public class Result
    {
        public bool Success { get; }

        public uint UsedSize { get; }

        public string ErrorMessage { get; }

        private Result(bool success, uint usedSize, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static Result Ok(uint usedSize) => new Result(true, usedSize, string.Empty);
        public static Result Fail(string message) => new Result(false, 0, message);
    }
}
