using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.UI.Helpers
{
    public class LoadingSpinner
    {
        private int _counter;
        private readonly CancellationTokenSource _cts;

        public LoadingSpinner(CancellationTokenSource cts)
        {
            _cts = cts;
        }

        public async Task Start()
        {
            _counter = 0;
            Console.CursorVisible = false;
            Console.Write(" ");
            await Spin();
        }

        public void Stop()
        {
            _counter = 0;
            _cts.TryReset();
            Console.CursorVisible = true;
        }

        private async Task Spin()
        {
            while (true)
            {
                _counter++;
                switch (_counter % 4)
                {
                    case 0: Console.Write("/"); break;
                    case 1: Console.Write("-"); break;
                    case 2: Console.Write("\\"); break;
                    case 3: Console.Write("|"); break;
                }
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                try { await Task.Delay(100, _cts.Token); }
                catch (TaskCanceledException) { Stop(); break; }
            }
        }
    }
}
