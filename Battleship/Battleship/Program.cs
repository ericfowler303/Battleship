using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            BattleShipGame game = new BattleShipGame(10,10);
            game.PlayGame();

            Console.ReadKey();
        }

    }

    class BattleShipGame
    {
        private string userMessage = string.Empty;
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

        public void PlayGame()
        {
            List<string> splitInput = new List<string>();
            
            // Keep playing the game until all ships have been destroyed
            while (listOfShips.Any(x => x.isDestroyed == false))
            {
                // Set to -1 to use as a filter to see if there was good user input
                int userX = -1;
                int userY = -1;

                // Draw the grid
                DrawGrid();

                // Print the user a message about recent sinkings if there is one
                if (userMessage != string.Empty)
                {
                    Console.WriteLine(userMessage);
                    System.Threading.Thread.Sleep(1500);
                    userMessage = string.Empty;
                }

                // Get user input in the form of x,y
                // Use .Replace and .Split to sanatize user input
                Console.Write("Please input new x,y coordinates to hit: ");
                string userInput = Console.ReadLine();
                if(userInput.Contains(',')){
                    // Mabye valid input
                    splitInput = userInput.Replace(" ", string.Empty).Split(',').ToList();
                    if (splitInput.Count == 2 && Regex.IsMatch(splitInput[0], @"^\d+$") && Regex.IsMatch(splitInput[1], @"^\d+$"))
                    {
                        // Proper input
                        userX = int.Parse(splitInput[0]);
                        userY = int.Parse(splitInput[1]);
                    } 
                }
                else {
                    // Invalid input, let the user see this then restart the turn
                    Console.WriteLine("Invalid Input");
                    System.Threading.Thread.Sleep(1500);
                }

                // if we have valid userinput (userX & userY)
                // add their move and see if they hit anything
                if (userX != -1 && userY != -1)
                {
                    userMoves.Add(new Point(userX, userY)); CombatRound++;
                    CheckShipsForDestruction();
                }
            }

            // End of game

        }

        public void CheckShipsForDestruction()
        {
            foreach (Ship aShip in listOfShips)
            {
                HasShipBeenDestroyed(aShip);
            }
        }
        private void HasShipBeenDestroyed(Ship aShip)
        {
            // Only check a ship if it hasn't been destroyed yet
            if (!aShip.isDestroyed)
            {
                // See if all of the ship's points have been hit by the user
                if (aShip.occupiedPoints.Intersect(userMoves).Count() == aShip.Length)
                {
                    aShip.isDestroyed = true;
                    // send the user a message that they destroyed this ship
                    switch (aShip.Type)
                    {
                        case Ship.ShipType.Carrier: userMessage = "You just sank the Carrier"; break;
                        case Ship.ShipType.Battleship: userMessage = "You just sank the Battleship"; break;
                        case Ship.ShipType.Cruiser: userMessage = "You just sank the Cruiser"; break;
                        case Ship.ShipType.Submarine: userMessage = "You just sank the Submarine"; break;
                        case Ship.ShipType.Minesweeper: userMessage = "You just sank the Minesweeper"; break;
                    }
                }
            }
        }

        public void DrawGrid()
        {
            Console.Clear();
            Console.WriteLine("Battleship       Ships Remaining: {0}", listOfShips.Where(g=>g.isDestroyed == false).Count());
            // Loop through every coordinate in the grid
            for (int y = 0; y < GridY; y++)
            {
                for (int x = 0; x < GridX; x++)
                {
                    // Figure out if any ship or user action is in this coordinate
                    if (IsShipHere(x, y) && IsUserMoveHere(x, y))
                    {
                        // User hit
                        
                        Ship shipAtCoord = listOfShips.Where(a=>a.occupiedPoints.Contains(new Point(x,y))).First();
                        if (shipAtCoord.isDestroyed)
                        { // Print a different color if a ship has been sunk on it's points
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("[X]");
                            Console.ResetColor();
                        }
                        else
                        { // Print generic hit if the ship is still floating
                            // Set the colors for every normal square
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("[X]");
                            Console.ResetColor();
                        }
                    }
                    else if (!IsShipHere(x, y) && IsUserMoveHere(x, y))
                    {
                        // User miss
                        // Set the colors for every normal square
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[O]");
                        Console.ResetColor();
                    }
                    /*else if (IsShipHere(x,y))
                    {
                        // ship here for debugging purposes
                        Console.Write("[_]");

                    }*/ 
                    else {
                        // Nothing here
                        // Set the colors for every normal square
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[ ]");
                        Console.ResetColor();
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

        private bool IsPointOnMap(int x, int y)
        {
            return (x>=0 && x<this.GridX) && (y>=0 && y<this.GridY);
        }

        private void InitShips()
        {
            Random rng = new Random();

            // Initialize each type of ship starting from largest to smallest
            for (int i = 0; i < 5; i++)
            {
                bool isPlaced = false;
                // Keep looking for a new placement for the ship until one is found
                while (!isPlaced)
                {
                    Ship aShip = new Ship((Ship.ShipType)i);
                    // Get a new random starting coordinate
                    int startingX = rng.Next(0, this.GridX);
                    int startingY = rng.Next(0, this.GridY);
                    
                    // Decide if this ship will go horizotal or vertical
                    Ship.ShipDirecton direction = (Ship.ShipDirecton)rng.Next(2);
                    switch (direction)
                    {
                        case Ship.ShipDirecton.Horizontal:
                            int validSpaceCounterX = 0;
                            for (int xval = startingX; xval < (startingX+aShip.Length); xval++)
                            {
                                // Count the spaces where there is no ship
                                if (!IsShipHere(xval, startingY) && IsPointOnMap(xval,startingY)) { validSpaceCounterX++; }
                            }
                            
                            // If the amount of valid spaces is the same as the length of the ship
                            // then this is a valid place to put the ship
                            if (validSpaceCounterX == aShip.Length)
                            {
                                // Fill in the points the ship will occupy
                                for (int xval = startingX; xval < (startingX + aShip.Length); xval++)
                                {
                                    aShip.occupiedPoints.Add(new Point(xval, startingY));
                                }
                                // Add the completed ship to the list of ships
                                listOfShips.Add(aShip);
                                isPlaced = true;
                            }                            
                            break;
                        case Ship.ShipDirecton.Vertical:
                            int validSpaceCounterY = 0;
                            for (int yval = startingY; yval < (startingY+aShip.Length); yval++)
                            {
                                // Count the spaces where there is no ship
                                if (!IsShipHere(startingX, yval) && IsPointOnMap(startingX,yval)) { validSpaceCounterY++; }
                            }
                            
                            // If the amount of valid spaces is the same as the length of the ship
                            // then this is a valid place to put the ship
                            if (validSpaceCounterY == aShip.Length)
                            {
                                // Fill in the points the ship will occupy
                                for (int yval = startingY; yval < (startingY + aShip.Length); yval++)
                                {
                                    aShip.occupiedPoints.Add(new Point(startingX,yval));
                                }
                                // Add the completed ship to the list of ships
                                listOfShips.Add(aShip);
                                isPlaced = true;
                            }  
                            break;
                    }

                }
            }
        }
    }

    class Ship
    {
        public enum ShipType
        {
            Carrier, Battleship, Cruiser, Submarine, Minesweeper
        }
        public enum ShipDirecton
        {
            Horizontal, Vertical
        }

        public ShipType Type { get; set; }
        public List<Point> occupiedPoints = new List<Point>();

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

    class Point : IEquatable<Point>
    {
        public int xVal { get; set; }
        public int yVal { get; set; }
        public Point(int x, int y) {
            this.xVal = x;
            this.yVal = y;
        }

       
        /// <summary>
        ///  Equal method is needed for IEquatable
        ///  This is needed to compare Points using Intersect
        /// </summary>
        /// <param name="otherPoint">The other Point to compare</param>
        /// <returns>true if the x and y values are the same</returns>
        public bool Equals(Point otherPoint)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(otherPoint, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, otherPoint)) return true;

            //Check whether the products' properties are equal.
            return this.xVal.Equals(otherPoint.xVal) && this.yVal.Equals(otherPoint.yVal);
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public override int GetHashCode()
        {
            //Get hash code for the Name field if it is not null.
            int hashXVal = xVal.GetHashCode();
            int hashYVal = yVal.GetHashCode();

            //Calculate the hash code for the Point
            return hashXVal ^ hashYVal;
        }
    }
}
