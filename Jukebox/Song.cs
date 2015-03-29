using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jukebox
{
    class Song
    {
        public string Name;
        public string Artist;
        public string URI;
        public int votos;

        public Song(string Name, string Artist, string URI)
        {
            this.Name = Name;
            this.Artist = Artist;
            this.URI = URI;
            votos = 0;
        }
    }
}
