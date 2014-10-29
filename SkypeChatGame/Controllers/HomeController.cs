using SkypeChatGame.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace SkypeChatGame.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public static List<MessageModel> Messages;
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            //Messages = Skype.Messages;
            //var rand = new Random();
            //var num = rand.Next(1, Messages.Count);
            //ViewBag.Message = Messages.ElementAt(num).Author + Messages.ElementAt(num).Contents;


            return View();
        }

        [Authorize]
        [ChildActionOnly]
        public PartialViewResult RecentGames()
        {
            using (var db = new UsersContext())
            {
                var user = db.CurrentUser;

                if (user != null && user.Games != null && user.Games.Count > 0)
                {
                    var HighScoreGames = user.Games.OrderByDescending(g => g.GameId).ToList();

                    if (HighScoreGames.Count >= 10)
                    {
                        HighScoreGames = HighScoreGames.GetRange(0, 10);
                    }
                    else
                    {
                        HighScoreGames = HighScoreGames.GetRange(0, HighScoreGames.Count);
                    }

                    return PartialView("_RecentGames", HighScoreGames);
                }
                else
                {
                    return PartialView("_RecentGames", null);
                }
            }
        }

        [Authorize]
        [ChildActionOnly]
        public PartialViewResult Leaderboards()
        {
            using (var db = new UsersContext())
            {
                var games = db.Games;
                if (games != null && games.Count() > 0)
                {
                    var HighScoreGames = games.Include("Player").OrderByDescending(game => game.Score).ToList();

                    if (HighScoreGames.Count >= 10)
                    {
                        HighScoreGames = HighScoreGames.GetRange(0, 10);
                    }
                    else
                    {
                        HighScoreGames = HighScoreGames.GetRange(0, HighScoreGames.Count);
                    }
                    return PartialView("_Leaderboards", HighScoreGames);
                }
                else
                {
                    return PartialView("_Leaderboards", null);
                }
            }
            //var db = new UsersContext();
            //var games = db.Games;
            //var HighScoreGames = games.OrderByDescending(game => game.Score).ToList();

            //if (HighScoreGames.Count >= 10)
            //{
            //    HighScoreGames = HighScoreGames.GetRange(0, 10);
            //}
            //else
            //{
            //    HighScoreGames = HighScoreGames.GetRange(0, HighScoreGames.Count);
            //}

            //return PartialView("_Leaderboards", HighScoreGames);
        }

        [ChildActionOnly]
        [HttpGet]
        public PartialViewResult AddComment(int id)
        {
            CommentPostModel post = new CommentPostModel();
            post.GameId = id;
            return PartialView("_AddComment", post);
        }

        [ChildActionOnly]
        [HttpPost]
        public ActionResult AddComment(CommentPostModel post)
        {

            using (var db = new UsersContext())
            {
                var User = db.CurrentUser;
                var Games = db.Games.Include("Messages").Include("Comments");

                var Game = Games.First(g => g.GameId == post.GameId);

                var Comment = new CommentModel();
                Comment.Author = User;
                Comment.Game = Game;
                Comment.Body = post.Body;
                Comment.Date = DateTime.Now;
                Game.Comments.Add(Comment);
                db.SaveChanges();

                return PartialView("_AddComment");//RedirectToAction("Game", new { id = Game.GameId }); //"Game", Game);
            }
        }

        [Authorize]
        public ActionResult Game(int id)
        {
            GameModel Game = new GameModel();
            using (var db = new UsersContext())
            {
                var Games = db.Games.Include("Player").Include("Messages").Include("Comments").ToList();

                var First = Games.First();
                var Last = Games.Last();

                if (id < First.GameId || id > Last.GameId) return RedirectToAction("Index");

                Game = Games.Find(g => g.GameId == id);
                return View(Game);
            }
        }

        public void Comment(int id, string body)
        {
            using (var db = new UsersContext())
            {
                var Games = db.Games.Include("Comments").ToList(); ;

                string host = HttpContext.Request.Url.Host;
                string url = "http://" + host + "/home/game";
                if (id < Games.First().GameId || id > Games.Last().GameId) Response.Redirect("http://localhost:10113/home/game/" + id);

                var Game = Games.First(g => g.GameId == id);

                var Comment = new CommentModel();
                Comment.Author = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                Comment.Body = body;
                Comment.Date = DateTime.Now;
                Comment.Game = Game;
                Game.Comments.Add(Comment);

                db.SaveChanges();
                Response.Redirect("http://localhost:10113/home/game/" + id);
            }
        }

        [Authorize]
        public ActionResult Chat()
        {
            using (var db = new UsersContext())
            {
                var Games = db.Games.Include("Player").OrderByDescending(p => p.Score).ThenByDescending(p => p.CompletionTime).Take(3).ToList();
                
                return View(Games);
            }
        }
    }
}
