﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Models
{
    public class Tournament : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Round> Rounds { get; set; } = new List<Round>();
    }
}
