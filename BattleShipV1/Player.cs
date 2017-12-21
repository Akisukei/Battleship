using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipV1
{
    class Player
    {
        private string name;
        private int wins;
        private int loses;

        public Player(string name, string win, string lose)
        {
            this.name = name;
            this.wins = Int32.Parse(win);
            this.loses = Int32.Parse(lose);
        }

        public string getName()
        {
            return name;
        }

        public int getWins()
        {
            return wins;
        }

        public int getLoses()
        {
            return loses;
        }

        public void incrementWins()
        {
            wins++;
        }

        public void incrementLoses()
        {
            loses++;
        }

        public string toString()
        {
            return name + "," + wins + "," + loses;
        }
    }
}

