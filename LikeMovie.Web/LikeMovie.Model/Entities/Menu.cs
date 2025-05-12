using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

[Table("MENUs")]
public partial class Menu
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? MenuName { get; set; }

    [StringLength(100)]
    public string? MenuLink { get; set; }

    public int? ParentId { get; set; }

    public int? OrderNumber { get; set; }
}
