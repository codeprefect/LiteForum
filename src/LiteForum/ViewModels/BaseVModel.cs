using System;

namespace LiteForum.ViewModels
{
    public abstract class BaseVModel
    {
        public int Id { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string User { get; set; }
    }
}
