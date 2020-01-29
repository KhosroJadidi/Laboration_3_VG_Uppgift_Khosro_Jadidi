using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace Laboration_3_VG_Uppgift_Khosro_Jadidi
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient lab3Client = new MongoClient("mongodb://localhost:27017");              //Connect to mongodb on localhost.
            var lab3Database= lab3Client.GetDatabase("lab3Database");                           //GetDatabse creates a new database called "lab3Database".
            var lab3Collection = lab3Database.GetCollection<BsonDocument>("restaurants");       //GetCollection creates a new collection called "restaurants", or if it already exists, gets is.

            List<BsonDocument> restaurantsList = new List<BsonDocument>                         //Creating a List of restaurants to be inserted into db.                           
            {
                new Restaurants                                                                 //See "Restaurants" class
                    (
                        new ObjectId("5c39f9b5df831369c19b6bca"),
                        "Sun Bakery Trattoria",
                        4,
                        new List<string>{ "Pizza", "Pasta", "Italian", "Coffee", "Sandwiches"}
                    ).ToBsonDocument(),                                                         //Each restaurant object in converted to Bson for esasy insert.


                new Restaurants
                    (
                        new ObjectId("5c39f9b5df831369c19b6bcb"),"Blue Bagels Grill",
                        3,
                        new List<string>{ "Bagels", "Cookies", "Sandwiches"}
                    ).ToBsonDocument(),


                new Restaurants
                    (
                        new ObjectId("5c39f9b5df831369c19b6bcc"),"Hot Bakery Cafe",
                        4,
                        new List<string>{ "Bakery", "Cafe", "Coffee", "Dessert"}
                    ).ToBsonDocument(),


                new Restaurants
                    (
                        new ObjectId("5c39f9b5df831369c19b6bcd"),"XYZ Coffee Bar",
                        5,
                        new List<string>{ "Coffee", "Cafe", "Bakery", "Chocolates"}
                    ).ToBsonDocument(),


                new Restaurants
                    (
                        new ObjectId("5c39f9b5df831369c19b6bce"),"456 Cookies Shop",
                        4,
                        new List<string>{ "Bakery", "Cookies", "Cake", "Coffee"}
                    ).ToBsonDocument()

            };

            lab3Collection.InsertMany(restaurantsList);                                             //Inserting our documents into db

            PrintAllRestaurants();

            PrintAllServingX("Cafe");

            IncrementStarByX("XYZ Coffee Bar", 1);

            UpdateNameXforY("456 Cookies Shop", "123 Cookies Heaven");

            AggByStars(4);


            void PrintAllRestaurants()
            {
                var allRestaurantsFilter = Builders<BsonDocument>.
                    Filter.Empty;                                                                   //A filter that matches everything
                List<BsonDocument> allRestaurantsDoc = 
                    lab3Collection.Find<BsonDocument>(allRestaurantsFilter).
                    ToList<BsonDocument>();                                                         //A list of all restaurants
                foreach (var restaurant in allRestaurantsDoc)                                       //print every restaurant
                {
                    Console.WriteLine(restaurant.ToString());
                }
            }
            void PrintAllServingX(string x)
            {
                var menuFilter = Builders<BsonDocument>.
                    Filter.Eq("Menu",x);                                                            //A filter that finds all restaurants with x on menu
                List<BsonDocument> menuDoc = lab3Collection.
                    Find<BsonDocument>(menuFilter).
                    ToList<BsonDocument>();                                                         //A list of all restaurants  with x on menu
                foreach (var item in menuDoc)                                                       //print the name and menu of each restaurant
                {
                    var restaurantObject = BsonSerializer.
                        Deserialize<Restaurants>(item);
                    var name = restaurantObject.Name;
                    var menu = restaurantObject.Menu;
                    Console.Write($"Name: {name}. Menu: ");
                    foreach (var item2 in menu)
                    {
                        Console.Write($" {item2}, ");
                    }
                    Console.WriteLine();
                }
            }
            void IncrementStarByX(string restaurant, int value)
            {
                var nameFilter = Builders<BsonDocument>.Filter.
                    Eq("Name",restaurant);                                                          //A filter that finds all restaurants with the given name
                var updateStar = Builders<BsonDocument>.Update.
                    Inc("Stars", value);                                                            //an update definition which increments "Stars" by given value
                lab3Collection.UpdateOne(nameFilter, updateStar);                                   //actual update
                PrintAllRestaurants();
            }
            void UpdateNameXforY(string presentName, string desiredName)
            {
                var nameFilter = Builders<BsonDocument>.Filter.
                    Eq("Name", presentName);                                                        //A filter that finds all restaurants with the given name
                var updateName = Builders<BsonDocument>.Update.
                    Set("Name", desiredName);                                                       //an update definition which changes "Name" to the given name
                lab3Collection.UpdateOne(nameFilter, updateName);                                   //actual update
                PrintAllRestaurants();
            }
            void AggByStars(int v)
            {
                var starFilter = Builders<BsonDocument>.
                    Filter.Gte("Stars", v);                                                         //A filter that finds all restaurants with 4 or more stars
                List<BsonDocument> starsDoc = lab3Collection.
                    Find<BsonDocument>(starFilter).
                    ToList<BsonDocument>();                                                         //A list of all restaurants  with 4 or more stars
                foreach (var item in starsDoc)
                {
                    var restaurant = BsonSerializer.
                        Deserialize<Restaurants>(item);
                    if (restaurant.Stars >= 4)
                    {
                        Console.WriteLine($"Name: {restaurant.Name} Stars: {restaurant.Stars}");
                    }
                }
            }
        }
    }
    public class Restaurants
    {
        public ObjectId _id { get; set; }                                                           //This propery allows us to  deserialize a MongoDb documents into a C# object 
        public string Name { get; set; }
        public int Stars { get; set; }
        public List<string> Menu { get; set; }
        public Restaurants
        (
            ObjectId objectId,
            string name,
            int stars,
            List<string> menu
        )
        {
            this._id = objectId;
            this.Name = name;
            this.Stars = stars;
            this.Menu = menu;
        }
    }
}
