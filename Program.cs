using NLog;
using System.Linq;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "/nlog.config";
// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

string selection;
do {
    Console.WriteLine("Enter your selection:\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts\nEnter q to quit");
    selection = Console.ReadLine();

    if(selection == "1" || selection == "2" || selection == "3" || selection == "4" || selection == "q") {
        logger.Info("Option \"{0}\" selected", selection);

        switch(selection) {
            case "1":
                // displayAllBlogs(logger);
                break;
            case "2":
                addBlog(logger);
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

try
{
    // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
    var name = Console.ReadLine();

    var blog = new Blog { Name = name };

    var db = new BloggingContext();
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);

    // Display all Blogs from the database
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine("All blogs in the database:");
    foreach (var item in query)
    {
        Console.WriteLine(item.Name);
    }
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");


static void addBlog(Logger logger) {
    // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
    var name = Console.ReadLine();

    var blog = new Blog { Name = name };

    var db = new BloggingContext();
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);
}