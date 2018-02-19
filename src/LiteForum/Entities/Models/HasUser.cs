
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using LiteForum.Entities.Interfaces;
using LiteForum.Models;

namespace LiteForum.Entities.Models
{
    public abstract class HasUser<T> : Entity<T>
    {
        public string UserId { get; set; }
        public virtual LiteForumUser User { get; set; }        
    }

}
