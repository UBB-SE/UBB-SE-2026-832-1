using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Dtos;

public class UserSessionRequestDto
{
    public long UserId { get; set; }
    public long ClientId { get; set; }
}
