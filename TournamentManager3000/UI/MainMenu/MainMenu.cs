﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000.UI.MainMenu
{
    public class MainMenu : AbstractMenu
    {
        private readonly MainMenuData _data;
        public MainMenu(MainMenuData data) 
        {
            _data = data;
        }

        public override Task<bool> TryExecute()
        {
            
        }
    }
    }
}
