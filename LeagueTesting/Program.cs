using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RiotLib;

namespace LeagueTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager.UpdateItems();
            Console.ReadLine();

            var dirs = Directory.GetDirectories(@"E:\League of Legends\Riot API\Images\Runes");
            foreach(var dir in dirs)
            {
                Console.WriteLine(dir);
                var subdirs = Directory.GetDirectories(dir);
                foreach(var subdir in subdirs)
                {
                    Console.WriteLine("\t" + subdir);
                    var files = Directory.GetFiles(subdir);
                    foreach(var file in files)
                    {
                        string newName = dir + "\\" + Path.GetFileName(file);
                        Console.WriteLine("\t\t" + file + " > " + newName);
                        File.Move(file, newName);
                    }
                    Directory.Delete(subdir);
                }
            }
            Console.ReadLine();

            double health1 = 880;
            double health2 = 880;
            double damage1 = 121;
            double damage2 = 151 * 1.3;
            double attackSpeed1 = 1/(0.625 * 1.309);
            double attackSpeed2 = 1/(0.625 * 1.059);
            double armor = 100 / (100 + 44.8);
            double healthRegen = 1.74 / 2;

            double regenTimer = 0.5;
            double attackTimer1 = attackSpeed1;
            double attackTimer2 = attackSpeed2;
            while (health1 > 0 && health2 > 0)
            {
                attackTimer1 -= 0.05;
                attackTimer2 -= 0.05;
                regenTimer -= 0.05;
                if (regenTimer <= 0)
                {
                    health1 += healthRegen;
                    health2 += healthRegen;
                    Console.WriteLine("Health1: {0}\t\tHealth2: {1}", health1, health2);
                    regenTimer += 0.5;
                }
                if (attackTimer1 <= 0)
                {
                    double damage = damage1 + Math.Max(0.08 * health2, 15);
                    damage *= armor;
                    health2 -= damage;
                    health1 += 0.15 * damage;
                    attackTimer1 += attackSpeed1;
                }
                if (attackTimer2 <= 0)
                {
                    double damage = damage2 * armor;
                    health1 -= damage;
                    health2 += 0.03 * damage;
                    attackTimer2 += attackSpeed2;
                }
            }
            Console.WriteLine("Health1: {0}\t\tHealth2: {1}", health1, health2);
            Console.ReadLine();
        }
    }
}
