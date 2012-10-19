using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Game.Mechanics
{


    public static class TeamReleations
    {
        public static bool IsHostile(Map.Team a, Map.Team b)
        {
            switch (a)
            { 
                case Map.Team.Zombie:
                    switch (b)
                    {
                        case Map.Team.Player: return true;
                    }
                    break;
                case Map.Team.Player:
                    switch (b)
                    {
                        case Map.Team.Zombie: return true;
                    }
                    break;
            }
            return false;
        }
    }
}
