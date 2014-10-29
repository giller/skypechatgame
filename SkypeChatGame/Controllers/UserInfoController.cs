using SkypeChatGame.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;

namespace SkypeChatGame.Controllers
{
    public class UserInfoController : ApiController
    {
        // GET api/<controller>
       /* public string Get()
        {
            if(!WebSecurity.IsAuthenticated) return ("Unauthorized Access");
            using (var db = new UsersContext())
            {
                try
                {
                    var name = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                    return name.UserName;
                }

                catch (DataException exc){
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.InnerException.Message);
                    return ("Arguement Error");
                }
            }
                
        }*/


        [Authorize]
        public string Get()
        {
            using (var db = new UsersContext())
            {
                var Game = db.HighScoreGame;
                if (Game.Player == null) return null;
                return Game.Player.Video;
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}