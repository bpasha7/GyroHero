using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mob
{
    /// <summary>
    /// UserName
    /// ReportEmail
    /// Location
    /// 
    /// </summary>
    public class UserSettings
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Vlaue { get; set; }
    }
}
