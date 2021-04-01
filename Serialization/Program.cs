using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Serialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person()
            {
                FirstName = "FN",
                LastName = "LN",
                Car = new Car()
                {
                    Speed = 10,
                    MaxSpeed = 100
                }
            };

            try
            {
                var mySerialized = Serializer.Serialize(person);
                Console.WriteLine(mySerialized);

                var deserializedMine = Serializer.Deserialize1<Person>(mySerialized);
                Console.WriteLine($"{deserializedMine.FirstName}, {deserializedMine.LastName}, {deserializedMine.Car.Speed},{deserializedMine.Car.MaxSpeed} ");
            }
            catch (StatusCodeException e)
            {
                Console.WriteLine(e.StatusCode);
                Console.WriteLine(e.Message);
                Environment.Exit((int)e.StatusCode);
            }
        }
    }

    public class Car
    {
        public int Speed { get; set; }

        public int MaxSpeed { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Car Car { get; set; }
    }
}