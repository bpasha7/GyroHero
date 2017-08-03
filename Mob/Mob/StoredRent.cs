using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mob
{
    public class StoredRent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Rent { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
