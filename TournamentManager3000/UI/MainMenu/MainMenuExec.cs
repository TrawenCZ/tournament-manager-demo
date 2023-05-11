using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000.UI.MainMenu
{
    public class MainMenuExec : AbstractMenu
    {
        private readonly MainMenuData _data;
        public MainMenuExec(MainMenuData data) 
        {
            _data = data;
        }

        public override Task<bool> TryExecute(out string messageToPrint)
        {
            
        }
    }
    }
}
