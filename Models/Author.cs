using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookquery.Models
{
    /// <summary>
    /// Segédosztály a könyvek szerzőinek linkjei lekérdezésének fogadására
    /// </summary>
    public class Author
    {
        public List<Links> Links { get; set; } = new List<Links>();
    }

    /// <summary>
    /// Szerzők honlapjainak adatai
    /// </summary>
    public class Links
    {
        public string URL { get; set; } = "";
        public string Title { get; set; } = "";
    }
}
