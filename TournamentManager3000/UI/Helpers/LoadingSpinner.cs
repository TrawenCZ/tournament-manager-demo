namespace TournamentManager3000.UI.Helpers
{
    public class LoadingSpinner
    {
        private int _counter;
        private CancellationToken _ct;

        public async Task Start(CancellationToken ct)
        {
            _ct = ct;
            _counter = 0;
            Console.CursorVisible = false;
            await Spin();
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
                try { await Task.Delay(100, _ct); }
                catch (TaskCanceledException) { Console.CursorVisible = true; break; }
            }
        }
    }
}
