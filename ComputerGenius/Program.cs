using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGenius
{
    [Flags]
    public enum Qualities
    {
        Savior,
        Sexy,
        Swole
    };

    [Flags]
    public enum Weapons
    {
        ForeskinsOfEnemies,
        Bread,
        Wine
    };

    class Program
    {
        static void Main(string[] args)
        {
            Deity Jesus = new Deity(
                Qualities.Sexy |
                Qualities.Swole |
                Qualities.Savior,
                Weapons.Bread |
                Weapons.Wine |
                Weapons.ForeskinsOfEnemies
            );

            while (true)
            {
                Console.WriteLine(Console.ReadLine().EndsWith(" ") ? "No." : "Yes.");
            }
        }
    }

    public class Deity
    {
        public Deity(Qualities qualities, Weapons weapons)
        {

        }
    }
}
