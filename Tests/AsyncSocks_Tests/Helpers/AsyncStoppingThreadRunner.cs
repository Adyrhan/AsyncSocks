using AsyncSocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks_Tests.Helpers
{
    public class AsyncStoppingThreadRunner
    {
        private ThreadRunner runner;

        public AsyncStoppingThreadRunner(ThreadRunner runner)
        {
            this.runner = runner;
        }

        public async void Stop()
        {
            await Task.Run(() => runner.Stop());
        }
    }
}
