using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using System.Linq;
using System.Reflection;
using System.Transactions;

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
        Console.WriteLine();
        logger.Info("Option \"{0}\" selected", selection);

        switch(selection) {
            case "1":
                displayAllBlogs(logger, database);
                break;
            case "2":
                addBlog(logger, database);
                break;
            case "3":
                createPost(logger, database);
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
    try {
        Console.WriteLine("{0} Blogs returned", database.Blogs.Count());

        if(database.Blogs.Count() > 0) {
            foreach(Blog blog in database.Blogs) {
                Console.WriteLine(blog.Name);
            }
        }
    } catch(Exception e) {
        Console.WriteLine(e.Message);
    }
}

static void addBlog(Logger logger, BloggingContext database) {
    try {
        string name;
        // Create and save a new Blog
        Console.Write("\nEnter a name for a new Blog: ");
        name = Console.ReadLine();

        if(!String.IsNullOrWhiteSpace(name)) {
            database.AddBlog(
                new Blog { 
                    Name = name 
                }
            );
            logger.Info("Blog added - {name}", name);
        } else {
            logger.Error("Blog name cannot be null");
        }
    } catch(Exception e) {
        Console.WriteLine(e.Message);
    }
}

static void createPost(Logger logger, BloggingContext database) {
    Console.WriteLine("Select the blog you would like to post to:\n");

    foreach(Blog blog in database.Blogs) 
        Console.WriteLine($"{blog.BlogId}) {blog.Name}");

    string inputId = Console.ReadLine();
    int outputId = 0;
    Blog blogg = new Blog();

    try {
        outputId = int.Parse(inputId.Trim());
        blogg = database.GetBlogById(outputId);
        if(outputId > 0 && outputId < database.Blogs.Count() + 1) {
        Console.WriteLine("Enter the Post title");
        string title = Console.ReadLine();
        if(!string.IsNullOrWhiteSpace(title)) {
            Console.WriteLine("Enter the Post content");
            string content = Console.ReadLine();
            try {
                database.AddPost(
                    new Post() {
                        PostId = database.Posts.Count() + 1, 
                        Title = title, 
                        Content = content, 
                        
                        BlogId = blogg.BlogId, 
                        Blog = blogg
                    }
                );
                logger.Info($"Post added - \"{title}\"");
            } catch (Exception) {
                logger.Error("There was an error creating your post");
            }
        }
        else logger.Error("Post title cannot be null");
    } 
    else logger.Error("There are no Blogs saved with that Id");
    } catch (Exception) {
        logger.Error("Invalid Blog Id");
    }
    
}
