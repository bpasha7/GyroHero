using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mob.Dto
{
    public class Error
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Invoker { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Invoker}: {Message} ({Date})\n";
        }
    }
}
