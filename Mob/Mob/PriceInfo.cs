using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Data;

namespace Mob
{
    public class PriceInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// продолжительность(строка)
        /// </summary>
        [MaxLength(15)]
        public string Name { get; set; }
        public decimal Price{ get; set; }
        /// <summary>
        /// Время проката
        /// </summary>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// Types:
        /// G - Gyro; C-Cycle
        /// </summary>
        public string Vehicle { get; set; }
    }
}
