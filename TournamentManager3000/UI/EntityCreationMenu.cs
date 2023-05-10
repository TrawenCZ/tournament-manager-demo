using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Models;

namespace TournamentManager3000.UI
{
    public class EntityCreationMenu
    {
        public void Show(IEntity entity)
        {
            var entityProps = entity.GetType().GetProperties().Where(prop => prop.CanWrite && prop.Name != "Id");
            foreach (var prop in entityProps)
            {
            }
        }
    }
}
