using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

public class BloggingContext : DbContext {
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public void AddBlog(Blog blog) {
        Blogs.Add(blog);
        SaveChanges();
    }

    public void AddPost(Post post) {
        Posts.Add(post);
        // SaveChanges(); // I don't know why this is causing an Exception
    }

    public void DeleteBlog(Blog blog) {
        this.Blogs.Remove(blog);
        this.SaveChanges();
    }

    public Blog GetBlogById(int id){
        foreach(Blog blog in Blogs) if(blog.BlogId == id) return blog;
        return null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
            
        var config = configuration.Build();
        optionsBuilder.UseSqlServer(@config["BlogConsole:ConnectionString"]);
    }
}