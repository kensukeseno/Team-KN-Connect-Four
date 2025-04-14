using System;

namespace ConnectFour
{
    public abstract class Player
    {
        public string Name { get; protected set; }
        public char Symbol { get; protected set; }
        public abstract int SelectColumn(ICommunication communication);
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, char symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        public override int SelectColumn(ICommunication communication)
        {
            return communication.AcceptColNum();
        }
    }

    public class AIPlayer : Player
    {
        public AIPlayer(char symbol)
        {
            Name = "AI player";
            Symbol = symbol;
        }
        public override int SelectColumn(ICommunication communication)
        {
            // generate a random number from 1 to 7
            int num;
            Random r = new Random();
            num = r.Next() % 7 + 1;

            communication.DisplayMessage($"{Name} selected column {num}.");
            // pause for a second
            Thread.Sleep(1000);
            return num;
        }
    }

    public class ConnectFourGame
    {
        public char[,] Board;

        // On initialization, create an empty board with 7 columns and 6 rows
        public ConnectFourGame()
        {
            Board = new char[6, 7];
        }

        // Check if there are 4 discs of their symbol/color in a row
        public bool CheckConnection(char symbol)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (Board[row, col] != symbol) continue;

                    // Check horizontally (right)
                    if (col <= 3 &&
                        Board[row, col + 1] == symbol &&
                        Board[row, col + 2] == symbol &&
                        Board[row, col + 3] == symbol)
                    {
                        return true;
                    }

                    // Check vertically (down)
                    if (row <= 2 &&
                        Board[row + 1, col] == symbol &&
                        Board[row + 2, col] == symbol &&
                        Board[row + 3, col] == symbol)
                    {
                        return true;
                    }

