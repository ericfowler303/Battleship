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
            BattleShipGame game = new BattleShipGame(10,10);


            Console.ReadKey();
        }

    }

    class BattleShipGame
    {
        private int GridX { get; set; }
        private int GridY { get; set; }
        List<Ship> listOfShips = new List<Ship>();
        List<Point> userMoves = new List<Point>();
        public bool AllShipsDestroyed
        {
            get{return listOfShips.All(x => x.isDestroyed == true);}
        }
        public int CombatRound { get; set; }

        public BattleShipGame(int xLength, int yLength)
        {
            this.GridX = xLength;
            this.GridY = yLength;
            // Init ships
            InitShips();
        }
        public void DrawGrid()
        {
            // Loop through every coordinate in the grid
            for (int y = 0; y < GridY; y++)
            {
                for (int x = 0; x < GridX; x++)
                {
                    // Figure out if any ship or user action is in this coordinate
                    if (IsShipHere(x, y) && IsUserMoveHere(x, y))
                    {
                        // User hit
                        Console.Write("[X]");
                    }
                    else if (!IsShipHere(x, y) && IsUserMoveHere(x, y))
                    {
                        // User miss
                        Console.Write("[O]");
                    }
                    else if (IsShipHere(x,y))
                    {
                        // ship here
                        Console.Write("[_]");

                    } else {
                        // Nothing here
                        Console.Write("[ ]");
                    }
                }
                Console.Write("\n");
            }
            // Print status messages here
        }

        private bool IsUserMoveHere(int x, int y)
        {
            return userMoves.Any(b => b.xVal == x && b.yVal == y);
        }

        private bool IsShipHere(int x, int y)
        {
            return listOfShips.SelectMany(a => a.occupiedPoints).Any(b => b.xVal == x && b.yVal == y);
        }

        private void InitShips()
        {
            Ship ship1 = new Ship(Ship.ShipType.Carrier);
            ship1.occupiedPoints.Add(new Point(0,0));
            ship1.occupiedPoints.Add(new Point(0, 1));
            ship1.occupiedPoints.Add(new Point(0, 2));
            ship1.occupiedPoints.Add(new Point(0, 3));
            ship1.occupiedPoints.Add(new Point(0, 4));

            Ship ship2 = new Ship(Ship.ShipType.Battleship);
            ship2.occupiedPoints.Add(new Point(2, 0));
            ship2.occupiedPoints.Add(new Point(3, 0));
            ship2.occupiedPoints.Add(new Point(4, 0));
            ship2.occupiedPoints.Add(new Point(5, 0));

            Ship ship3 = new Ship(Ship.ShipType.Submarine);
            ship3.occupiedPoints.Add(new Point(1, 8));
            ship3.occupiedPoints.Add(new Point(2, 8));
            ship3.occupiedPoints.Add(new Point(3, 8));

            Ship ship4 = new Ship(Ship.ShipType.Cruiser);
            ship4.occupiedPoints.Add(new Point(4, 6));
            ship4.occupiedPoints.Add(new Point(4, 7));
            ship4.occupiedPoints.Add(new Point(4, 8));

            Ship ship5 = new Ship(Ship.ShipType.Minesweeper);
            ship5.occupiedPoints.Add(new Point(7, 4));
            ship5.occupiedPoints.Add(new Point(7, 5));

            listOfShips.Add(ship1);
            listOfShips.Add(ship2);
            listOfShips.Add(ship3);
            listOfShips.Add(ship4);
            listOfShips.Add(ship5);
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
