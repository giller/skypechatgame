using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompareAttribute = System.Web.Mvc.CompareAttribute;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SQLite;
using System.Diagnostics;
using System.Web;
using SkypeChatGame.Controllers;
using WebMatrix.WebData;

namespace SkypeChatGame.Models
{
    public class MessageModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public string Author { get; set; }
        public string FakeAuthor1 { get; set; }
        public string FakeAuthor2 { get; set; }
        public string Contents { get; set; }
        public long Timestamp { get; set; }

        public virtual GameModel Game { get; set; }
    }

    public static class Skype
    {
        private static SQLiteConnection db;
        //Path to skype db
        private static string DbPath = "C:/main.db";

        public static Dictionary<string, string> AllowedPeopleDict;
        public static List<string> AllowedPeople;
        public static List<MessageModel> Messages;

        public static void Main()
        {
            db = new SQLiteConnection("Data Source=" + DbPath + ";Version=3;");
            AllowedPeople = new List<string>();
            AllowedPeopleDict = new Dictionary<string, string>();

            //AllowedPeople contains the Skype username as a Key and the display name as the Value
            //I have removed the skype id's from the github version 

            foreach (var e in AllowedPeopleDict)
            {
                AllowedPeople.Add(e.Value);
            }

            Messages = GenerateList();
        }

        public static List<MessageModel> GenerateList()
        {
            var ReturnList = new List<MessageModel>();

            db.Open();

            //SQL command removed to hide convo and skype ID's
            string sql2 = "";
            var command = new SQLiteCommand(sql2, db);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!reader["body_xml"].ToString().Equals(String.Empty) && AllowedPeopleDict.ContainsKey(reader["author"].ToString()))
                {
                    var message = new MessageModel();
                    message.Author = AllowedPeopleDict[reader["author"].ToString()];
                    message.Timestamp = (long)reader["timestamp"];
                    if (!reader["body_xml"].ToString().Equals(String.Empty))
                    {
                        var content = HttpUtility.HtmlDecode(reader["body_xml"].ToString());
                        var ContentArray = content.Split(' ');
                        if (ContentArray.Length < 5) continue;
                        message.Contents = content;
                    }
                    ReturnList.Add(message);
                }
            }
            db.Close();
            return ReturnList;
        }

        public static MessageModel GetMessage()
        {
            var ReturnMessage = new MessageModel();
            var Rand = new Random();
            var Num = Rand.Next(1, Messages.Count);
            ReturnMessage = Messages[Num];
            return ReturnMessage;
        }
    }

    public class ChangeVideoModel
    {
        public string VideoId { get; set; }
    }

    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<GameModel> Games { get; set; }
        public DbSet<MessageModel> Messages { get; set; }

        public UserProfile CurrentUser{
            get { return this.UserProfiles.Find(WebSecurity.CurrentUserId); }
        }

        //Determine wether to use a string or make a meme model
        public GameModel HighScoreGame { get{
            var HighestScore = new GameModel();
            HighestScore.Score = 0;

            if (Games != null)
            {
                foreach (var game in this.Games.Include("Player"))
                {
                    if (game.Score > HighestScore.Score) HighestScore = game;
                }

                return HighestScore;
            }
            else
            {
                return null;
            }
        }}

        //Property to get the Top games, will priortise the most recent game in the case of a tie
        public List<GameModel> TopGames
        {
            get
            {
                var Games = this.Games;

                if (Games != null)
                {
                    var Game1 = new GameModel();
                    var Game2 = new GameModel();
                    var Game3 = new GameModel();

                    foreach (var game in this.Games.Include("Player"))
                    {
                        if (game.Score > Game3.Score)
                        {
                            if (game.Score > Game2.Score)
                            {
                                if (game.Score > Game1.Score) Game1 = game;
                                else
                                {
                                    Game2 = game;
                                }
                            }
                            else
                            {
                                Game3 = game;
                            }
                        }
                    }

                    var List = new List<GameModel>();
                    List.Add(Game1);
                    List.Add(Game2);
                    List.Add(Game3);

                    Debug.WriteLine("Returning List");
                    return List;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        public UserProfile()
        {
            Games = new List<GameModel>();
            HighScore = 0;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? HighScore { get; set; }
        public string Video { get; set; }

        public virtual List<CommentModel> Comments { get; set; }
        public virtual List<GameModel> Games { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
