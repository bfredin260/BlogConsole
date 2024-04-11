using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using System.Linq;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "/nlog.config";
// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

var database = new BloggingContext();

string selection;
do {
    Console.WriteLine("\nEnter your selection:\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts\nEnter q to quit");
    selection = Console.ReadLine();

    if(selection == "1" || selection == "2" || selection == "3" || selection == "4" || selection == "q") {
        logger.Info("\nOption \"{0}\" selected", selection);

        switch(selection) {
            case "1":
                displayAllBlogs(logger, database);
                break;
            case "2":
                addBlog(logger, database);
                break;
            case "3":
                // createPost();
                break;
            case "4":
                // displayPosts();
                break;
            default:
                logger.Info("Closing Program...");
                break;
        }

    } else {
        logger.Warn("Please enter a valid selection!\n");
    }
} while(selection != "q");

logger.Info("Program ended");


// -- methods -- //

static void displayAllBlogs(Logger logger, BloggingContext database) {
    Console.WriteLine("{0} Blogs returned", database.Blogs.Count());

    if(database.Blogs.Count() > 0) {
        foreach(Blog blog in database.Blogs) {
            Console.WriteLine(blog.Name);
        }
    }
}

static void addBlog(Logger logger, BloggingContext database) {
    // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
    string name = Console.ReadLine();
    database.AddBlog(
        new Blog { 
            Name = name 
        }
    );
    logger.Info("Blog added - {name}", name);

    // Display all Blogs from the database
    var query = database.Blogs.OrderBy(blog => blog.Name);
}