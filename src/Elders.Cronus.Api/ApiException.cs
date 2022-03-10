using System;

namespace Elders.Cronus.Api
{
    internal class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public string Content { get; set; }
    }
}
