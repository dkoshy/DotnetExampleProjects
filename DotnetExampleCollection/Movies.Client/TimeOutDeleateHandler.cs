using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client
{
   public  class TimeOutDeleateHandler : DelegatingHandler
    {
       
        public TimeSpan _TimeOut = new TimeSpan(0, 0, 30);

        public TimeOutDeleateHandler()
        {

        }

        public TimeOutDeleateHandler(TimeSpan timeOut)
        {
            _TimeOut = timeOut;
        }

        public TimeOutDeleateHandler(HttpMessageHandler httpMessageHandler, TimeSpan timeOut)
            :base(httpMessageHandler)
        {
            _TimeOut = timeOut;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            var linkedCanselationSourceToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            linkedCanselationSourceToken.CancelAfter(_TimeOut);

            try
            {
                 response = await base.SendAsync(request, linkedCanselationSourceToken.Token);
            }
            catch(OperationCanceledException ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException($"API is tomed out -----> {ex}");
                }

                throw;
            }

            return response;

        }
    }
}
