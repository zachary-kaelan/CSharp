using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib
{
    class Program
    {
        static void Main(string[] args)
        {
            Internal.Initialize();
            var searchResults = Internal.Search("Maddie");
            foreach(var result in searchResults)
            {
                var user = Internal.GetUser(result.user.username);
                Console.WriteLine(user.username + " - " + user.edge_owner_to_timeline_media.count);
                /*if (user.edge_owner_to_timeline_media.count >= 63 && user.edge_owner_to_timeline_media.count <= 71)
                    Console.WriteLine(user.username + " - " + user.edge_owner_to_timeline_media.count);*/
            }
            //var user = Internal.GetUser(searchResults[0].user.username);
            //Console.WriteLine(user.edge_owner_to_timeline_media.count);
            Console.ReadLine();
        }
    }
}
