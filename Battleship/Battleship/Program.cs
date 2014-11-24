using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            BattleShipGame game = new BattleShipGame();
            
        }

    }

    class BattleShipGame
    {
        public BattleShipGame()
        {
            // Init ships
        }
    }

    class Ship
    {
        public enum ShipType
        {
            Carrier, Battleship, Cruiser, Submarine, Minesweeper
        }

        public ShipType Type { get; set; }
        public List<Point> occupiedPoints = new List<Point>();
        private int _length;

        public int Length
        {
            get{
                switch (Type)
                {
                    case ShipType.Carrier: return 5;
                    case ShipType.Battleship: return 4;
                    case ShipType.Cruiser: return 3;
                    case ShipType.Submarine: return 3;
                    case ShipType.Minesweeper: return 2;
                }
                return -1;
            }
        }
        public bool isDestroyed { get; set; }

        public Ship(ShipType shipType)
        {
            this.Type = shipType;
        }
    }

    class Point {
        public int xVal { get; set; }
        public int yVal { get; set; }
        public Point(int x, int y) {
            this.xVal = x;
            this.yVal = y;
        }
    }
}
