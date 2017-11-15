using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;
using RLNET;

namespace RogueApp.Core
{
    //Our custom DungeonMap class extends the bass RogueSharp map class
    public class DungeonMap : Map
    {
        public List<Rectangle> Rooms;

        private readonly List<Monster> _monsters;

        public DungeonMap()
        {
            Rooms = new List<Rectangle>();
            _monsters = new List<Monster>();

        }

        //This method will be called anytime we move the player to update the FOV
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            //compute the field of view
            ComputeFov(player.X, player.Y, player.Awareness, true);
            //mark all cells within the field of view
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        

        //The Draw Method will be called each time the map is updated
        //It will render all of the symbols/colors for each cell to the map sub console
        public void Draw( RLConsole mapConsole, RLConsole statConsole)
        {
            mapConsole.Clear();
            foreach(Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
            //Keep an index so we know what position to draw monster stats at
            int i = 0;
            //Iterate through each monster on the map and draw it after drawing the cells
            foreach(Monster monster in _monsters)
            {
                //When the monster is in the FOV also draw their stats
                if(IsInFov(monster.X, monster.Y))
                {
                    monster.Draw(mapConsole, this);

                    //Pass in the index to DrawStats and increment it afterwords
                    monster.DrawStats(statConsole, i);
                    i++;
                }
                
            }
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            //When we haven't explored a cell yet, we don't want to draw anything
            if (!cell.IsExplored)
            {
                return;
            }

            //When a cell is currently in the field-of-view it should be drawn with lighter colors
            if(IsInFov(cell.X, cell.Y))
            {
                //Choose the symbol to draw based on if the cell is walkable or not
                //'.' for floor and # for walls
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            //When a cell is outside of the field of view draw it with darker colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }// end method

        //Returns true when able to place Actor on the cell or false otherwise
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            //Only allow actore placemeent if cell is walkable/
            if(GetCell(x, y).IsWalkable)
            {
                SetIsWalkable(actor.X, actor.Y, true);
                //Update actors position
                actor.X = x;
                actor.Y = y;
                //The new cell the actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false);
                //Don't forget to update the FOV if we just repositioned the player
                if(actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        //A helper method for setting the IsWalkable property on a cell
        public void SetIsWalkable(int x, int y, bool isWalkAble)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkAble, cell.IsExplored);
        }        
        //Called by MapGenerator after we generate a new map to add the player to the map
        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game.SchedulingSystem.Add(player);
        }
        //Make a Monster
        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            //can't walk on monsters bro!
            SetIsWalkable(monster.X, monster.Y, false);
            Game.SchedulingSystem.Add(monster);
        }
        public void RemoveMonster(Monster monster)
        {
            _monsters.Remove(monster);
            //After removing the monster make sure the cell is walkable again
            SetIsWalkable(monster.X, monster.Y, true);
            Game.SchedulingSystem.Remove(monster);
        }
        public Monster GetMonstersAt(int x, int y)
        {
            //FirstOrDefault method looks to return a value and if none to use default
            //so pass in the x and y coords and return monster if monster coords match those exact coords
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }
        //Look for a random  walkable tile in the room
        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;

                    if (IsWalkable(x,y))
                    {
                        return new Point(x, y);
                    }
                }
            }
            //If we don't find a walkable location
            return null;
        }
        //Iterate through each cell in the room and return true if walkable
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x < room.Width - 2; x++)
            {
                for (int y = 2; y < room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;        

        }

    }

}
