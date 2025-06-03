// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

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
            UsedSize = usedSize;
            ErrorMessage = errorMessage;
        }

        public static Result Ok(uint usedSize) => new Result(true, usedSize, string.Empty);
        public static Result Fail(string message) => new Result(false, 0, message);
    }

    public class InitializeResult
    {
        public bool Success { get; }

        public string ErrorMessage { get; }

        private InitializeResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static InitializeResult Ok() => new InitializeResult(true, string.Empty);
        public static InitializeResult Fail(string message) => new InitializeResult(false, message);
    }
}
