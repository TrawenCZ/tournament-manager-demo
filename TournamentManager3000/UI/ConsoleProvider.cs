using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000.UI
{
    public interface ConsoleProvider
    {
        private AbstractMenu _currentMenu;

        public ConsoleProvider(AbstractMenu currentMenu)
        {
            _currentMenu = currentMenu;
        }
    }
}
