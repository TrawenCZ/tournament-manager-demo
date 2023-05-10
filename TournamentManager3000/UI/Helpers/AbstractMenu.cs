using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.UI.Helpers
{
    public abstract class AbstractMenu
    {
        private LoadingSpinner _loadingSpinner = new LoadingSpinner();
        public abstract Task<bool> TryExecute();

        public async Task WaitForLoad(Task task)
        {
            var ct = new CancellationTokenSource();
            var loadingTask = _loadingSpinner.Start(ct.Token);
            await task;
            ct.Cancel();
        }
    }
}
