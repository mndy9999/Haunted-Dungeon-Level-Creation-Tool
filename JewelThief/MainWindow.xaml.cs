/********************************************************************************************************************************************************************************/
//                                                           16025091 - Tool Development Coursework 2                                                                           //
//                                                                                                                                                                              //
// I have added a row of buttons on the left side of the game screen so the user can select which element will be changed. (floor/tile, enemies, fire, player position, etc).   //
// On the right side I added 2 buttons that change the difficulty of the level, by changing the enemies movement speed (they only work if they are set before starting the game)//
// a checkbox that can make the player invincible (ability to walk through the enemies but not the walls), 2 text boxes and a button for changing the size of the map,          //
// a text box to change the time the player has to finish the game and text boxes and buttons that load new icons for each element of the game.                                 //
//                                                                                                                                                                              //
// The game can be saved using the text box and button on the lower right corner. The game will be saved in "..\..\..\..\HauntedDungeon\*level name*" where *level name* is the //
// text in the text box.                                                                                                                                                        //
//                                                                                                                                                                              //
// I have added a new element to the game, a power up (star icon) that can be placed in the game so that when the player steps on it, the element is eliminated and             //
// the character becomes invincible. I set it up so that the actual element is deleted but the icon remains in the game because I did not find a way to render the game         //
// without reseting the enemies and player to the starting position.                                                                                                            //
//                                                                                                                                                                              //
// I also did not manage to unfreeze the enemies when selecting a different one from the enemies list. I tried adding another for loop to multiply the old enemy's movement     //
// speed by 1000 but it ended up multiplying the movement speed of the same character every time I selected a new one. I think a way to solve this problem would be to save the //
// selected element from the enemies array into a variable then multiply the movement speed by 1000 and then update the variable whenever the user selects a new enemy to freeze// 
//                                                                                                                                                                              //
/********************************************************************************************************************************************************************************/

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
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace WizardDungeon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ///////////////////////////////////////////////////////////
        // These variables are all used by the game engine to
        // run the dynamics of the game.

        DispatcherTimer countDown = null;
        DispatcherTimer timer = null;
        Stopwatch watch = new Stopwatch();
        long previous_time = 0;
        int time_ellapsed = 0;



        ///////////////////////////////////////////////////////////
        // The game engine can be used to update the game state. The 
        // game state represents the position and velocities of the
        // player and enemies. Note the enemies do not have a velocity
        // defined as part of the level.

        CGameEngine gameEngine;
        CGameState gameState;


        ///////////////////////////////////////////////////////////
        // The level represents the position of walls and floors
        // and contains starting positions of the player and enemy and goal.
        // The game textures stores all the images used to represent
        // different icons.

        CLevel currentLevel;
        CGameTextures gameTextures;


        ////////////////////////////////////////////////////////////
        // We keep a reference to these as we need to be able to 
        // update their position in the canvas. We do this by passing
        // a reference to the canvas (e.g. Canvas.SetLeft(enemyIcons[i], Position X);)

        Image[] enemyIcons;
        Image[] fireIcons;
        Image playerIcon;
        Image[] powerIcons;


        BitmapImage PlayerIcon_new = new BitmapImage();
        BitmapImage FloorIcon_new = new BitmapImage();
        BitmapImage WallIcon_new = new BitmapImage();
        BitmapImage EnemyIcon_new = new BitmapImage();
        BitmapImage FireIcon_new = new BitmapImage();
        BitmapImage GoalIcon_new = new BitmapImage();
        BitmapImage PowerIcon_new = new BitmapImage();

        /////////////////////////////////////////////////////////////
        // These represent the player and monster speed per frame 
        // (in units of tiles).

        float player_speed = 0.15f;
        float monster_speed = 0.07f;

        //string used to check which icon is selected from the left row
        string ButtonFlag;

        public MainWindow()
        {
            InitializeComponent();
        }




        //****************************************************************//
        // This function is used as an event handler for the load click  *//
        // event. This is used to load a new level. In addition to       *//
        // the data required for the level it also ensures the board is  *//
        // displayed.                                                    *//
        //****************************************************************//
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ////////////////////////////////////////////////////////////
            // clear any existing children from the canvas.

            cvsMainScreen.Children.Clear();

            ///////////////////////////////////////////////////////////
            // Get the directory where the level data is stored and 
            // load the data in. 

            string fileDir = txtLevelDir.Text;
            currentLevel = CLevelParser.ImportLevel(fileDir);
            gameTextures = CLevelParser.ImportTextures(fileDir);


            ///////////////////////////////////////////////////////////
            // Draw the set of wall and floor tiles for the current
            // level and the goal icon. This is part of the game
            // we do not expect to change as it cannot move.

            DrawLevel();


            //////////////////////////////////////////////////////////
            // Add a game state, this represents the position and velocity
            // of all the enemies and the player. Basically, anything
            // that is dynamic that we expect to move around.

            gameState = new CGameState(currentLevel.EnemyPositions.Count());


            ///////////////////////////////////////////////////////////
            // Set up the player to have the correct .bmp and set it to 
            // its initial starting point. The player's position is stored
            // as a tile index on the Clevel class, this must be converted 
            // to a pixel position on the game state.

            playerIcon = new Image();
            playerIcon.Width = CGameTextures.TILE_SIZE;
            playerIcon.Height = CGameTextures.TILE_SIZE;

            playerIcon.Source = gameTextures.PlayerIcon;

            cvsMainScreen.Children.Add(playerIcon);


            //////////////////////////////////////////////////////////
            // Create instances of the enemies and fires for display. We must do
            // this as each child on a canvas must be a distinct object,
            // we could not simply add the same image multiple times.

            enemyIcons = new Image[currentLevel.EnemyPositions.Count()];

            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                enemyIcons[i] = new Image();

                enemyIcons[i].Width = CGameTextures.TILE_SIZE;
                enemyIcons[i].Height = CGameTextures.TILE_SIZE;

                enemyIcons[i].Source = gameTextures.EnemyIcon;

                cvsMainScreen.Children.Add(enemyIcons[i]);
            }


            fireIcons = new Image[currentLevel.FirePositions.Count()];

            for (int i = 0; i < currentLevel.FirePositions.Count(); i++)
            {
                fireIcons[i] = new Image();

                fireIcons[i].Width = CGameTextures.TILE_SIZE;
                fireIcons[i].Height = CGameTextures.TILE_SIZE;

                fireIcons[i].Source = gameTextures.FireIcon;

                cvsMainScreen.Children.Add(fireIcons[i]);

                CPoint2i tilePosition = CLevelUtils.GetPixelFromTileCoordinates(new CPoint2i(currentLevel.FirePositions[i].X, currentLevel.FirePositions[i].Y));


                Canvas.SetLeft(fireIcons[i], tilePosition.X);
                Canvas.SetTop(fireIcons[i], tilePosition.Y);
            }

            powerIcons = new Image[currentLevel.PowerPositions.Count()];

            for (int i = 0; i < currentLevel.PowerPositions.Count(); i++)
            {
                powerIcons[i] = new Image();

                powerIcons[i].Width = CGameTextures.TILE_SIZE;
                powerIcons[i].Height = CGameTextures.TILE_SIZE;

                powerIcons[i].Source = gameTextures.PowerIcon;

                CPoint2i tilePosition = CLevelUtils.GetPixelFromTileCoordinates(new CPoint2i(currentLevel.PowerPositions[i].X, currentLevel.PowerPositions[i].Y));

                Canvas.SetLeft(powerIcons[i], tilePosition.X);
                Canvas.SetTop(powerIcons[i], tilePosition.Y);

                cvsMainScreen.Children.Add(powerIcons[i]);
            }



            FireImage.Source = gameTextures.FireIcon;
            EnemyImage.Source = gameTextures.EnemyIcon;
            WallImage.Source = gameTextures.FloorTexture;
            PlayerImage.Source = gameTextures.PlayerIcon;
            GoalImage.Source = gameTextures.GoalIcon;
            PowerImage.Source = gameTextures.PowerIcon;

            ////////////////////////////////////////////////////////////
            // Set each instance of a dynamic object to its initial position
            // as defined by the current level object.

            InitialiseGameState();


            ////////////////////////////////////////////////////////////
            // Render the current game state, this will render the player
            // and the enemies in their initial position.

            RenderGameState();

            EnemyList.Items.Clear();
            //add enemies to a list
            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                EnemyList.Items.Add("Enemy " + i);
            }

        }


        //****************************************************************//
        // This initilaises the dynamic parts of the game to their initial//
        // positions as specified by the game level.                      //
        //****************************************************************//
        private void InitialiseGameState()
        {
            ////////////////////////////////////////////////////////////
            // Place the player at their initial position.

            gameState.Player.Position = CLevelUtils.GetPixelFromTileCoordinates(currentLevel.StartPosition);
            Random random = new Random();

            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                ////////////////////////////////////////////////////////////
                // Place each enemy at their initial position and give them an
                // initial random direction.

                gameState.Enemies[i].Position = CLevelUtils.GetPixelFromTileCoordinates(currentLevel.EnemyPositions[i]);

                gameState.Enemies[i].TargetPosition.X = gameState.Enemies[i].Position.X;
                gameState.Enemies[i].TargetPosition.Y = gameState.Enemies[i].Position.Y;


                /////////////////////////////////////////////////////////////
                // Create a random direction to walk in.

                int direction = random.Next() % 4;

                switch (direction)
                {
                    case 0:
                        gameState.Enemies[i].Velocity.X = monster_speed;
                        gameState.Enemies[i].Velocity.Y = 0.0f;
                        break;

                    case 1:
                        gameState.Enemies[i].Velocity.X = -monster_speed;
                        gameState.Enemies[i].Velocity.Y = 0.0f;
                        break;

                    case 2:
                        gameState.Enemies[i].Velocity.X = 0.0f;
                        gameState.Enemies[i].Velocity.Y = monster_speed;
                        break;

                    case 3:
                    default:
                        gameState.Enemies[i].Velocity.X = 0.0f;
                        gameState.Enemies[i].Velocity.Y = -monster_speed;
                        break;
                }
            }
        }


        //****************************************************************//
        // This function renders the dynamic content of the game to the  *//
        // main canvas. This is done by updating the positions of all    *//
        // the sprite icons in the canvas using the current game state   *//
        // and then invoking the canvas to refresh.                      *//
        //****************************************************************//
        private void RenderGameState()
        {
            Canvas.SetLeft(playerIcon, gameState.Player.Position.X);
            Canvas.SetTop(playerIcon, gameState.Player.Position.Y);


            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                Canvas.SetLeft(enemyIcons[i], gameState.Enemies[i].Position.X);
                Canvas.SetTop(enemyIcons[i], gameState.Enemies[i].Position.Y);
            }
        }


        //****************************************************************//
        // This function draws the static parts of the level onto the    *//
        // canvas.                                                       *//
        //****************************************************************//
        private void DrawLevel()
        {
            /////////////////////////////////////////////////////////////
            // Compute the width of the canvas, this will be the number
            // of tiles multiplied by the tile size (in pixels).

            int width = currentLevel.Width * CGameTextures.TILE_SIZE;
            int height = currentLevel.Height * CGameTextures.TILE_SIZE;

            cvsMainScreen.Width = width;
            cvsMainScreen.Height = height;


            /////////////////////////////////////////////////////////////
            // Loop through the level setting each tiled position on the 
            // canvas.

            for (int y = 0; y < currentLevel.Height; y++)
            {
                for (int x = 0; x < currentLevel.Width; x++)
                {
                    /////////////////////////////////////////////////////////
                    // We must create a new instance of the image as an image
                    // can only be added once to a given canvas.

                    Image texture = new Image();
                    texture.Width = CGameTextures.TILE_SIZE;
                    texture.Height = CGameTextures.TILE_SIZE;


                    //////////////////////////////////////////////////////////
                    // Set the position of the tile, we must convert from tile
                    // coordinates to pixel coordinates.

                    CPoint2i tilePosition = CLevelUtils.GetPixelFromTileCoordinates(new CPoint2i(x, y));


                    Canvas.SetLeft(texture, tilePosition.X);
                    Canvas.SetTop(texture, tilePosition.Y);


                    //////////////////////////////////////////////////////////
                    // Check whether it should be a wall tile or floor tile.

                    if (currentLevel.GetTileType(x, y) == eTileType.Wall)
                    {
                        texture.Source = gameTextures.WallTexture;
                    }
                    else
                    {
                        texture.Source = gameTextures.FloorTexture;
                    }

                    cvsMainScreen.Children.Add(texture);
                }
            }


            ////////////////////////////////////////////////////////////
            // The goal is also static as it does not move so we will
            // also add this now also.

            Image goalImg = new Image();
            goalImg.Width = CGameTextures.TILE_SIZE;
            goalImg.Height = CGameTextures.TILE_SIZE;

            goalImg.Source = gameTextures.GoalIcon;

            CPoint2i GoalPosition = CLevelUtils.GetPixelFromTileCoordinates(new CPoint2i(currentLevel.GoalPosition.X, currentLevel.GoalPosition.Y));

            Canvas.SetLeft(goalImg, GoalPosition.X);
            Canvas.SetTop(goalImg, GoalPosition.Y);

            cvsMainScreen.Children.Add(goalImg);



        }


        //****************************************************************//
        // This event handler is used to handle a key being pressed.     *//
        // It only takes action if a direction key is pressed.           *//
        //****************************************************************//
        private void tb_KeyDown(object sender, KeyEventArgs args)
        {
            //////////////////////////////////////////////////////////
            // We set the players velocity, this controls which direction
            // it will move in.

            if (Keyboard.IsKeyDown(Key.Up))
            {
                args.Handled = true;
                gameState.Player.Velocity.Y = -player_speed;
                gameState.Player.Velocity.X = 0.0f;
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {
                args.Handled = true;
                gameState.Player.Velocity.Y = player_speed;
                gameState.Player.Velocity.X = 0.0f;
            }
            else if (Keyboard.IsKeyDown(Key.Left))
            {
                args.Handled = true;
                gameState.Player.Velocity.Y = 0.0f;
                gameState.Player.Velocity.X = -player_speed;
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {
                args.Handled = true;
                gameState.Player.Velocity.Y = 0.0f;
                gameState.Player.Velocity.X = player_speed;
            }
        }

        //****************************************************************//
        // This event handler is used to handle a key being released.    *//
        // We presume the direction key is being released.               *//
        //****************************************************************//
        private void tb_KeyUp(object sender, KeyEventArgs args)
        {
            ///////////////////////////////////////////////////////////////
            // By setting a player's velocity to zero it will not move.
            if (args.Key == Key.Up || args.Key == Key.Down || args.Key == Key.Left || args.Key == Key.Right)
            {
                gameState.Player.Velocity.X = 0.0f;
                gameState.Player.Velocity.Y = 0.0f;
            }

        }


        //****************************************************************//
        // This event handler is used to start the game.                 *//
        //****************************************************************//
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            lblMsg.Content = "";

            watch.Reset();
            watch.Start();
            previous_time = 0;


            //////////////////////////////////////////////////////////////
            // Create a new game engine to handle the interactions and 
            // register events to handle collisions with enemies or
            // winning the game.


            gameEngine = new CGameEngine(currentLevel);
            gameEngine.OnGoalReached += EndGame;
            gameEngine.OnPlayerCaught += EndGame;


            //////////////////////////////////////////////////////////////
            // The game is rendered by using a dispatchTimer. This will 
            // trigger the game loop (RunTime) every 40ms.

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += RunGame;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 40);

                countDown = new DispatcherTimer();
                countDown.Tick += UpdateTime;
                countDown.Interval = new TimeSpan(0, 0, 0, 1, 0);
            }
            time_ellapsed = 0;
            countDown.Start();
            timer.Start();



            /////////////////////////////////////////////////////////////
            // Make some of the elements on screen disabled so that the
            // user can't restart mid game play.

            txtLevelDir.IsEnabled = false;
            btnStart.IsEnabled = false;
            btnLoad.IsEnabled = false;
            Keyboard.Focus(cvsMainScreen);

            ////////////////////////////////////////////////////////////
            // Set each instance of a dynamic object to its initial position.

            InitialiseGameState();


            ////////////////////////////////////////////////////////////
            // Render the current game state, this will render the player
            // and the enemies in their initial position.

            RenderGameState();
        }


        private void UpdateTime(object sender, object o)
        {
            time_ellapsed++;

            if (time_ellapsed < currentLevel.Time)
                lblMsg.Content = "Time Remaining: " + (currentLevel.Time - time_ellapsed);
            else
            {
                EndGame(this, "You lose, you ran out of time!");
            }
        }

        //****************************************************************//
        // This function represents the main game loop. It computes the   //
        // time between this and the previous call and then updates the   //
        // game state. It then requests the new game state to be          //
        // rendered.                                                      //
        //****************************************************************//
        private void RunGame(object sender, object o)
        {
            //////////////////////////////////////////////////////////////
            // Compute the difference in time between two consecutive 
            // calls.

            long current_time = watch.ElapsedMilliseconds;
            long time_delta = current_time - previous_time;
            previous_time = current_time;


            //////////////////////////////////////////////////////////////
            // Update and render the game.

            gameEngine.UpdateVelocities(gameState, (float)time_delta);
            gameEngine.UpdatePositions(gameState, (float)time_delta);

            RenderGameState();
        }


        //****************************************************************//
        // This function will be registered to be triggered when the game //
        // finishes. It re-enables any buttons and displays a message to  //
        // indicate the end result of the game.                           //
        //****************************************************************//
        public void EndGame(object sender, string message)
        {
            countDown.Stop();
            lblMsg.Content = message;
            timer.Stop();
            txtLevelDir.IsEnabled = true;
            btnStart.IsEnabled = true;
            btnLoad.IsEnabled = true;
        }


        private void Render()
        {
            ////////////////////////////////////////////////////////////
            // clear any existing children from the canvas.

            cvsMainScreen.Children.Clear();

            ///////////////////////////////////////////////////////////
            // Draw the set of wall and floor tiles for the current
            // level and the goal icon. This is part of the game
            // we do not expect to change as it cannot move.

            DrawLevel();

            //////////////////////////////////////////////////////////
            // Add a game state, this represents the position and velocity
            // of all the enemies and the player. Basically, anything
            // that is dynamic that we expect to move around.

            gameState = new CGameState(currentLevel.EnemyPositions.Count());

            ///////////////////////////////////////////////////////////
            // Set up the player to have the correct .bmp and set it to 
            // its initial starting point. The player's position is stored
            // as a tile index on the Clevel class, this must be converted 
            // to a pixel position on the game state.

            playerIcon = new Image();
            playerIcon.Width = CGameTextures.TILE_SIZE;
            playerIcon.Height = CGameTextures.TILE_SIZE;

            playerIcon.Source = gameTextures.PlayerIcon;

            cvsMainScreen.Children.Add(playerIcon);


            //////////////////////////////////////////////////////////
            // Create instances of the enemies and fires for display. We must do
            // this as each child on a canvas must be a distinct object,
            // we could not simply add the same image multiple times.

            enemyIcons = new Image[currentLevel.EnemyPositions.Count()];

            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                enemyIcons[i] = new Image();

                enemyIcons[i].Width = CGameTextures.TILE_SIZE;
                enemyIcons[i].Height = CGameTextures.TILE_SIZE;

                enemyIcons[i].Source = gameTextures.EnemyIcon;

                cvsMainScreen.Children.Add(enemyIcons[i]);
            }


            fireIcons = new Image[currentLevel.FirePositions.Count()];

            for (int i = 0; i < currentLevel.FirePositions.Count(); i++)
            {
                fireIcons[i] = new Image();

                fireIcons[i].Width = CGameTextures.TILE_SIZE;
                fireIcons[i].Height = CGameTextures.TILE_SIZE;

                fireIcons[i].Source = gameTextures.FireIcon;

                cvsMainScreen.Children.Add(fireIcons[i]);

                CPoint2i tilePosition = CLevelUtils.GetPixelFromTileCoordinates(new CPoint2i(currentLevel.FirePositions[i].X, currentLevel.FirePositions[i].Y));

                Canvas.SetLeft(fireIcons[i], tilePosition.X);
                Canvas.SetTop(fireIcons[i], tilePosition.Y);
            }

            powerIcons = new Image[currentLevel.PowerPositions.Count()];

            for (int i = 0; i < currentLevel.PowerPositions.Count(); i++)
            {
                powerIcons[i] = new Image();

                powerIcons[i].Width = CGameTextures.TILE_SIZE;
                powerIcons[i].Height = CGameTextures.TILE_SIZE;

                powerIcons[i].Source = gameTextures.PowerIcon;

                CPoint2i tilePosition = CLevelUtils.GetPixelFromTileCoordinates(new CPoint2i(currentLevel.PowerPositions[i].X, currentLevel.PowerPositions[i].Y));

                Canvas.SetLeft(powerIcons[i], tilePosition.X);
                Canvas.SetTop(powerIcons[i], tilePosition.Y);

                cvsMainScreen.Children.Add(powerIcons[i]);
            }

        InitialiseGameState();
        

        EnemyList.Items.Clear();

            //add enemies to a list
            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                EnemyList.Items.Add("Enemy " + i);

            }

            ////////////////////////////////////////////////////////////
            // Render the current game state, this will render the player
            // and the enemies in their initial position.

            RenderGameState();

        }

        //radio buttons which change the speed of the enemies 
        private void RadioBtn_Checked(object sender, RoutedEventArgs e)
        {

            if (sender == Easy)
            {
                monster_speed = 0.07f;
            }
            else if (sender == Hard)
            {
                monster_speed = 0.1f;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////
        ///
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {

                if (EnemyList.SelectedIndex == i)
                {
                    gameState.Enemies[i].Velocity.X = gameState.Enemies[i].Velocity.X / 1000;
                    gameState.Enemies[i].Velocity.Y = gameState.Enemies[i].Velocity.Y / 1000;



                }


            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            gameEngine.SetInvincibility(true);
            currentLevel.PowerPositions.Clear();
            Render();
        }

        private void Invincible_Unchecked(object sender, RoutedEventArgs e)
        {
            gameEngine.SetInvincibility(false);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////
        ///This function determines which button is pressed so it selects the corresponding element.
        ///Next, it swaps the tiles between floors and walls, changes the starting position of the player
        ///and the goal position and can add or delete enemies, fires or power ups.
        private void cvsMainScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //The position of the tile clicked is saved in the tilePosition variable
            //which will later be used as the position of the elements
            Point p = e.GetPosition(cvsMainScreen);
            CPoint2i pixelPos = new CPoint2i((int)p.X, (int)p.Y);
            CPoint2i tilePosition = CLevelUtils.GetTileCoordinatesFromPixel(pixelPos);

            //change between floor and walls
            if (ButtonFlag == "FW")
            {
                CPoint2i texturePosition = CLevelUtils.GetPixelFromTileCoordinates(tilePosition);
                if (currentLevel.GetTileType(tilePosition.X, tilePosition.Y) == eTileType.Wall)
                {
                    currentLevel.SetTileType(tilePosition.X, tilePosition.Y, eTileType.Floor);
                }
                else
                {
                    currentLevel.SetTileType(tilePosition.X, tilePosition.Y, eTileType.Wall);
                }

            }

            //add or delete gosts
            if (ButtonFlag == "Ghost")
            {
                CPoint2i EnemyPos = new CPoint2i();
                for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
                {
                    CPoint2i EnemyPixelPos = new CPoint2i((int)gameState.Enemies[i].Position.X, (int)gameState.Enemies[i].Position.Y);
                    EnemyPos = CLevelUtils.GetTileCoordinatesFromPixel(EnemyPixelPos);
                    if (EnemyPos.X == tilePosition.X && EnemyPos.Y == tilePosition.Y)
                    {
                        currentLevel.EnemyPositions.RemoveAt(i);
                    }
                }
                if (EnemyPos.X != tilePosition.X || EnemyPos.Y != tilePosition.Y)
                {
                    currentLevel.EnemyPositions.Add(new CPoint2i(tilePosition.X, tilePosition.Y));
                }
            }

            //add or delete fires
            if (ButtonFlag == "Fire")
            {
                CPoint2f firePos = new CPoint2f();
                for (int i = 0; i < currentLevel.FirePositions.Count(); i++)
                {
                    firePos = new CPoint2f((int)currentLevel.FirePositions[i].X, (int)currentLevel.FirePositions[i].Y);
                    if (firePos.X == tilePosition.X && firePos.Y == tilePosition.Y)
                    {
                        currentLevel.FirePositions.RemoveAt(i);
                    }
                }
                if (firePos.X != tilePosition.X || firePos.Y != tilePosition.Y)
                {
                    currentLevel.FirePositions.Add(new CPoint2i(tilePosition.X, tilePosition.Y));
                }
            }

            //change the player's starting position
            if(ButtonFlag == "Player")
            {
                currentLevel.StartPosition = tilePosition;
            }

            //change the goal position
            if(ButtonFlag == "Goal")
            {
                currentLevel.GoalPosition = tilePosition;
            }

            //add or delete power ups
            if(ButtonFlag == "Power")
            {
                CPoint2i powerPos = new CPoint2i();
                for (int i = 0; i < currentLevel.PowerPositions.Count(); i++)
                {
                    powerPos = new CPoint2i(currentLevel.PowerPositions[i].X, currentLevel.PowerPositions[i].Y);
                    if (powerPos.X == tilePosition.X && powerPos.Y == tilePosition.Y)
                    {
                        currentLevel.PowerPositions.RemoveAt(i);
                    }
                }
                if(powerPos.X != tilePosition.X || powerPos.Y != tilePosition.Y)
                 {
                    currentLevel.PowerPositions.Add(new CPoint2i(tilePosition.X, tilePosition.Y));
                }

            }
            Render();
        }


        ////////////////////////////////////////////////////////////////////////////
        ///This function is used for loading the new images for the game's elements.
        ///To do that, it takes the string from a text box and uses it as a path
        ///to find the new textures.
        private void btn_Load_Click(object sender, RoutedEventArgs e)
        {
            if (sender == PlayerLoad_btn)
            {
                PlayerIcon_new = CLevelParser.LockFreeBmpLoad(@PlayerLoad_textBox.Text);
                gameTextures.PlayerIcon = PlayerIcon_new;
            }
            else if (sender == FloorLoad_btn)
            {
                FloorIcon_new = CLevelParser.LockFreeBmpLoad(@FloorLoad_textBox.Text);
                gameTextures.FloorTexture = FloorIcon_new;

            }
            else if (sender == WallLoad_btn)
            {
                WallIcon_new = CLevelParser.LockFreeBmpLoad(@WallLoad_textBox.Text);
                gameTextures.WallTexture = WallIcon_new;
                WallImage.Source = WallIcon_new;
            }
            else if (sender == EnemyLoad_btn)
            {
                EnemyIcon_new = CLevelParser.LockFreeBmpLoad(@EnemyLoad_textBox.Text);
                gameTextures.EnemyIcon = EnemyIcon_new;
                EnemyImage.Source = EnemyIcon_new;
            }
            else if (sender == FireLoad_btn)
            {
                FireIcon_new = CLevelParser.LockFreeBmpLoad(@FireLoad_textBox.Text);
                gameTextures.FireIcon = FireIcon_new;
                FireImage.Source = FireIcon_new;
            }
            else if(sender == GoalLoad_btn)
            {
                GoalIcon_new = CLevelParser.LockFreeBmpLoad(@GoalLoad_textBox.Text);
                gameTextures.GoalIcon = GoalIcon_new;
                GoalImage.Source = GoalIcon_new;
                
            }
            Render();
        }

        //////////////////////////////////////////////////////////////////////////
        ///This function is used for changing the selected button's appearance.
        ///It also sets the button flag which determines which button is selected.
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (sender == FireButton)
            {
                FireButton.BorderBrush = new SolidColorBrush(Colors.BlueViolet);
                FireButton.Background = new SolidColorBrush(Colors.LightBlue);

                setDeafault(GhostButton);
                setDeafault(TWButton);
                setDeafault(PlayerButton);
                setDeafault(GoalButton);
                setDeafault(PowerButton);

                ButtonFlag = "Fire";
            }
            else if (sender == GhostButton)
            {
                GhostButton.BorderBrush = new SolidColorBrush(Colors.BlueViolet);
                GhostButton.Background = new SolidColorBrush(Colors.LightBlue);

                setDeafault(FireButton);
                setDeafault(TWButton);
                setDeafault(PlayerButton);
                setDeafault(GoalButton);
                setDeafault(PowerButton);

                ButtonFlag = "Ghost";
            }
            else if (sender == TWButton)
            {
                TWButton.BorderBrush = new SolidColorBrush(Colors.BlueViolet);
                TWButton.Background = new SolidColorBrush(Colors.LightBlue);

                setDeafault(GhostButton);
                setDeafault(FireButton);
                setDeafault(PlayerButton);
                setDeafault(GoalButton);
                setDeafault(PowerButton);

                ButtonFlag = "FW";
            }
            else if(sender == PlayerButton)
            {
                PlayerButton.BorderBrush = new SolidColorBrush(Colors.BlueViolet);
                PlayerButton.Background = new SolidColorBrush(Colors.LightBlue);

                setDeafault(GhostButton);
                setDeafault(FireButton);
                setDeafault(TWButton);
                setDeafault(GoalButton);
                setDeafault(PowerButton);

                ButtonFlag = "Player";
            }
            else if (sender == GoalButton)
            {
                GoalButton.BorderBrush = new SolidColorBrush(Colors.BlueViolet);
                GoalButton.Background = new SolidColorBrush(Colors.LightBlue);

                setDeafault(GhostButton);
                setDeafault(FireButton);
                setDeafault(TWButton);
                setDeafault(PlayerButton);
                setDeafault(PowerButton);

                ButtonFlag = "Goal";
            }
            else if (sender == PowerButton)
            {
                PowerButton.BorderBrush = new SolidColorBrush(Colors.BlueViolet);
                PowerButton.Background = new SolidColorBrush(Colors.LightBlue);

                setDeafault(GhostButton);
                setDeafault(FireButton);
                setDeafault(TWButton);
                setDeafault(PlayerButton);
                setDeafault(GoalButton);



                ButtonFlag = "Power";
            }

        }

        ////////////////////////////////////////////////
        ///This function is used for setting the buttons 
        ///background to default (white)

        private void setDeafault(Button button)
        {
            button.BorderBrush = new SolidColorBrush(Colors.White);
            button.Background = new SolidColorBrush(Colors.White);
        }



        /////////////////////////////////////////////////////////////////////
        ///This function saves the level by exporting the elements positions
        ///and the textures used into a folder named using a text box
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            string levelName = levelSave_TxtBox.Text;
            string levelLocation = @"..\..\..\..\HauntedDungeon\";
            CLevelParser.ExportLevel(currentLevel, (levelLocation + levelName));
            CLevelParser.ExportTextures(gameTextures, (levelLocation + levelName));
        }



        ////////////////////////////////////////////////////////////////////////////
        ///This function takes the string from the aferent text box and turns it
        ///into an integer which is then set as the time.
        private void Time_TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentLevel.Time = int.Parse(Time_TxtBox.Text);
        }


        ///////////////////////////////////////////////////////////////////////////////
        ///This function resizes the map by taking the strings from the two text boxes
        ///and tunring them into integers. If an element is outside the map, it will be
        ///sent to position [0.0]

        private void Resize_btn_Click(object sender, RoutedEventArgs e)
        {

            int x = int.Parse(col_txt.Text);
            int y = int.Parse(row_txt.Text);
            currentLevel.Resize(x, y);

            for (int i = 0; i < currentLevel.EnemyPositions.Count(); i++)
            {
                CPoint2i enemyPos = new CPoint2i((int)gameState.Enemies[i].Position.X, (int)gameState.Enemies[i].Position.Y);
                CPoint2i enemyTilePos = CLevelUtils.GetTileCoordinatesFromPixel(enemyPos);
                if (enemyTilePos.X >= x || enemyTilePos.Y >= y)
                {
                    currentLevel.EnemyPositions[i].X = 0;
                    currentLevel.EnemyPositions[i].Y = 0;

                }
            }

            for(int i = 0; i < currentLevel.FirePositions.Count(); i++)
            {
                if (currentLevel.FirePositions[i].X >= x || currentLevel.FirePositions[i].Y >= y)
                {
                    currentLevel.FirePositions[i].X = 0;
                    currentLevel.FirePositions[i].Y = 0;
                }
            }

            if (currentLevel.StartPosition.X >= x || currentLevel.StartPosition.Y >= y)
            {
                currentLevel.StartPosition.X = 0;
                currentLevel.StartPosition.Y = 0;
            }

            if (currentLevel.GoalPosition.X >= x || currentLevel.GoalPosition.Y >= y)
            {
                currentLevel.GoalPosition.X = 0;
                currentLevel.GoalPosition.Y = 0;
            }
            
            Render();
        }


    }
    
}

