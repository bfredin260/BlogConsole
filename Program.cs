using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using System.ComponentModel.DataAnnotations;
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
    Console.WriteLine("\nEnter your selection:\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts\n5) Delete Blog\n6) Edit Blog\nEnter q to quit");
    selection = Console.ReadLine();

    if(selection == "1" || selection == "2" || selection == "3" || selection == "4" || selection == "5" || selection == "6" ||selection == "q") {
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
                displayPosts(logger, database);
                break;
            case "5":
                deleteBlog(logger, database);
                break;
            case "6":
                editBlog(logger, database);
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
    Blog blog = InputBlog(logger, database);
    if (blog != null) {
        //blog.BlogId = BlogId;
        database.AddBlog(blog);
        logger.Info("Blog added - {name}", blog.Name);
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

static void displayPosts(Logger logger, BloggingContext database) {
    Console.WriteLine("Select the blog's posts to display:\n0) Posts from all blogs");
    foreach(Blog blog in database.Blogs) Console.WriteLine($"{blog.BlogId}) Posts from \"{blog.Name}\"");
    string input = Console.ReadLine();
    switch(input) {
        case "0": 
            Console.WriteLine($"{database.Posts.Count()} post(s) returned");
            foreach(Post post in database.Posts) Console.WriteLine($"Blog: {post.Blog.Name}\nTitle: {post.Title}\nContent: {post.Content}\n"); 
            break;
        default:
            int blogId;
            if(int.TryParse(input, out blogId)) {
                if(blogId > 0 && blogId < database.Blogs.Count() + 1) {
                    int postCount = 0;
                    foreach(Post post in database.Posts) if(post.BlogId == blogId) postCount++;

                    Console.WriteLine($"{postCount} post(s) returned");
                    foreach(Post post1 in database.Posts) if(post1.BlogId == blogId) Console.WriteLine($"Blog: {post1.Blog.Name}\nTitle: {post1.Title}\nContent: {post1.Content}\n"); 

                    // [WARNING] This part doesn't work currently, I don't understand why but the database won't let me SaveChanges()
                    //           after adding a new Post to the DbSet Posts.
                }
                else logger.Error("There are no Blogs saved with that Id");
            }
            else logger.Error("Enter a valid Id");
            break;
    }
}

static void deleteBlog(Logger logger, BloggingContext database) {
    Console.WriteLine("Choose the blog to delete:");
    var blog = GetBlog(logger, database);
    if (blog != null)
    {
       // input blog
        Blog UpdatedBlog = InputBlog(logger, database);
        if (UpdatedBlog != null)
        {
            UpdatedBlog.BlogId = blog.BlogId;
            database.EditBlog(UpdatedBlog);
            logger.Info($"Blog (id: {blog.BlogId}) updated");
        }
        database.DeleteBlog(blog);
        logger.Info($"Blog (id: {blog.BlogId}) deleted");
    }
}

static void editBlog(Logger logger, BloggingContext database) {
    // edit blog
            Console.WriteLine("Choose the blog to edit:");
            var blog = GetBlog(logger, database);
            if (blog != null)
            {

            }
}


static Blog GetBlog(Logger logger, BloggingContext database)
{
    // display all blogs
    var blogs = database.Blogs.OrderBy(b => b.BlogId);
    foreach (Blog b in blogs)
    {
        Console.WriteLine($"{b.BlogId}: {b.Name}");
    }
    if (int.TryParse(Console.ReadLine(), out int BlogId))
    {
        Blog blog = database.Blogs.FirstOrDefault(b => b.BlogId == BlogId);
        if (blog != null)
        {
            return blog;
        }
    }
    logger.Error("Invalid Blog Id");
    return null;
}


static Blog InputBlog(Logger logger, BloggingContext database)
{
    Blog blog = new Blog();
    Console.WriteLine("Enter the Blog name");
    blog.Name = Console.ReadLine();

    ValidationContext context = new ValidationContext(blog, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(blog, context, results, true);
    if (isValid)
    {
        return blog;
    }
    else
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
    return null;
}