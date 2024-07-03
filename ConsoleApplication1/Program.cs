using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
//using  openai;
using OpenAI;
using OpenAI.Chat;


namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            // Load environment variables

            string apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var openAiClient = new OpenAIClient(apiKey);
            // Get the chat client for gpt-3.5-turbo
            var chatClient = openAiClient.GetChatClient("gpt-3.5-turbo");

            ResultCollection<StreamingChatCompletionUpdate> updates
                = chatClient.CompleteChatStreaming("talk to me please!'");
            
            Console.WriteLine($"[ASSISTANT]:");
            foreach (StreamingChatCompletionUpdate update in updates)
            {
                foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                {
                    Console.Write(updatePart);
                }
            }
            
            
            var pop = new OverallPopulation();
            pop.InitRandomAnimalPopulation();
            pop.PrintAnimalPopulation();

            // Wait for 3 seconds
            System.Threading.Thread.Sleep(3000);
            
            // Make all animals fight
            while (pop.AnimalPopulation.Count > 1)
            {
                System.Threading.Thread.Sleep(2000);
                if (Tools.RollChance(50))
                {
                    pop.Fight(verbose: true);
                }
                else
                {
                    Console.WriteLine("No fighting today.");
                }
         
            }

            // Print remaining animals
            pop.PrintAnimalPopulation();
        }
    }

    public class OverallPopulation
    {
        public LinkedList<Animal> AnimalPopulation = new LinkedList<Animal>();
        private Random random = new Random(); // Single Random instance for consistent results

        public void AddAnimal(Animal animal)
        {
            AnimalPopulation.AddLast(animal);
        }

        public void RemoveAnimal(Animal animal)
        {
            AnimalPopulation.Remove(animal);
        }

        public void PrintAnimalPopulation()
        {
            foreach (var animal in AnimalPopulation)
            {
                Console.WriteLine($"{animal.Name} is a {animal.Species} and is {animal.Age} years old.");
            }
        }

        public void InitRandomAnimalPopulation()
        {
            for (int i = 0; i < 10; i++)
            {
                var cat = new Cat(Tools.GenerateName(random), Tools.GenerateInt(random, 1, 100), Tools.GenerateInt(random, 1, 10), Tools.GenerateInt(random, 1, 3));
                AddAnimal(cat);
                var dog = new Dog(Tools.GenerateName(random), Tools.GenerateInt(random, 1, 100), Tools.GenerateInt(random, 1, 10), Tools.GenerateInt(random, 1, 3));
                AddAnimal(dog);
            }
        }

        public void Fight( bool verbose = false)
        {
            if (AnimalPopulation.Count < 2) return;

            // Select two random animals
            var animal1 = AnimalPopulation.ElementAt(random.Next(AnimalPopulation.Count));
            var animal2 = AnimalPopulation.ElementAt(random.Next(AnimalPopulation.Count));

            // Ensure two different animals are selected
            while (animal1 == animal2)
            {
                animal2 = AnimalPopulation.ElementAt(random.Next(AnimalPopulation.Count));
            }

            SingleDuel(animal1, animal2, verbose);
        }

        private void SingleDuel(Animal animal1, Animal animal2, bool verbose)
        {
            Console.WriteLine($"A fight starts between {animal1.Name} and {animal2.Name}!");
            if (verbose)
            {
                // Print animal stats and differences and  who is the favourite to win
                if (animal1.HitPoints > animal2.HitPoints)
                {
                    // animal1 is the favourite
                    Console.WriteLine($"{animal1.Name} has {animal1.HitPoints} HP and is the favourite.");
                    Console.WriteLine($"{animal1.Name} has {animal1.AttackDamage} attack damage.");
                    Console.WriteLine($"{animal2.Name} has {animal2.HitPoints} HP and is the opponent.");
                    Console.WriteLine($"{animal2.Name} has {animal2.AttackDamage} attack damage.");
                }
                
            }
            while (animal1.HitPoints > 0 && animal2.HitPoints > 0)
            {
                animal1.TakeDamage(animal2.AttackDamage);
                if (animal1.HitPoints > 0)
                {
                    animal2.TakeDamage(animal1.AttackDamage);
                }
                Console.WriteLine($"{animal1.Name} attacks {animal2.Name} for {animal1.AttackDamage} damage.");
                Console.WriteLine($"{animal2.Name} attacks {animal1.Name} for {animal2.AttackDamage} damage.");
                Console.WriteLine($"{animal1.Name} has {animal1.HitPoints} HP left.");
                Console.WriteLine($"{animal2.Name} has {animal2.HitPoints} HP left.");
            }

            if (animal1.HitPoints <= 0 && animal2.HitPoints <= 0)
            {
                Console.WriteLine($"Both {animal1.Name} and {animal2.Name} have died in battle.");
                RemoveAnimal(animal1);
                RemoveAnimal(animal2);
            }
            else if (animal1.HitPoints <= 0)
            {
                Console.WriteLine($"{animal1.Name} was killed in battle. {animal2.Name} is the winner!");
                RemoveAnimal(animal1);
            }
            else if (animal2.HitPoints <= 0)
            {
                Console.WriteLine($"{animal2.Name} was killed in battle. {animal1.Name} is the winner!");
                RemoveAnimal(animal2);
            }
        }
    }

    }

    public abstract class Animal
    {
        public abstract string Name { get; }
        public abstract int Age { get; }
        public abstract string Species { get; }
        public abstract int HitPoints { get; set; }
        public abstract int AttackDamage { get; }
        
        public void TakeDamage(int damage)
        {
            HitPoints -= damage;
        }
        
    }

    public class Dog : Animal
    {
        public override string Name { get; }
        public override int Age { get; }
        public override string Species { get; }
        public override int HitPoints { get; set; }
        public override int AttackDamage { get; }

        public Dog(string name, int age, int hp, int damage)
        {
            Name = name;
            Age = age;
            Species = "Dog";
            HitPoints = hp;
            AttackDamage = damage;
        }
    }

    public class Cat : Animal
    {
        public override string Name { get; }
        public override int Age { get; }
        public override string Species { get; }
        public override int HitPoints { get; set; }
        public override int AttackDamage { get; }
        

        public Cat(string name, int age, int hp, int damage)
        {
            Name = name;
            Age = age;
            Species = "Cat";
            HitPoints = hp;
            AttackDamage = damage;
        }
    }

    public static class Tools
    {
        private static readonly Random Random = new Random(); // Ensure Random instance is shared
        public static string GenerateName(Random random)
        {
            var names = new string[]
        {
            "John", "Jane", "Bob", "Alice", "Mike", "Sarah", "Tom", "Lucy", "Jack", "Emily",
            "Liam", "Emma", "Noah", "Olivia", "Aiden", "Ava", "Caden", "Isabella", "Grayson", "Sophia",
            "Mason", "Mia", "Lucas", "Charlotte", "Logan", "Amelia", "Ethan", "Harper", "James", "Evelyn",
            "Alexander", "Abigail", "Benjamin", "Avery", "Elijah", "Ella", "William", "Scarlett", "Michael", "Grace",
            "Daniel", "Chloe", "Henry", "Sofia", "Jackson", "Lily", "Sebastian", "Aria", "David", "Layla",
            "Joseph", "Mila", "Samuel", "Nora", "Matthew", "Luna", "Gabriel", "Ellie", "Anthony", "Zoe",
            "Andrew", "Leah", "Joshua", "Hazel", "Christopher", "Violet", "Jack", "Aurora", "Oliver", "Savannah",
            "Ryan", "Riley", "Nathan", "Brooklyn", "Isaac", "Paisley", "Caleb", "Bella", "Connor", "Claire",
            "Landon", "Skylar", "Julian", "Penelope", "Christian", "Aubrey", "Hunter", "Sadie", "Cameron", "Ariana",
            "Aaron", "Allison", "Eli", "Alyssa", "Isaiah", "Anna", "Thomas", "Samantha", "Charles", "Autumn",
            "Adam", "Hailey", "Dominic", "Nevaeh", "Adrian", "Kennedy", "Lucas", "Elena", "Miles", "Eva",
            "Nolan", "Naomi", "Nathaniel", "Quinn", "Parker", "Caroline", "Zachary", "Stella", "Jaxon", "Maya",
            "Mateo", "Genesis", "Leo", "Kinsley", "Jose", "Madelyn", "Theodore", "Piper", "Xavier", "Ruby",
            "Lincoln", "Serenity", "Jace", "Willow", "Asher", "Everly", "Carter", "Lydia", "Wyatt", "Delilah",
            "Cooper", "Lillian", "Easton", "Ayla", "Colton", "Eliana", "Jordan", "Raelynn", "Evan", "Sophie",
            "Jeremiah", "Kaylee", "Angel", "Carson", "Robert", "Brielle", "Austin", "Madeline", "Brody", "Peyton",
            "Nathaniel", "Brianna", "Declan", "Mackenzie", "Brayden", "Hadley", "Bentley", "Eloise", "Elliot", "Ivy",
            "Josiah", "Lilliana", "Roman", "Adeline", "Everett", "Emery", "Kayden", "Arianna", "Greyson", "Isla",
            "Micah", "Lyla", "Jason", "Ariel", "Silas", "Emerson", "Ian", "Everleigh", "Wesley", "Vivian",
            "Miles", "Valentina", "Sawyer", "Reagan", "Amir", "Clara", "Kingston", "Delaney", "Ayden", "Jordyn",
            "Vincent", "Laila", "Giovanni", "Sara", "Diego", "Rylee", "Harrison", "Mya", "Ashton", "Melanie",
            "Ivan", "Gabriella", "Axel", "Eliza", "Max", "Arya", "Jonah", "Jasmine", "Cayden", "Taylor",
            "Hugo", "Elsie", "Kai", "June", "Emmett", "Charlie", "Jasper", "Tessa", "Jesus", "Marley",
            "Oscar", "Amara", "Maxwell", "Daisy", "Zayden", "Amina", "Ryder", "Harmony", "Carlos", "Adriana",
            "Maddox", "Lia", "Malachi", "Adalyn", "Leonardo", "Eden", "Damian", "Alina", "Xander", "Diana",
            "Santiago", "Camille", "Ezekiel", "Fiona", "Jace", "Lena", "Luis", "Nina", "Ryker", "Arabella",
            "Rafael", "Amaya", "King", "Catalina", "Zane", "Rosalie", "Milo", "Evangeline", "Jude", "Evelynn",
            "Amari", "Freya", "Beau", "Juliette", "Ronan", "Daleyza", "Tucker", "Teagan", "Brandon", "Alexandra",
            "Colt", "Ximena", "Reid", "Harlow", "Beckett", "Ashley", "Carson", "Andrea", "Waylon", "Bailey",
            "Rhett", "Brooklynn", "Grant", "Katherine", "Wesley", "Sloane", "Emilio", "Winter", "Finn", "Sabrina",
            "Paxton", "Maggie", "Hayes", "Myla", "Kaleb", "Emelia", "Simon", "Alexis", "Phoenix", "Alivia",
            "Enzo", "Nadia", "Matias", "Mabel", "Graham", "Josephine", "Jesse", "Margot", "Barrett", "Athena",
            "Luka", "Phoebe", "Remy", "Gemma", "Aidan", "Julianna", "Karter", "Blake", "Rocco", "Miriam",
            "Derek", "Sienna", "Holden", "Allie", "Emerson", "Alana", "Adonis", "Kimberly", "Kyrie", "Talia",
            "Zachariah", "Maeve", "Rafael", "Morgan", "Jett", "Brooklyn", "Hendrix", "Carmen", "Knox", "Vivienne",
            "Malik", "Gracie", "Crew", "Ember", "Sage", "Jade", "Kade", "Cecilia", "Finley", "Celeste",
            "Nico", "Esme", "Riley", "Savanna", "Brooks", "Raegan", "Cash", "Emery", "Leon", "Emersyn",
            "Rocco", "Lucia", "Cassius", "Anastasia", "Cody", "Mariah", "Nash", "Felicity", "Sonny", "Giselle",
            "Porter", "Scarlet", "Tate", "Alessandra", "Maverick", "Adeline", "Kyler", "Lexi", "Griffin", "Maliyah",
            "Anderson", "Aylin", "Apollo", "Amira", "Rory", "Annie", "Alaric", "Brynn", "Cyrus", "Anaya",
            "Callan", "Arielle", "Orion", "Ainsley", "Zephyr", "Addilyn", "Knox", "Anya", "Ridge", "Chelsea",
            "Elias", "Harlee", "Lawson", "Juliet", "Marshall", "Liana", "Zeke", "Paloma", "Zayn", "Remi"
        };

            return names[random.Next(names.Length)];
        }

        public static int GenerateInt(Random random, int lower, int upper)
        {
            return random.Next(lower, upper);
        }

        /// <summary>
        /// Determines whether an event with a given chance occurs.
        /// </summary>
        /// <param name="chance">The chance (out of 100) that the event occurs.</param>
        /// <returns>True if the event occurs, otherwise false.</returns>
        public static bool RollChance(float chance)
        {
            if (chance < 0 || chance > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(chance), "Chance must be between 0 and 100.");
            }
            return Random.NextDouble() < chance / 100.0;
        }
        
     
    }
