using System;
using System.Diagnostics;
using System.Net.Http;

namespace FluentRest.Tests
{
    public class LogInterceptor : IFluentClientInterceptor
    {
        private readonly Action<string> _writer;
        private const string _key = "LogInterceptor:Stopwatch";

        public LogInterceptor(Action<string> writer)
        {
            _writer = writer;
        }


        public HttpRequestMessage TransformRequest(FluentRequest fluentRequest, HttpRequestMessage httpRequest)
        {
            var watch = Stopwatch.StartNew();
            fluentRequest.State[_key] = watch;

            _writer?.Invoke($"Request: {httpRequest}");

            return httpRequest;
        }

        public FluentResponse TransformResponse(HttpResponseMessage httpResponse, FluentResponse fluentResponse)
        {
            var message = $"Response: {httpResponse}";

            var watch = fluentResponse.Request?.GetState<Stopwatch>(_key);
            if (watch != null)
            {
                watch.Stop();
                message += $"; Time: {watch.ElapsedMilliseconds} ms";
            }

            _writer?.Invoke(message);

            return fluentResponse;
        }
    }
}