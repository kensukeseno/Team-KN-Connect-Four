namespace ConnectFour
{
    public abstract class Player
    {
        public string Name { get; protected set; }
        public char Symbol { get; protected set; }
        public bool human { get; protected set; }
    }

    public class HumanPlayer : Player {
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
        public ConnectFourGame() {
            board = new char[6, 7];
        }

        // Check if there are 4 discs of their symbol/color in a row
        public bool CheckConnection(char symbol)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    // Check horizontally (right)
                    if (col <= 7 - 4 && board[row, col] == symbol && 
                        board[row, col + 1] == symbol && board[row, col] == symbol && 
                        board[row, col + 2] == symbol && board[row, col] == symbol && board[row, col + 3] == symbol)
                    {
                        return true;
                    }

                    // Check vertically (down)
                    if (row <= 6 - 4 && board[row, col] == symbol && 
                        board[row + 1, col] == symbol && board[row, col] == symbol && 
                        board[row + 2, col] == symbol && board[row, col] == symbol && board[row + 3, col] == symbol)
                    {
                        return true;
                    }

                    // Check diagonally (down-right)
                    if (row <= 6 - 4 && col <= 7 - 4 && board[row, col] == symbol && 
                        board[row + 1, col + 1] == symbol && board[row, col] == symbol && 
                        board[row + 2, col + 2] == symbol && board[row, col] == symbol && board[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }

                    // Check diagonally (down-left)
                    if (row <= 6 - 4 && col >= 3 && board[row, col] == symbol && 
                        board[row + 1, col - 1] == symbol && board[row, col] == symbol && 
                        board[row + 2, col - 2] == symbol && board[row, col] == symbol && board[row + 3, col - 3] == symbol)
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
            if (board[5,0] != '\0' && board[5, 1] != '\0' && 
                board[5, 2] != '\0' && board[5, 3] != '\0' && 
                board[5, 4] != '\0' && board[5, 5] != '\0' && board[5, 6] != '\0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Add a dick in a column. If the column is full, return false
        public bool AddDisk(int colNum, char symbol)
        {
            // if the selected column is full, return false
            if (board[5, colNum - 1] != '\0')
            {
                return false;
            } else {
                if(board[0, colNum - 1] == '\0')
                {
                    board[0, colNum - 1] = symbol;
                }
                else if (board[1, colNum - 1] == '\0')
                {
                    board[1, colNum - 1] = symbol;
                }
                else if (board[2, colNum - 1] == '\0')
                {
                    board[2, colNum - 1] = symbol;
                }
                else if (board[3, colNum - 1] == '\0')
                {
                    board[3, colNum - 1] = symbol;
                }
                else if (board[4, colNum - 1] == '\0')
                {
                    board[4, colNum - 1] = symbol;
                }
                else if (board[5, colNum - 1] == '\0')
                {
                    board[5, colNum - 1] = symbol;
                }
                return true;
            }
            
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
                // Check if the input is number
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
        public void DiplayBoard(char[,] board)
        {
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    if (board[5 - i, j] != '\0')
                    {
                        Console.Write(board[5 - i, j] + " ");
                    }
                    else
                    {
                        Console.Write("# ");
                    }
                }
                Console.WriteLine("");
            }
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
            communication.ShowMessageOnConsole("Enter a player 1 name: ");
            players[0] = new HumanPlayer(communication.AcceptPlayerName(), 'x');
            communication.ShowMessageOnConsole("Enter a player 2 name: ");
            players[1] = new HumanPlayer(communication.AcceptPlayerName(), 'o');
        }

        public void NextTurn()
        {
            turn = ++turn % 2;
            // Show the current board
            communication.DiplayBoard(connectFourGame.board);
            // Ask the next player for an input
            communication.ShowMessageOnConsole($"{players[turn].Name}, choose a culomn.");
            // Repeat until player enters a right culomn
            do
            {
                int colNum;
                do
                {
                     colNum = communication.AcceptColNum();
                    // Column number validation
                    if (colNum >= 1 && colNum <= 7)
                    {
                        break;
                    }
                    communication.ShowMessageOnConsole("Enter a number between 1 and 7");
                } while (true);


                if(connectFourGame.AddDisk(colNum, players[turn].Symbol))
                {
                    break;
                }
                communication.ShowMessageOnConsole("Enter a right input");
            } while (true);
        }

        // Check if there is a connect four
        public bool CheckConnectFour()
        {
            if (connectFourGame.CheckConnection(players[turn].Symbol))
            {
                communication.ShowMessageOnConsole($"Winner is {players[turn].Name}");
                return true;
            }
            else
            {
                return false;
            }
        }

        // Check if the board is full
        public bool CheckBoardIsFull()
        {
            if (connectFourGame.CheckBoardIsFull())
            {
                communication.ShowMessageOnConsole("Draw");
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize a game
            ConnectFourController connectFourController = new ConnectFourController();
            do
            {
                // Next player
                connectFourController.NextTurn();
                // Check the game status
                if (connectFourController.CheckConnectFour())
                {
                    break;
                };
                // Check if the board is full
                if (connectFourController.CheckBoardIsFull())
                {
                    break;
                }
            }while(true);
        }
    }
}
