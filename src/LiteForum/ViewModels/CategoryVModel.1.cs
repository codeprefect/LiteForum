using System;
using System.Collections.Generic;

namespace LiteForum.ViewModels
{
    public class CategoryVModel : BaseVModel
    {
        public string Name { get; set; }
        public ICollection<PostVModel> Posts { get; set; }
    }
}