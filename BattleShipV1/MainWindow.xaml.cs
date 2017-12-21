using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;
using System.IO;

//Disclaimer: All images,sounds,resources from the Resources folder are copyrights
//belongs to their respective owners especially to Milton Bradley. No copyright infringement intended. 
//Yes you read it right.
namespace BattleShipV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /**
         * Initialize and loads the playerList from txt file
         */
        public MainWindow()
        {
            InitializeComponent();

            loadPlayerList();
        }

        #region Global Variables
        private int ctrShipPlacement;
        private int shipDestroyed;
        private RadioButton rbButtons;
        private SoundPlayer main = new SoundPlayer(BattleShipV1.Properties.Resources.intro);
        private SoundPlayer splash = new SoundPlayer(BattleShipV1.Properties.Resources.waterSplash);
        private SoundPlayer damage = new SoundPlayer(BattleShipV1.Properties.Resources.hit);
        private SoundPlayer cont = new SoundPlayer(BattleShipV1.Properties.Resources._continue);
        private Random rnd = new Random();
        private List<Player> playerList = new List<Player>(10);
        private int currentPlayer;

        private int[,] arrPlayer;
        private int[,] arrComputer;

        private bool aPlayerHasWon;
        private bool shipGotHit;
        private bool outOfBounds;

        private List<int> xPatternList = new List<int>();
        private List<int> yPatternList = new List<int>();

        private bool playerDestroyGotHit;
        private bool playerCruiserGotHit;
        private bool playerSubmarineGotHit;
        private bool playerBattleshipGotHit;
        private bool playerAircraftGotHit;

        private int playerDestroyer;
        private int playerCruiser;
        private int playerSubmarine;
        private int playerBattleship;
        private int playerAircraft;

        private int computerDestroyer;
        private int computerCruiser;
        private int computerSubmarine;
        private int computerBattleship;
        private int computerAircraft;

        private int playerTurnCtr;
        private int shipHitCtr;
        private int x1, y1, x2, y2;
        private int computerShipValue;
        #endregion

        /**
         * Load method that runs on the program everytime when it starts.
         */
        private void load(object sender, RoutedEventArgs e)
        {
            //Initialized to 0 (default value of cleared grid)
            setGame();

            grpPlayer.IsEnabled = false; //Making it false so the user will put ship first
            main.PlayLooping();
        }

        /**
         * Sets all the necessary elements and the game when it first loaded, new game and reset game.
         */
        private void setGame()
        {
            ctrShipPlacement = 0;
            arrPlayer = new int[10, 10];
            arrComputer = new int[10, 10];
            grpPlayer.IsEnabled = true;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    arrPlayer[i, j] = 0;
                    arrComputer[i, j] = 0;
                }

            xPatternList.Clear();
            yPatternList.Clear();
            setPatternArrayList();

            aPlayerHasWon = false;
            shipGotHit = false;
            playerTurnCtr = 0;

            shipHitCtr = 0;
            playerDestroyGotHit = false;
            playerCruiserGotHit = false;
            playerSubmarineGotHit = false;
            playerBattleshipGotHit = false;
            playerAircraftGotHit = false;

            playerDestroyer = 2;
            playerCruiser = 3;
            playerSubmarine = 3;
            playerBattleship = 4;
            playerAircraft = 5;

            computerDestroyer = 2;
            computerCruiser = 3;
            computerSubmarine = 3;
            computerBattleship = 4;
            computerAircraft = 5;

            if (spStartMenu.Visibility == Visibility.Visible)
            {
                menuNew.IsEnabled = false;
                menuReset.IsEnabled = false;
            }
        }

        /**
         * Used for intelligent mode, where patterArrayList is used for saving positions
         * in a pattern. It will set up pattern 1, 2, 3, randomizes each pattern then 
         * merge them by pattern order in one single arrayList and used as a queue later on
        */
        #region setPatternArrayList()
        private void setPatternArrayList()
        {
            int[] arrXPattern1 = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9 };
            int[] arrYPattern1 = { 0, 5, 1, 6, 2, 7, 3, 8, 4, 9, 0, 5, 1, 6, 2, 7, 3, 8, 4, 9 };

            int[] arrXPattern2 = { 0, 0, 1, 1, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 8, 8, 9, 9 };
            int[] arrYPattern2 = { 3, 8, 4, 9, 5, 0, 6, 1, 7, 2, 8, 3, 9, 4, 0, 5, 1, 6 };

            int[] arrXPattern3 = { 0, 0, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9 };
            int[] arrYPattern3 = { 1, 6, 2, 7, 0, 3, 8, 1, 4, 9, 2, 5, 3, 6, 4, 7, 0, 5, 8, 1, 6, 9, 2, 7 };

            randomizeArray(arrXPattern1, arrYPattern1);
            randomizeArray(arrXPattern2, arrYPattern2);
            randomizeArray(arrXPattern3, arrYPattern3);

            mergeXArrayToList(arrXPattern1);
            mergeXArrayToList(arrXPattern2);
            mergeXArrayToList(arrXPattern3);
            mergeYArrayToList(arrYPattern1);
            mergeYArrayToList(arrYPattern2);
            mergeYArrayToList(arrYPattern3);
        }

        private void randomizeArray(int[] array1, int[] array2)
        {
            int temp1, temp2;
            int a, b;
            int arrayLength = array1.Length;

            for (int i = 0; i < arrayLength; i++)
            {
                a = rnd.Next(arrayLength - 1);
                temp1 = array1[a];
                temp2 = array2[a];

                b = rnd.Next(arrayLength - 1);
                array1[a] = array1[b];
                array2[a] = array2[b];

                array1[b] = temp1;
                array2[b] = temp2;
            }
        }

        private void mergeXArrayToList(int[] xArray)
        {
            for (int i = 0; i < xArray.Length; i++)
                xPatternList.Add(xArray[i]);
        }

        private void mergeYArrayToList(int[] yArray)
        {
            for (int i = 0; i < yArray.Length; i++)
                yPatternList.Add(yArray[i]);
        }
        #endregion  //used for intelligent mode pattern

        /**
         * loadPlayerList loads the playerList filled with Player object from nameTextList.txt
         */
        private void loadPlayerList()
        {
            playerList.Clear();

            try
            {
                if (File.Exists("nameTextList.txt"))
                {
                    StreamReader inputFileReader = new StreamReader("nameTextList.txt");
                    string record = null;
                    string[] field = null;

                    while (inputFileReader.Peek() >= 0)
                    {
                        record = inputFileReader.ReadLine();
                        field = record.Split(',');

                        playerList.Add(new Player(field[0], field[1], field[2]));
                    }
                    inputFileReader.Close();
                }
                else
                    Console.WriteLine("File does not exist!");
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
        }

        /**
         * savePlayerListToFile saves the current loaded playerList to nameTextList.txt to be
         * loaded later next time
         */ 
        private void savePlayerListToFile()
        {
            StreamWriter outputFileWriter = null;

            try
            {
                outputFileWriter = new StreamWriter("nameTextList.txt");

                foreach (Player player in playerList)
                    outputFileWriter.WriteLine(player.toString());
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
            finally
            {
                outputFileWriter.Close();
            }
        }

        /**
         * searchForPlayer(string playerName) is used to search if a Player inside playerList
         * exists. If it does, return the index of that existing player, if not, return -1
         */
        private int searchForPlayer(string playerName)
        {
            for (int i = 0; i < playerList.Count; i++)
                if (playerList[i].getName() == playerName)
                    return i;
            return -1;
        }

        /**
         * Method event where user points the mouse pointer and it will clear the textbox.
         */
        private void txtName_MouseEnter(object sender, MouseEventArgs e)
        {
            if (txtName.Text == "Please Enter Your Name Here")
                txtName.Text = "";
        }

        /**
         * Method that will validate player name if the user press enter in the textbox.
         */
        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnEnterValidateName_Click(new object(), new RoutedEventArgs());
            }
        }

        /**
         * Displays which ship has been selected to be put on the grid.
         */
        private void rbShips_Checked(object sender, RoutedEventArgs e)
        {
            rbButtons = sender as RadioButton;

            lblPlayerMessage.Visibility = Visibility.Visible;
            lblPlayerMessage.Content = rbButtons.Content.ToString() + " ship selected." + "\nPut your ship to the grid.";

            if (rbHorizontal.IsChecked == true || rbVertical.IsChecked == true)
                grpPlayer.IsEnabled = true;
        }

        /**
         * Method event that will determine which row and column the user puts the ship.
         * Will also allow mouse right click and change the orientation.
         */
        private void gridPlacement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image imgClicked = sender as Image;

            int row = (int)imgClicked.GetValue(Grid.RowProperty);
            int column = (int)imgClicked.GetValue(Grid.ColumnProperty);

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    if (rbHorizontal.IsChecked == true)
                        rbVertical.IsChecked = true;
                    else if (rbVertical.IsChecked == true)
                        rbHorizontal.IsChecked = true;
                }

                if (rbHorizontal.IsChecked == true)
                    horizontalPosition(row, column, imgClicked);
                else if (rbVertical.IsChecked == true)
                    verticalPosition(row, column, imgClicked);
            }
        }

        /**
         * Method that determines which ship will be put to the grid horizontally.
         */
        private void horizontalPosition(int row, int col, Image imgClicked)
        {
            if (rbAircraft.IsChecked == true)
            {
                if (col <= 5) placeToGridH(row, col, 5, 5);
                else if (col > 5) labelMessage(0);
            }
            else if (rbBattleship.IsChecked == true)
            {
                if (col <= 6) placeToGridH(row, col, 6, 4);
                else if (col > 6) labelMessage(0);
            }
            else if (rbSubmarine.IsChecked == true)
            {
                if (col <= 7) placeToGridH(row, col, 7, 3);
                else if (col > 7) labelMessage(0);
            }
            else if (rbCruiser.IsChecked == true)
            {
                if (col <= 7) placeToGridH(row, col, 7, 2);
                else if (col > 7) labelMessage(0);
            }
            else if (rbDestroyer.IsChecked == true)
            {
                if (col <= 8) placeToGridH(row, col, 8, 1);
                else if (col > 8) labelMessage(0);
            }
        }

        /**
         * Method that determines which ship will be put to the grid vertically.
         */
        private void verticalPosition(int row, int col, Image imgClicked)
        {
            if (rbAircraft.IsChecked == true)
            {
                if (row <= 5) placeToGridV(row, col, 5, 5);
                else if (row > 5) labelMessage(0);
            }
            else if (rbBattleship.IsChecked == true)
            {
                if (row <= 6) placeToGridV(row, col, 6, 4);
                else if (row > 6) labelMessage(0);
            }
            else if (rbSubmarine.IsChecked == true)
            {
                if (row <= 7) placeToGridV(row, col, 7, 3);
                else if (row > 7) labelMessage(0);
            }
            else if (rbCruiser.IsChecked == true)
            {
                if (row <= 7) placeToGridV(row, col, 7, 2);
                else if (row > 7) labelMessage(0);
            }
            else if (rbDestroyer.IsChecked == true)
            {
                if (row <= 8) placeToGridV(row, col, 8, 1);
                else if (row > 8) labelMessage(0);
            }
        }

        /**
         * Method that will place the ship to grid horizontally.
         */
        private void placeToGridH(int row, int col, int size, int value)
        {
            int temp = col;
            BitmapImage cropped;
            bool hasShip = false;
            int length = 10 - size;
            lblPlayerMessage.Visibility = Visibility.Hidden;
            lblComputerMessage.Visibility = Visibility.Hidden;

            if (rbAircraft.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/aircraftCarrierH.png", UriKind.RelativeOrAbsolute));
            else if (rbBattleship.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/battleshipH.png", UriKind.RelativeOrAbsolute));
            else if (rbSubmarine.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/submarineH.png", UriKind.RelativeOrAbsolute));
            else if (rbCruiser.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/cruiserH.png", UriKind.RelativeOrAbsolute));
            else
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/destroyerH.png", UriKind.RelativeOrAbsolute));

            try
            {
                //checks the initial position if there is a ship
                if (arrPlayer[row, col] != 0)
                    hasShip = true;

                //checks position one from the left if there's a ship
                if ((col - 1) >= 0)
                    if (arrPlayer[row, (col - 1)] != 0)
                        hasShip = true;

                //checks if there is a ship from the right next to each other
                for (int i = 0; i <= length && !(hasShip); i++)
                {
                    if ((col + i) < 10)
                    {
                        if (arrPlayer[row, (col + i)] != 0)
                            hasShip = true;
                    }
                }

                //checks if there is a ship horizontally close to each other
                for (int i = 0; i < length && !(hasShip); i++)
                {
                    if ((col + i) < 10)
                        if (arrPlayer[row + 1, col + i] != 0 || arrPlayer[row - 1, col + i] != 0)
                            hasShip = true;
                }
            }
            catch (Exception e)
            {
                for (int i = 0; i < length && !(hasShip); i++)
                {
                    if (row == 0)
                    {
                        if (arrPlayer[row + 1, col + i] != 0)
                            hasShip = true;
                    }

                    if (row == 9)
                    {
                        if (arrPlayer[row - 1, col + i] != 0)
                            hasShip = true;
                    }
                }
            }
            
            temp = col;

            if (hasShip == false)
            {
                for (int i = 0, x = 0; i < length; i++, x += 30, temp++)
                {
                    var image = new Image { Source = new CroppedBitmap(cropped, new Int32Rect(x, 0, 30, 30)) };
                    Grid.SetRow(image, row);
                    Grid.SetColumn(image, temp);
                    image.MouseDown += gridPlacement_MouseDown;
                    gridPlayer.Children.Add(image);
                    arrPlayer[row, temp] = value;
                }
                ctrShipPlacement++;
                labelMessage(2);
                grpPlayer.IsEnabled = false;
                deleteShip();
                checkGridShips();
            }
            else
                labelMessage(1);
        }

        /**
         * Method that will place the ship to grid vertically.
         */
        private void placeToGridV(int row, int col, int size, int value)
        {
            int temp = row;
            BitmapImage cropped;
            bool hasShip = false;
            int length = 10 - size;
            lblPlayerMessage.Visibility = Visibility.Hidden;
            lblComputerMessage.Visibility = Visibility.Hidden;

            if (rbAircraft.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/aircraftCarrierV.png", UriKind.RelativeOrAbsolute));
            else if (rbBattleship.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/battleshipV.png", UriKind.RelativeOrAbsolute));
            else if (rbSubmarine.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/submarineV.png", UriKind.RelativeOrAbsolute));
            else if (rbCruiser.IsChecked == true)
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/cruiserV.png", UriKind.RelativeOrAbsolute));
            else
                cropped = new BitmapImage(new Uri("pack://application:,,,/Resources/Ships/destroyerV.png", UriKind.RelativeOrAbsolute));

            try
            {
                //checks the initial position if there is a ship
                if (arrPlayer[row, col] != 0)
                    hasShip = true;

                //checks position one row above if there's a ship
                if ((row - 1) >= 0)
                    if (arrPlayer[(row - 1), col] != 0)
                        hasShip = true;

                //checks if there is a ship below or if next to each other
                for (int i = 0; i <= length && !(hasShip); i++)
                {
                    if ((row + i) < 10)
                        if (arrPlayer[row + i, col] != 0)
                            hasShip = true;
                }

                //checks if there is a ship vertically close to each other
                for (int i = 0; i < length && !(hasShip); i++)
                {
                    if ((row + i) < 10)
                        if (arrPlayer[row + i, col + 1] != 0 || arrPlayer[row + i, col - 1] != 0)
                            hasShip = true;
                }
            }
            catch (Exception e) 
            {
                for (int i = 0; i < length && !(hasShip); i++)
                {
                    if (col == 0)
                    {
                        if (arrPlayer[row + i, col + 1] != 0)
                            hasShip = true;
                    }

                    if (col == 9)
                    {
                        if (arrPlayer[row + i, col - 1] != 0)
                            hasShip = true;
                    }
                }
            }

            temp = row;

            if (hasShip == false)
            {
                for (int i = 0, y = 0; i < length; i++, y += 30, temp++)
                {
                    var image = new Image { Source = new CroppedBitmap(cropped, new Int32Rect(0, y, 30, 30)) };
                    Grid.SetRow(image, temp);
                    Grid.SetColumn(image, col);
                    image.MouseDown += gridPlacement_MouseDown;
                    gridPlayer.Children.Add(image);
                    arrPlayer[temp, col] = value;
                }
                ctrShipPlacement++;
                labelMessage(2);
                grpPlayer.IsEnabled = false;
                deleteShip();
                checkGridShips();
            }
            else
                labelMessage(1);
        }

        /**
         * Method that hides the ship on the selection when it is placed on the grid.
         */
        private void deleteShip()
        {
            if ((string)rbButtons.Content == "Aircraft")
                aircraftCarrier.Visibility = Visibility.Hidden;
            else if ((string)rbButtons.Content == "Battleship")
                battleship.Visibility = Visibility.Hidden;
            else if ((string)rbButtons.Content == "Submarine")
                submarine.Visibility = Visibility.Hidden;
            else if ((string)rbButtons.Content == "Cruiser")
                cruiser.Visibility = Visibility.Hidden;
            else if ((string)rbButtons.Content == "Destroyer")
                destroyer.Visibility = Visibility.Hidden;

            rbButtons.Visibility = Visibility.Hidden;
            rbButtons.IsChecked = false;
        }

        /**
         * Method that hide the ship container and orientation.
         * It will display game mode and start button.
         */
        private void checkGridShips()
        {
            if (ctrShipPlacement == 5)
            {
                grpShipOrientation.Visibility = Visibility.Hidden;
                grpShips.Visibility = Visibility.Hidden;
                tBlockTip.Visibility = Visibility.Hidden;
                btnStartGame.Visibility = Visibility.Visible;
                grpBoxMode.Visibility = Visibility.Visible;
            }
        }
        
        /**
         * Method that will start the game.
         */
        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            grpShips.Visibility = Visibility.Hidden;
            grpShipOrientation.Visibility = Visibility.Hidden;
            grpBoxMode.Visibility = Visibility.Hidden;
            btnStartGame.Visibility = Visibility.Hidden;
            grpComputer.Visibility = Visibility.Visible;
            wpComputer.Visibility = Visibility.Visible;

            grpComputer.IsEnabled = true;
            grpPlayer.IsEnabled = false;

            setComputerShipPosition(); //this will set the computer's grid
        }

        /**
         * Method that displays the user and AI moves.
         */
        private void labelMessage(int content)
        {
            lblPlayerMessage.Visibility = Visibility.Visible;
            lblComputerMessage.Visibility = Visibility.Visible;
            imgWarning.Visibility = Visibility.Hidden;

            if (content == 0)
                lblPlayerMessage.Content = "Ship is outside the grid. Select again!";
            if (content == 1)
                lblPlayerMessage.Content = "Other ship will be overlap! or \nDon't place ship close to each other";
            if (content == 2)
                lblPlayerMessage.Content = "Ship has been placed to grid!";
            if (content == 5)
                lblPlayerMessage.Content = "Last move: You have missed!";
            if (content == 6)
                lblComputerMessage.Content = "Last move: The AI have missed!";
            if (content == 7)
            {
                lblPlayerMessage.Content = "Last move: You have damaged a ship!";
                imgWarning.Visibility = Visibility.Visible;
            }
            if (content == 8)
                lblComputerMessage.Content = "Last move: The AI have damaged a ship!";
            if (content == 9)
                lblPlayerMessage.Content = "Congratulations! You've won!";
            if (content == 10)
                lblComputerMessage.Content = "Game Over. All of your ships have been sunk.";
        }

        /**
         * Displays what ship has been destroyed in a pop up window.
         */
        private void labelMessage(int content, string shipname)
        {
            popUpMessage p = new popUpMessage();
            p.Owner = this;
            lblPlayerMessage.Visibility = Visibility.Visible;
            lblComputerMessage.Visibility = Visibility.Visible;

            shipDestroyed = content;

            if (content == 3)
            {
                damage.Play();
                p.lblPopUpMessage.Content = "You have destroyed their " + shipname + "!";
                p.ShowDialog();
                lblPlayerMessage.Content = "You have destroyed their " + shipname + "!";
            }
            else if (content == 4)
            {
                damage.Play();
                p.lblPopUpMessage.Content = "The AI have destroyed your " + shipname + "!";
                p.ShowDialog();
                lblComputerMessage.Content = "The AI have destroyed your " + shipname + "!";
            }
        }

        /**
         * An asynchronous method is one that we call to start the lengthy operation
         */
        private async void timer() { await delay(); }

        /**
         * Creates a task that completes after a time delay.
         */
        private async Task delay()
        {
            await Task.Delay(100);
            
            cont.PlayLooping();
        }

        /**
         * Method when the player clicks the AI's grid, it will show if the attack is a hit, 
         * missed, or already shot before.
         */
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image imgClicked = sender as Image;

            int row = (int)imgClicked.GetValue(Grid.RowProperty);
            int column = (int)imgClicked.GetValue(Grid.ColumnProperty);

            BitmapImage hit = new BitmapImage(new Uri("pack://application:,,,/Resources/hit.png", UriKind.RelativeOrAbsolute));
            BitmapImage noHit = new BitmapImage(new Uri("pack://application:,,,/Resources/noHit.png", UriKind.RelativeOrAbsolute));

            var imgHit = new Image { Source = new CroppedBitmap(hit, new Int32Rect(0, 0, 30, 30)) };
            var imgNoHit = new Image { Source = new CroppedBitmap(noHit, new Int32Rect(0, 0, 30, 30)) };

            if (arrComputer[row, column] == 0)
            {
                labelMessage(5); //that player missed
                Grid.SetRow(imgNoHit, row);
                Grid.SetColumn(imgNoHit, column);
                gridComputer.Children.Add(imgNoHit);
                imgNoHit.MouseDown += Image_MouseDown;
                arrComputer[row, column] = -1; //-1 Value of the grid being already shot
                gridPlayer.IsEnabled = false;
                splash.Play(); //Play splash water sound effect
                shipDestroyed = 0;
                popUpMessage p = new popUpMessage();
                p.Owner = this;
                p.lblPopUpMessage.Content = "You missed!";
                p.ShowDialog();
                turnShot();
            }
            else if (arrComputer[row, column] > 0)
            {
                Grid.SetRow(imgHit, row);
                Grid.SetColumn(imgHit, column);
                gridComputer.Children.Add(imgHit);
                imgHit.MouseDown += Image_MouseDown;
                aimForTarget(row, column);
                arrComputer[row, column] = -1; //-1 Value of the grid being already shot
                checkIfShipDestroyed();
                gridPlayer.IsEnabled = false;
                damage.Play(); //Play damage water sound effect
                if (shipDestroyed != 3)
                {
                    shipDestroyed = 0;
                    popUpMessage p = new popUpMessage();
                    p.Owner = this;
                    p.lblPopUpMessage.Content = "You got a hit! Destroy it!";
                    p.ShowDialog();
                }
                turnShot();
            }
            else if (arrComputer[row, column] == -1)
            {
                popUpMessage p = new popUpMessage();
                p.Owner = this;
                p.lblPopUpMessage.Content = "Already made that shot!";
                p.ShowDialog();
            }
        }

        /**
         * After the player uses his turn, it will check if a player has won, if not, increment 
         * turn counter and play AI's turn
         */
        private void turnShot()
        {
            if (!aPlayerHasWon)
            {
                playerTurnCtr++;
                computerTurn();
            }
            else
                displayEndGameScreen();
        }

        /**
         * Method for the computer's turn. It will first check if any ship is damaged. If so, it 
         * will go into killerMode method, else it'll stay in hunterMode until it finds a ship
         */
        private void computerTurn()
        {
            if (playerDestroyGotHit || playerCruiserGotHit || playerSubmarineGotHit ||
                                            playerBattleshipGotHit || playerAircraftGotHit) 
            {
                killerMode();
            }
            else //hunter mode
            {
                shipHitCtr = 0;     //means it hasn't hit a ship

                if (rbIntelligentMode.IsChecked == true)
                {
                    //will keep queueing from patternArrayList until the position is free
                    x1 = getXFromPatternList();
                    y1 = getYFromPatternList();
                    while (spotIsTaken(x1, y1))
                    {
                        x1 = getXFromPatternList();
                        y1 = getYFromPatternList();
                    }

                    aimForTarget(x1, y1);
                    if (shipGotHit)
                        shipHitCtr++;
                }
                else //rbSimpleMode.IsChecked == true
                {
                    //will keep generating random positins until a position is free
                    x1 = rnd.Next(10); //0-9
                    y1 = rnd.Next(10);
                    while (spotIsTaken(x1, y1))
                    {
                        x1 = rnd.Next(10); //0-9
                        y1 = rnd.Next(10);
                    }

                    aimForTarget(x1, y1);
                    if (shipGotHit)
                        shipHitCtr++;
                }
            }

            //after the AI plays, it will check if AI won
            //if so, display end game or else turn counter++ and wait for player's turn
            if (!aPlayerHasWon)
                playerTurnCtr++;
            else
                displayEndGameScreen();
        }

        /**
         * used for intelligent: gets the x position from queue in xPatternList
         */
        private int getXFromPatternList()
        {
            int x = xPatternList[0];
            xPatternList.RemoveAt(0);
            return x;
        }

        /**
         * used for intelligent: gets the y position from queue in yPatternList
         */
        private int getYFromPatternList()
        {
            int y = yPatternList[0];
            yPatternList.RemoveAt(0);
            return y;
        }

        /**
         * Method that displays the attack of the AI on the grid and know if it missed or a hit.
         */
        private void displayFireImg(int x, int y)
        {
            BitmapImage hit = new BitmapImage(new Uri("pack://application:,,,/Resources/hit.png", UriKind.RelativeOrAbsolute));
            BitmapImage noHit = new BitmapImage(new Uri("pack://application:,,,/Resources/noHit.png", UriKind.RelativeOrAbsolute));

            var imgHit = new Image { Source = new CroppedBitmap(hit, new Int32Rect(0, 0, 30, 30)) };
            var imgNoHit = new Image { Source = new CroppedBitmap(noHit, new Int32Rect(0, 0, 30, 30)) };

            popUpMessage p = new popUpMessage();

            if (arrPlayer[x, y] == 0)
            {
                Grid.SetRow(imgNoHit, x);
                Grid.SetColumn(imgNoHit, y);
                gridPlayer.Children.Add(imgNoHit);
                arrPlayer[x, y] = -1;
                gridPlayer.IsEnabled = true;
                splash.Play();
                p.lblPopUpMessage.Content = "AI Justin missed!";
                p.Owner = this;
                p.ShowDialog();
                timer();
            }

            else if (arrPlayer[x, y] > 0)
            {
                Grid.SetRow(imgHit, x);
                Grid.SetColumn(imgHit, y);
                gridPlayer.Children.Add(imgHit);
                arrPlayer[x, y] = -1;
                gridPlayer.IsEnabled = true;
                damage.Play();
                p.lblPopUpMessage.Content = "AI Justin got a hit!";
                p.Owner = this;
                p.ShowDialog();
                timer();
            }
        }

        /**
         * killerMode stays active until no ships are damaged. 
         */
        private void killerMode()
        {
            //if its the first hit, itll just shoot around randomly
            if (shipHitCtr == 1)
            {
                x2 = x1;
                y2 = y1;

                shootAround();
                //while loop keeps randomizing if random position is out of grid or spot taken
                while ((x2 < 0 || x2 > 9) || (y2 < 0 || y2 > 9) || spotIsTaken(x2, y2))
                {
                    x2 = x1;
                    y2 = y1;
                    shootAround();
                }

                aimForTarget(x2, y2);
                if (shipGotHit)
                    shipHitCtr++;

                checkIfShipDestroyed();
            }
            //if it already shot around and got a hit, itll shoot in the direction the 2nd shot was
            else if (shipHitCtr > 1)
            {
                shootNext();

                if (shipGotHit)
                    shipHitCtr++;

                checkIfShipDestroyed();
            }
        }

        /**
         * used by killerMode to randomly generate one of the 4 positions it can shoot to
         */
        private void shootAround()
        {
            int rng = rnd.Next(1, 5); //1-4
            if (rng == 1)
                x2 = x1 + 1;
            else if (rng == 2)
                x2 = x1 - 1;
            else if (rng == 3)
                y2 = y1 + 1;
            else //rng == 4
                y2 = y1 - 1;
        }

        /**
         * a bit hard to understand method where it shoots the spots that is aligned with
         * the first and second shot. It'll keep going in one direction until it misses. If
         * it misses, it'll go the opposite direction until the ship is dead
         */
        private void shootNext()
        {
            if (x1 < x2)
            {
                //check if the next position to shoot is out of grid or spot taken, if not, shoot there
                if ((x1 + shipHitCtr <= 9) && !spotIsTaken(x1 + shipHitCtr, y1))
                    aimForTarget(x1 + shipHitCtr, y1);
                else
                {  //if it's out of grid or taken, then shoot the other way now
                    x2 = x1 - 1;
                    shipHitCtr = 1;
                    aimForTarget(x2, y1);
                }
            }
            else if (x1 > x2)
            {
                if ((x1 - shipHitCtr >= 0) && !spotIsTaken(x1 - shipHitCtr, y1))
                    aimForTarget(x1 - shipHitCtr, y1);
                else
                {
                    x2 = x1 + 1;
                    shipHitCtr = 1;
                    aimForTarget(x2, y1);
                }
            }
            else if (y1 < y2)
            {
                if ((y1 + shipHitCtr <= 9) && !spotIsTaken(x1, y1 + shipHitCtr))
                    aimForTarget(x1, y1 + shipHitCtr);
                else
                {
                    y2 = y1 - 1;
                    shipHitCtr = 1;
                    aimForTarget(x1, y2);
                }
            }
            else if (y1 > y2)
            {
                if ((y1 - shipHitCtr >= 0) && !spotIsTaken(x1, y1 - shipHitCtr))
                    aimForTarget(x1, y1 - shipHitCtr);
                else
                {
                    y2 = y1 + 1;
                    shipHitCtr = 1;
                    aimForTarget(x1, y2);
                }
            }
        }

        /**
         * spotIsTaken(int x, int y) sees if the position it will shoot using x and y is already 
         * taken or not. If it's already used, return true, else return false
         */
        private bool spotIsTaken(int x, int y)
        {
            if (arrPlayer[x, y] >= 0)
                return false;
            else //if arrPlayer[x, y] == -1
                return true;
        }

        /**
         * Player's turn: aim for the position with x and y. If it hits something, see which ship 
         *      got hit and decrement it's part count (it's like an HP bar).
         * AI's turn: same function as the player's turn, except state that a player ship got hit
         *      so it'll enter hunterMode. Also initially state shipGotHit to true and if later it 
         *      sees that the hit is a miss, it'll turn it to false.
         */
        private void aimForTarget(int x, int y)
        {
            if (playerTurnCtr % 2 == 0) //player's turn
            {
                if (arrComputer[x, y] == 1)
                {
                    computerDestroyer--;
                    labelMessage(7);        //notify player damaged a ship
                }
                else if (arrComputer[x, y] == 2)
                {
                    computerCruiser--;      //cruiser has 3 parts. 0 means it's destroyed
                    labelMessage(7);
                }
                else if (arrComputer[x, y] == 3)
                {
                    computerSubmarine--;
                    labelMessage(7);
                }
                else if (arrComputer[x, y] == 4)
                {
                    computerBattleship--;
                    labelMessage(7);
                }
                else if (arrComputer[x, y] == 5)
                {
                    computerAircraft--;
                    labelMessage(7);
                }

            }
            else //if playerTurnCtr % 2 == 1    AI's turn
            {
                shipGotHit = true;

                if (arrPlayer[x, y] == 1)
                {
                    playerDestroyer--;
                    labelMessage(8);        //notify that AI damaged a ship
                    playerDestroyGotHit = true;     //a ship is damaged, go into killerMode next turn
                }
                else if (arrPlayer[x, y] == 2)
                {
                    playerCruiser--;
                    labelMessage(8);
                    playerCruiserGotHit = true;
                }
                else if (arrPlayer[x, y] == 3)
                {
                    playerSubmarine--;
                    labelMessage(8);
                    playerSubmarineGotHit = true;
                }
                else if (arrPlayer[x, y] == 4)
                {
                    playerBattleship--;
                    labelMessage(8);
                    playerBattleshipGotHit = true;
                }
                else if (arrPlayer[x, y] == 5)
                {
                    playerAircraft--;
                    labelMessage(8);
                    playerAircraftGotHit = true;
                }
                else
                {
                    labelMessage(6);        //notify that AI missed
                    shipGotHit = false;
                }

                displayFireImg(x, y);
            }
        }

        /**
         * Player's turn: checks if any ship has 0 parts left, then display that specific ship got 
         *      destroyed.
         * AI's turn: same function as player's turn except turn the destroyed ship got hit to false
         *      so it'll go back to hunterMode next turn.
         * 
         * Afterwards, it'll check if all the ships are destroyed from both players. If the player
         * or AI wins, it'll display an appropriate msg and make aPlayerHasWon = true
         */
        private void checkIfShipDestroyed()
        {
            if (playerTurnCtr % 2 == 0)
            {
                if (computerDestroyer == 0)
                {
                    labelMessage(3, "destroyer");
                    computerDestroyer = -1;     //so it won't display AGAIN after it did once
                }
                else if (computerCruiser == 0)
                {
                    labelMessage(3, "cruiser");
                    computerCruiser = -1;
                }
                else if (computerSubmarine == 0)
                {
                    labelMessage(3, "submarine");
                    computerSubmarine = -1;
                }
                else if (computerBattleship == 0)
                {
                    labelMessage(3, "battleship");
                    computerBattleship = -1;
                }
                else if (computerAircraft == 0)
                {
                    labelMessage(3, "aircraft carrier");
                    computerAircraft = -1;
                }
            }
            else //playerTurnCtr % 2 == 1
            {
                if (playerDestroyer == 0)
                {
                    labelMessage(4, "destroyer");
                    playerDestroyer = -1;           //so it won't display AGAIN after it did once
                    playerDestroyGotHit = false;    //false meaning it's DEAD
                }
                else if (playerCruiser == 0)
                {
                    labelMessage(4, "cruiser");
                    playerCruiser = -1;
                    playerCruiserGotHit = false;
                }
                else if (playerSubmarine == 0)
                {
                    labelMessage(4, "submarine");
                    playerSubmarine = -1;
                    playerSubmarineGotHit = false;
                }
                else if (playerBattleship == 0)
                {
                    labelMessage(4, "battleship");
                    playerBattleship = -1;
                    playerBattleshipGotHit = false;
                }
                else if (playerAircraft == 0)
                {
                    labelMessage(4, "aircraft carrier");
                    playerAircraft = -1;
                    playerAircraftGotHit = false;
                }
            }

            if ((computerDestroyer + computerCruiser + computerSubmarine +
                                computerBattleship + computerAircraft) == -5)
            {
                labelMessage(9);    //display player wins
                aPlayerHasWon = true;
            }
            else if ((playerDestroyer + playerCruiser + playerSubmarine +
                                playerBattleship + playerAircraft) == -5)
            {
                labelMessage(10);   //display AI wins
                aPlayerHasWon = true;
            }
        }

        /**
         * AI will put the ships randomnly
         */
        private void setComputerShipPosition()
        {
            outOfBounds = false;
            computerShipValue = 5;

            for (int i = 0; i < 5; i++, computerShipValue--)
            {
                int orientation = rnd.Next(1, 3);

                if (orientation == 1)
                    computerPositionH(computerShipValue);
                else if (orientation == 2)
                    computerPositionV(computerShipValue);

                if (outOfBounds == true)
                {
                    i--;
                    computerShipValue++;
                    outOfBounds = false;
                }
            }
        }

        /**
         * Positioning the ship in the grid to horizontal position.
         */
        private void computerPositionH(int computerShipValue)
        {
            int row = rnd.Next(0, 9);
            int col = rnd.Next(0, 9);
            int temp = col;
            bool hasShip = false;

            int length = 10 - checkSize(computerShipValue);

            if ((length + col) < 10)
            {
                outOfBounds = false;
                for (int i = 0; i < length && !(hasShip); i++, temp++)
                {
                    if (arrComputer[row, temp] != 0)
                    {
                        hasShip = true;
                        outOfBounds = true;
                    }
                }
            }
            else
            {
                hasShip = true;
                outOfBounds = true;
            }

            if (hasShip == false)
            {
                temp = col;
                for (int i = 0; i < length; i++, temp++)
                    arrComputer[row, temp] = computerShipValue;
            }

        }
        
        /**
         * Positioning the ship in the grid to vertical position.
         */
        private void computerPositionV(int computerShipValue)
        {
            int row = rnd.Next(0, 9);
            int col = rnd.Next(0, 9);
            int temp = row;
            bool hasShip = false;

            int length = 10 - checkSize(computerShipValue);

            if ((length + row) < 10)
            {
                for (int i = 0; i < length && !(hasShip); i++, temp++)
                {
                    if (arrComputer[temp, col] != 0)
                    {
                        hasShip = true;
                        outOfBounds = true;
                    }
                }
            }
            else
            {
                hasShip = true;
                outOfBounds = true;
            }

            if (hasShip == false)
            {
                temp = row;
                for (int i = 0; i < length; i++, temp++)
                    arrComputer[temp, col] = computerShipValue;
            }
        }

        /**
         * Checks the size of the ship.
         */
        private int checkSize(int computerShipValue)
        {
            int size;

            if (computerShipValue == 5) size = 5;
            else if (computerShipValue == 4) size = 6;
            else if (computerShipValue == 3) size = 7;
            else if (computerShipValue == 2) size = 7;
            else size = 8;

            return size;
        }

        /**
         * The game will reset but still in the current game play.
         */
        private void menuResetGame(object sender, RoutedEventArgs e)
        {
            BitmapImage tiles = new BitmapImage(new Uri("pack://application:,,,/Resources/seaTile.png", UriKind.RelativeOrAbsolute));

            elementVisible();
            elementHidden();
            setGame();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    var image = new Image { Source = new CroppedBitmap(tiles, new Int32Rect(0, 0, 30, 30)) };
                    image.MouseDown += gridPlacement_MouseDown;
                    gridPlayer.Children.Add(image);
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);

                    image = new Image { Source = new CroppedBitmap(tiles, new Int32Rect(0, 0, 30, 30)) };
                    image.MouseDown += Image_MouseDown;
                    gridComputer.Children.Add(image);
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);
                }

            grpPlayer.IsEnabled = true;
            rbHorizontal.IsChecked = true;
        }

        /**
         * Display all necessary elements when resets the game.
         */
        private void elementVisible()
        {
            aircraftCarrier.Visibility = Visibility.Visible;
            battleship.Visibility = Visibility.Visible;
            submarine.Visibility = Visibility.Visible;
            cruiser.Visibility = Visibility.Visible;
            destroyer.Visibility = Visibility.Visible;

            rbAircraft.Visibility = Visibility.Visible;
            rbBattleship.Visibility = Visibility.Visible;
            rbSubmarine.Visibility = Visibility.Visible;
            rbCruiser.Visibility = Visibility.Visible;
            rbDestroyer.Visibility = Visibility.Visible;

            grpShips.Visibility = Visibility.Visible;
            grpShipOrientation.Visibility = Visibility.Visible;

            tBlockTip.Visibility = Visibility.Visible;
        }

        /**
         * Hide all necessary elements called by resets the game.
         */
        private void elementHidden()
        {
            btnStartGame.Visibility = Visibility.Hidden;
            grpBoxMode.Visibility = Visibility.Hidden;
            grpComputer.Visibility = Visibility.Hidden;
            wpComputer.Visibility = Visibility.Hidden;
            imgWarning.Visibility = Visibility.Hidden;
            lblPlayerMessage.Visibility = Visibility.Hidden;

            lblComputerMessage.Content = "";
            grpComputer.IsEnabled = true;
        }

        /**
         * This will exit the current gameplay and go back to main menu.
         */
        private void menuNewGame(object sender, RoutedEventArgs e)
        {
            menuResetGame(sender, e);

            txtName.Text = "Please Enter Your Name Here";
            txtName.Visibility = Visibility.Visible;
            spStartMenu.Visibility = Visibility.Visible;
            btnEnterValidateName.Visibility = Visibility.Visible;
            menuNew.IsEnabled = false;
            menuReset.IsEnabled = false;

            rbHorizontal.IsChecked = true;
            rbSimpleMode.IsChecked = true;
            grpPlayer.IsEnabled = true;
            main.PlayLooping();
        }

        /**
         * It will display a messagebox to ask the user if it wants to play again or exit the game.
         */
        private void displayEndGameScreen()
        {
            MessageBoxResult result;

            if (playerTurnCtr % 2 == 0)
            {
                playerList[currentPlayer].incrementWins();
                result = MessageBox.Show("Congratulations! You've won! Play again?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            else
            {
                playerList[currentPlayer].incrementLoses();
                result = MessageBox.Show("Game Over. All of your ships have been sunk.  Play again?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }


            if (result == MessageBoxResult.Yes)
            {
                menuNewGame(new object(), new RoutedEventArgs());
                txtName.Text = playerList[currentPlayer].getName();
                btnEnterValidateName_Click(new object(), new RoutedEventArgs());
            }
            else
                menuExitGame(new object(), new RoutedEventArgs());
        }

        /**
         * It will check if the user exists, if not add the new player then save to the txt. 
         * Then the game will start.
         */
        private void btnEnterValidateName_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Trim() == "")
                MessageBox.Show("Enter A Valid Name");
            else
            {
                string name = txtName.Text.ToUpper();
                currentPlayer = searchForPlayer(name);

                if (currentPlayer == -1)
                {
                    Player newPlayer = new Player(name, "0", "0");
                    playerList.Add(newPlayer);
                    currentPlayer = searchForPlayer(newPlayer.getName());
                }

                lblName.Content = playerList[currentPlayer].getName();
                lblWins.Content = playerList[currentPlayer].getWins();
                lblLosses.Content = playerList[currentPlayer].getLoses();
                
                savePlayerListToFile();

                menuNew.IsEnabled = true;
                menuReset.IsEnabled = true;

                lblName.Content = txtName.Text.ToUpper();
                txtName.Visibility = Visibility.Hidden;
                spStartMenu.Visibility = Visibility.Hidden;
                btnEnterValidateName.Visibility = Visibility.Hidden;
            }
        }

        /**
         * Method that will show new window on what are the legends on the gameplay.
         */
        private void pictureLegend(object sender, RoutedEventArgs e)
        {
            pictureLegend pLegend = new pictureLegend();
            pLegend.Owner = this;
            pLegend.ShowDialog();
        }

        /**
         * Method that will show new window on how to play battleship.
         */
        private void howToPlay(object sender, RoutedEventArgs e)
        {
            howToPlay how = new howToPlay();
            how.Owner = this;
            how.ShowDialog();
        }

        /**
         * Will enable the radio button of ship if the image is clicked.
         * It will also allow user to right click the image on grid and will change image orientation.
         */
        private void ship_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image shipClicked = sender as Image;

            if(shipClicked.Name == "aircraftCarrier")
                rbAircraft.IsChecked = true;
            else if(shipClicked.Name == "battleship")
                rbBattleship.IsChecked = true;
            else if (shipClicked.Name == "submarine")
                rbSubmarine.IsChecked = true;
            else if (shipClicked.Name == "cruiser")
                rbCruiser.IsChecked = true;
            else if (shipClicked.Name == "destroyer")
                rbDestroyer.IsChecked = true;

            if(e.ChangedButton == MouseButton.Right)
            {
                if (rbHorizontal.IsChecked == true)
                    rbVertical.IsChecked = true;
                else if (rbVertical.IsChecked == true)
                    rbHorizontal.IsChecked = true;
            }
        }
        
        /**
         * The game will close.
         */
        private void menuExitGame(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}