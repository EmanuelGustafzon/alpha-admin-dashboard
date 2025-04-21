using Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models;

public class NotificationForm
{
    public string? Icon { get; set; }

    public string Message { get; set; } = null!;

    public string? Action { get; set; }
}
