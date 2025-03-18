namespace ConnectFour
{
    public abstract class Player
    {
        public string Name { get; protected set; }
        public char Symbol { get; protected set; }
        public bool human { get; protected set; }
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, char symbol)
        {
            Name = name;
            Symbol = symbol;
            human = true;
        }
    }

    public class ConnectFourGame
    {
        public char[,] board;

        // On initialization, create an empty board with 7 columns and 6 rows
        public ConnectFourGame()
        {
            board = new char[6, 7];
        }

        // Check if there are 4 discs of their symbol/color in a row
        public bool CheckConnection(char symbol)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (board[row, col] != symbol) continue;

                    // Check horizontally (right)
                    if (col <= 3 &&
                        board[row, col + 1] == symbol &&
                        board[row, col + 2] == symbol &&
                        board[row, col + 3] == symbol)
                    {
                        return true;
                    }

                    // Check vertically (down)
                    if (row <= 2 &&
                        board[row + 1, col] == symbol &&
                        board[row + 2, col] == symbol &&
                        board[row + 3, col] == symbol)
                    {
                        return true;
                    }

                    // Check diagonally (down-right)
                    if (row <= 2 && col <= 3 &&
                        board[row + 1, col + 1] == symbol &&
                        board[row + 2, col + 2] == symbol &&
                        board[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }

                    // Check diagonally (down-left)
                    if (row <= 2 && col >= 3 &&
                        board[row + 1, col - 1] == symbol &&
                        board[row + 2, col - 2] == symbol &&
                        board[row + 3, col - 3] == symbol)
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
                if (board[5, col] == '\0') 
                {
                    return false; // if empty space found, no need to check further
                }
            }
            return true; // if loop completes, board is full
        }

        // Add a disk in a column. If the column is full, return false
        // changed from if-else to "for" for more readability and efficiency 
        public bool AddDisk(int colNum, char symbol)
        {
            for (int row = 0; row < 6; row++) 
            {
                // if selected column is full, return false
                if (board[row, colNum - 1] == '\0')
                {
                    board[row, colNum - 1] = symbol;
                    return true;
                }
            }
            return false; 
        }
    }

    public class ConsoleCommunication
    {
        public void ShowMessageOnConsole(string msg)
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
                ShowMessageOnConsole("Enter a number");
            } while (true);
        }

        // Accept a player name
        public string AcceptPlayerName()
        {
            return Console.ReadLine();
        }

        // Display the current board
        public void DisplayBoard(char[,] board)
        {
            Console.SetCursorPosition(0, 0); // Keeps the board static

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
    }

    
    public class ConnectFourController
    {
        public Player[] players;
        public ConnectFourGame connectFourGame;
        public ConsoleCommunication communication;
        public int turn = -1;

        // Constructor. Initialize players, a game and a communication media
        public ConnectFourController()
   
        {
            players = new Player[2];
            connectFourGame = new ConnectFourGame();
            communication = new ConsoleCommunication();

            // Ask for player names first
            communication.ShowMessageOnConsole("Enter player 1 name: ");
            players[0] = new HumanPlayer(communication.AcceptPlayerName(), 'x');

            communication.ShowMessageOnConsole("Enter player 2 name: ");
            players[1] = new HumanPlayer(communication.AcceptPlayerName(), 'o');

            Console.Clear(); // Clear previous text before showing the board
            //text on top of the board
            Console.WriteLine("CONNECT 4 GAME");
           
            // display board
            communication.DisplayBoard(connectFourGame.board);
        }


        public void NextTurn()
        {
            turn = ++turn % 2;
            // Show the current board
            communication.DisplayBoard(connectFourGame.board);

            // Ask next player for input
            communication.ShowMessageOnConsole($"\n{players[turn].Name}, choose a column from 1-7:");
            // Repeat until player enters a right column
            int colNum;
            do
            {
                colNum = communication.AcceptColNum();
                if (colNum >= 1 && colNum <= 7 && connectFourGame.AddDisk(colNum, players[turn].Symbol))
                {
                    break;
                }
                communication.ShowMessageOnConsole("Invalid or full column. Pick again."); //
            } while (true);

            communication.DisplayBoard(connectFourGame.board);

            // ensure winning move is display before checking winner
            if (CheckConnectFour())
            {
                communication.ShowMessageOnConsole($"\nWinner is {players[turn].Name}!\n");

                return;
            }

            // Check if board is full to declare a draw
            if (CheckBoardIsFull())
            {
                communication.ShowMessageOnConsole("\nDraw!\n");
            }
        }
     
        public bool CheckConnectFour()
        {
            return connectFourGame.CheckConnection(players[turn].Symbol);
        }

        public bool CheckBoardIsFull()
        {
            return connectFourGame.CheckBoardIsFull();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            do
            {
                // start a game
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
               

                // added option to restart the game after each match
                Console.WriteLine("\nWould you like to play again? (Y/N)");
                string input = Console.ReadLine().ToLower();

                // if user chooses anything other than "y", exit the game
                if (input != "y")
                {
                    Console.WriteLine("Thanks for playing!");
                    break;
                }

            } while (true);
        }
    }
}
