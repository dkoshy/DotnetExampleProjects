using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client
{
    public class RetryPolicyHandler : DelegatingHandler
    {
        private readonly int _maxRetryPolicy = 2;

        public RetryPolicyHandler()
        {

        }

        public RetryPolicyHandler(int maxRetryPolicy ):base()
        {
            _maxRetryPolicy = maxRetryPolicy;
        }

        public RetryPolicyHandler(HttpMessageHandler httpMessageHandler , int maxretyrPolicy )
            :base(httpMessageHandler)
        {
            _maxRetryPolicy = maxretyrPolicy;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            for(int i= 0; i < _maxRetryPolicy; i++)
            {
                response =  await  base.SendAsync(request,  cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }
}