                    // Check diagonally (down-right)
                    if (row <= 2 && col <= 3 &&
                        Board[row + 1, col + 1] == symbol &&
                        Board[row + 2, col + 2] == symbol &&
                        Board[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }

                    // Check diagonally (down-left)
                    if (row <= 2 && col >= 3 &&
                        Board[row + 1, col - 1] == symbol &&
                        Board[row + 2, col - 2] == symbol &&
                        Board[row + 3, col - 3] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Check if the board is full
        public bool CheckBoardIsFull()
        {
            for (int col = 0; col < 7; col++)
            {
                if (Board[5, col] == '\0')
                {
                    return false; // if empty space found, no need to check further
                }
            }
            return true; // if loop completes, board is full
        }

        // Add a disk in a column. If the column is full, return false
        public bool AddDisk(int colNum, char symbol)
        {
            for (int row = 0; row < 6; row++)
            {
                // if selected column is full, return false
                if (Board[row, colNum - 1] == '\0')
                {
                    Board[row, colNum - 1] = symbol;
                    return true;
                }
            }
            return false;
        }
    }

    // Define an interface for communication
    public interface ICommunication
    {
        void DisplayMessage(string message);
        int AcceptColNum();
        string AcceptPlayerName();
        void DisplayBoard(char[,] board);
        void ClearScreen();
        bool AskToRestartGame();
        int AskMode();
    }

    // Console implementation of the interface
    public class ConsoleCommunication : ICommunication
    {
        public void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        // Accept a column number
        public int AcceptColNum()
        {
            do
            {
                string input = Console.ReadLine();
                // Check if the input is a number
                if (int.TryParse(input, out int colNum))
                {
                    return colNum;
                }
                DisplayMessage("Enter a number");
            } while (true);
        }

        // Accept a player name
        public string AcceptPlayerName()
        {
            return Console.ReadLine().Trim();
        }

        // Display the current board
        public void DisplayBoard(char[,] board)
        {
            // Console.SetCursorPosition(0, 0); // Keeps the board static
            //text on top of the board
            Console.WriteLine("CONNECT 4 GAME");
            Console.WriteLine("\n  1  2  3  4  5  6  7");

            for (int row = 5; row >= 0; row--)
            {
                Console.Write("|"); // Left border
                for (int col = 0; col < 7; col++)
                {
                    char piece = board[row, col] == '\0' ? '*' : board[row, col];

                    if (piece == 'x')
                        Console.ForegroundColor = ConsoleColor.Red; // Red for X
                    else if (piece == 'o')
                        Console.ForegroundColor = ConsoleColor.Yellow; // Yellow for O

                    Console.Write($" {piece} ");
                    Console.ResetColor(); // Reset console color
                }
                Console.WriteLine("|"); // Right border
            }

            Console.WriteLine("-----------------------"); // Bottom border
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public bool AskToRestartGame()
        {
            do
            {
                Console.WriteLine("Would you like to play again? (Y/N)");
                string input = Console.ReadLine().ToLower();
                // if user chooses anything other than "y", exit the game
                if (input == "y")
                {
                    return true;
                }
                else if (input == "n")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            } while (true);
        }

        public int AskMode() 
        {
            do
            {
                try {
                    Console.WriteLine("How many players are playing? (1 or 2)");
                    int input = int.Parse(Console.ReadLine());
                    // if user chooses anything other than "y", exit the game
                    if (input == 1)
                    {
                        return 1;
                    }
                    else if (input == 2)
                    {
                        return 2;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input!");
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input!");
                }
            } while (true);

        }
    }


    public class ConnectFourController
    {
        public Player[] Players;
        public ConnectFourGame ConnectFourGame;
        public ICommunication Communication;
        public int Turn;
        public int Mode { get; set; }

        // Constructor. Initialize players, a game and a communication media
        public ConnectFourController()

        {
            // Ask a mode of the game
            /*int mode = ICommunication.AskMode();*/

            // Initialize a game
            Players = new Player[2];
            ConnectFourGame = new ConnectFourGame();
            Communication = new ConsoleCommunication();

            // Clear the screen
            Communication.ClearScreen();

            // ask for either 1 player mode or 2 plyaers mode to play
            Mode = Communication.AskMode();

            if(Mode == 1)
            {
                // Ask for human player name
                Communication.DisplayMessage("Enter player name: ");
                Players[0] = new HumanPlayer(Communication.AcceptPlayerName(), 'x');

                // Create an AI player
                Players[1] = new AIPlayer('o');
            }
            else
            {
                // Ask for player names
                Communication.DisplayMessage("Enter player 1 name: ");
                Players[0] = new HumanPlayer(Communication.AcceptPlayerName(), 'x');

                Communication.DisplayMessage("Enter player 2 name: ");
                Players[1] = new HumanPlayer(Communication.AcceptPlayerName(), 'o');
            }

            Communication.ClearScreen(); // Clear previous text before showing the board

            // display board
            Communication.DisplayBoard(ConnectFourGame.Board);

            // Decide who's turn is the first
            Random r = new Random();
            Turn = r.Next() % 2;
        }

        public void NextTurn()
        {
            Turn = ++Turn % 2;
            // Clear the screen
            Communication.ClearScreen();
            // Show the current board
            Communication.DisplayBoard(ConnectFourGame.Board);

            // Ask next player for input
            Communication.DisplayMessage($"\n{Players[Turn].Name}, choose a column from 1-7:");
            // Repeat until player enters a right column
            int colNum;
            do
            {
                colNum = Players[Turn].SelectColumn(Communication);
                if (colNum >= 1 && colNum <= 7 && ConnectFourGame.AddDisk(colNum, Players[Turn].Symbol))
                {
                    break;
                }
                Communication.DisplayMessage("Invalid or full column. Pick again.");
            } while (true);

            // Clear the screen
            Communication.ClearScreen();
            Communication.DisplayBoard(ConnectFourGame.Board);

            // ensure winning move is display before checking winner
            if (CheckConnectFour())
            {
                Communication.DisplayMessage($"\nWinner is {Players[Turn].Name}!\n");

                return;
            }

            // Check if board is full to declare a draw
            if (CheckBoardIsFull())
            {
                Communication.DisplayMessage("\nDraw!\n");
            }
        }

        public bool CheckConnectFour()
        {
            return ConnectFourGame.CheckConnection(Players[Turn].Symbol);
        }

        public bool CheckBoardIsFull()
        {
            return ConnectFourGame.CheckBoardIsFull();
        }

        // Check if players want a next game 
        public bool Restart()
        {
            if (Communication.AskToRestartGame())
            {
                return true;
            }
            else
            {
                Communication.DisplayMessage("Thanks for playing!");
                return false;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            do
            {
                // Start a game
                ConnectFourController connectFourController = new ConnectFourController();

                do
                {
                    connectFourController.NextTurn();

                    if (connectFourController.CheckConnectFour())
                    {
                        break;
                    }

                    if (connectFourController.CheckBoardIsFull())
                    {
                        break;
                    }

                } while (true);


                if (!connectFourController.Restart())
                {
                    break;
                }

            } while (true);
        }
    }
}
