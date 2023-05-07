using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.UI.MainMenu
{
    public class MainMenuData
    {
        public readonly Dictionary<string, MainMenuOpEnum> commandMapper = new Dictionary<string, MainMenuOpEnum>
            {
                {"help", MainMenuOpEnum.Help},
                {"exit", MainMenuOpEnum.Exit},
                {"create-tournament", MainMenuOpEnum.CreateTournament},
                {"show-tournament", MainMenuOpEnum.ShowTournament},
                {"create-team", MainMenuOpEnum.CreateTeam},
                {"show-team", MainMenuOpEnum.ShowTeam},
                {"create-player", MainMenuOpEnum.CreatePlayer},
                {"show-player", MainMenuOpEnum.ShowPlayer}
            };
    }
}
