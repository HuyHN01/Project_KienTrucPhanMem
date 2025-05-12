using System;

namespace LikeMovie.Model.Entities
{
    public class MENUs
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string MenuLink { get; set; }
        public int? ParentId { get; set; }
        public int? OrderNumber { get; set; }
    }
}