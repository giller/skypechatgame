using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SkypeChatGame.Models
{
    public class GameModel
    {

        public GameModel()
        {
            Messages = new List<MessageModel>();
            Comments = new List<CommentModel>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }
        public int Score { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public virtual List<CommentModel> Comments { get; set; } 
        public virtual UserProfile Player { get; set; }
        public virtual List<MessageModel> Messages { get; set; }
    }

    public class CommentModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }

        public virtual GameModel Game { get; set; }
        public virtual UserProfile Author { get; set; }
    }

    public class CommentPostModel
    {
        public string Body { get; set; }
        public int GameId { get; set; }
    }

    public class HighScoreModel
    {
        [Key]
        public UserProfile Owner { get; set; }
        public string VideoId { get; set; }
    }
}