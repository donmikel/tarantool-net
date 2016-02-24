using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Util.Internal.Collections;
using Tarantool.Net.IProto;

namespace Tarantool.Net.Examples
{
    static class Program
    {
        static ActorSystem _actorSystem;

        [STAThread]
        static void Main()
        {
            _actorSystem = ActorSystem.Create("tarantool");
            var connection = _actorSystem.ActorOf(Tarantool.Create());

            connection.Ask(Request
                .Select()
                .WithSpaceId(512)
                .WithIndexId(0)
                .WithIterator(Iterator.ALL)
                .Build())
                .Result
                .Match()
                .With<Failure>(f =>
                {
                    Console.WriteLine($"Error: {f.Exception}");
                })
                .With<Response>(r =>
                {
                    foreach (var tuple in r.Body)
                    {
                        Console.WriteLine(tuple);
                    }

                });

            Console.ReadLine();
        }

    }
}
