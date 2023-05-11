using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000.UI.PlayerMenu
{
    public class PlayerMenuExec : AbstractMenu
    {
        private readonly PlayerMenuData _data;
        public PlayerMenuExec(PlayerMenuData data)
        {
            _data = data;
        }
        public override Task<bool> TryExecute(out string messageToPrint)
        {
            messageToPrint = "Player Menu";
            return Task.FromResult(true);
        }
    }
}
